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
            Console.WriteLine("=== Francisco Zamora's test ===");
            Console.WriteLine("1. Existent Cars");
            Console.WriteLine("2. New Car");
            Console.WriteLine("3. Update Car information");
            Console.WriteLine("4. Exit");
            Console.WriteLine("/nPlease, select an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    carStock();
                    break;
                case "2":
                    newCar();
                    break;
                case "3":
                    updateCar();
                    break;
                case "4":
                    quit = true;
                    break;
                default:
                    Console.WriteLine("Not a valid option, please select a valid one");
                    break;
            }
        }
    }
    //Prints all the cars that are in stock in the table
    static void carStock()
    {
        List<Car> cars = new List<Car>();

        using (MySqlConnection cnx = new(connDB))
        {
            try
            {
                cnx.Open();

                string query = "select * from car";

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
    static void newCar()
    {

    }
    //Update information of a car
    static void updateCar()
    {

    }
}