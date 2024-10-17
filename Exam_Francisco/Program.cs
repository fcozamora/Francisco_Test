using MySql.Data.MySqlClient;

class Car
{
    public int CarID { get; set; }

    public required string Make { get; set; }

    public required string Model { get; set; }

    public int Year { get; set; }

    public decimal Price { get; set; }

    public DateTime DateAdded { get; set; }

    public override string ToString()

    {
        return $"{CarID}: {Make} {Model} ({Year}) - ${Price} (Added on {DateAdded.ToShortDateString()})";
    }
}

class Program
{
    static string connDB = "Server=localhost; port=3307; Database=cars_test; UserID=root";
    static void Main(string[] args)
    {
        bool quit = false;

        while (!quit)
        {
            Console.WriteLine("\n=== Francisco Zamora's test ===");
            Console.WriteLine("1. Existent Cars");
            Console.WriteLine("2. New Car");
            Console.WriteLine("3. Update Car information");
            Console.WriteLine("4. Exit");
            Console.Write("\nPlease, select an option: ");

            try
            {
                switch (Console.ReadLine())
                {
                    case "1":
                        CarStock();
                        break;
                    case "2":
                        NewCar();
                        break;
                    case "3":
                        UpdateCar();
                        break;
                    case "4":
                        quit = true;
                        break;
                    default:
                        Console.WriteLine("Not a valid option, please select a valid one");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Program has run succesfully");
            }
            
        }
    }
    //Prints all the cars that are in stock in the table
    static void CarStock()
    {
        List<Car> cars = new List<Car>();

        using (MySqlConnection cnx = new(connDB))
        {
            try
            {
                cnx.Open();

                string query = "select * from Car";

                MySqlCommand cmd = new(query, cnx); //Query Connection to DataBase which should return all cars in the table

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Car car = new Car()
                        {
                            CarID = reader.GetInt32("CarID"),
                            Make = reader.GetString("Make"),
                            Model = reader.GetString("Model"),
                            Year = reader.GetInt32("Year"),
                            Price = reader.GetDecimal("Price"),
                            DateAdded = reader.GetDateTime("DateAdded")
                        };
                        cars.Add(car);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            foreach (var car in cars)
            {
                Console.WriteLine(car);
            }
            
        }
    }
    //Add a new car to the table
    static void NewCar()
    {
        using (MySqlConnection cnx = new(connDB))
        {
            try
            {
                Console.Write("Brand of the car: ");
                string make = Console.ReadLine();
                Console.Write("\nModel of the car: ");
                string model = Console.ReadLine();
                Console.Write("\nYear of the car: ");
                int year = int.Parse(Console.ReadLine());
                Console.Write("\nPrice of the car: ");
                decimal price = decimal.Parse(Console.ReadLine());
                DateTime dateAdded = DateTime.Today;

                cnx.Open();

                string query = "INSERT INTO Car (Make, Model, Year, Price, DateAdded) VALUES (@Make, @Model, @Year, @Price, @DateAdded)";
                
                MySqlCommand queryComm = new(query, cnx); //Send the query to the database to insert the new car information
                queryComm.Parameters.AddWithValue("@Make", make);
                queryComm.Parameters.AddWithValue("@Model", model);
                queryComm.Parameters.AddWithValue("@Year", year);
                queryComm.Parameters.AddWithValue("@Price", price);
                queryComm.Parameters.AddWithValue("@DateAdded", dateAdded);

                int rowsAffected = queryComm.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("\nCar added succesfully");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }            
            finally
            {
                Console.WriteLine("\nProgram has run succesfully");
            }
        }
    }
    //Update information of a car
    static void UpdateCar()
    {
        Console.WriteLine("\n=== Car list ===");
        
        CarStock(); //Printing list of cars to choose from

        Console.Write("\nSelect the car you want to update information for: ");
        int carId = int.Parse(Console.ReadLine());

        using (MySqlConnection cnx = new(connDB))
        {
            try
            {
                cnx.Open();

                string query = "SELECT * FROM Car WHERE CarID = @CarID"; //Select all data for the selected car

                MySqlCommand queryComm = new(query, cnx); //Send the query to the database to get car information

                queryComm.Parameters.AddWithValue("@CarID", carId);

                MySqlDataReader reader = queryComm.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("=== Car Information ===");
                    Console.WriteLine($"Car ID: {reader["CarID"]}");
                    Console.WriteLine($"Brand: {reader["Make"]}");
                    Console.WriteLine($"Model: {reader["Model"]}");
                    Console.WriteLine($"Year: {reader["Year"]}");
                    Console.WriteLine($"Price: {reader["Price"]}");
                    Console.WriteLine($"DateAdded: {reader["DateAdded"]}");
                    reader.Close();

                    //Variables to save new information
                    string newMake = null;
                    string newModel = null;
                    int? newYear = null;
                    decimal? newPrice = null;

                    //Typing new information
                    Console.Write("Type the new brand (Leave empty if there are no changes): ");
                    string input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input)) newMake = input;

                    Console.Write("Type the model (Leave empty if there are no changes): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input)) newModel = input;

                    Console.Write("Type the new year (Leave empty if there are no changes): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input)) newYear = int.Parse(input);

                    Console.Write("Type the new price (Leave empty if there are no changes): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input)) newPrice = decimal.Parse(input);

                    //Creating Query to update the new information
                    string updateQuery = "UPDATE Car SET ";

                    //This list will hold only the data to be updated
                    List<string> updateFields = new List<string>();
                    MySqlCommand updateCommand = new MySqlCommand();

                    //If the variable is not null or has value, it saves the data in the updateFields list and the updateCommand as the new data
                    if (newMake != null)
                    {
                        updateFields.Add("Make = @Make");
                        updateCommand.Parameters.AddWithValue("@Make", newMake);
                    }

                    if (newModel != null)
                    {
                        updateFields.Add("Model = @Model");
                        updateCommand.Parameters.AddWithValue("@Model", newModel);
                    }

                    if (newYear.HasValue)
                    {
                        updateFields.Add("Year = @Year");
                        updateCommand.Parameters.AddWithValue("@Year", newYear.Value);
                    }

                    if (newPrice.HasValue)
                    {
                        updateFields.Add("Price = @Price");
                        updateCommand.Parameters.AddWithValue("@Price", newPrice.Value);
                    }

                    if (updateFields.Count > 0)
                    {
                        //It finishes de query to be send to the data base by adding the fields to be updated from the updateFields
                        updateQuery += string.Join(", ", updateFields) + " WHERE CarID = @CarID";
                        updateCommand.CommandText = updateQuery;
                        updateCommand.Connection = cnx;
                        updateCommand.Parameters.AddWithValue("@CarID", carId);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Car information has been updated succesfully");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No changes made, all field were left empty");
                    }
                }
                else
                {
                    Console.WriteLine("There are no cars with the ID selected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("\nProgram has run succesfully");
            }
        }

    }
}
