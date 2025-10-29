using System;
using System.Linq;
using Spectre.Console;
using DataAccess;

namespace PragueParking
{
    /// <summary>
    /// Huvudapplikationsklass för Prague Parking System
    /// Hanterar användargränssnitt med Spectre.Console
    /// </summary>
    class Program
    {
        private static ParkingGarage garage = new ParkingGarage();
        private static AppConfig? config;
        private static string priceListContent = "";

        static void Main(string[] args)
        {
            LoadConfiguration();
            LoadPriceList();
            LoadParkingData();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => SaveParkingData();
            Console.CancelKeyPress += (s, e) => SaveParkingData();

            while (true)
            {
                ShowMainMenu();
            }
        }

        /// <summary>
        /// Laddar konfiguration från config.json eller skapar standard om den inte finns
        /// </summary>
        static void LoadConfiguration()
        {
            try
            {
                config = MinaFiler.ReadJson<AppConfig>("config/config.json");
                if (config == null)
                {
                    config = new AppConfig
                    {
                        NumberOfParkingSpots = 100,
                        VehicleTypes = new List<VehicleConfig>
                        {
                            new VehicleConfig { Type = "CAR", Name = "Car", HourlyRate = 20, SpotsRequired = 1, CapacityPerSpot = 1 },
                            new VehicleConfig { Type = "MC", Name = "Motorcycle", HourlyRate = 10, SpotsRequired = 1, CapacityPerSpot = 2 },
                            new VehicleConfig { Type = "BUS", Name = "Bus", HourlyRate = 50, SpotsRequired = 4, CapacityPerSpot = 1 },
                            new VehicleConfig { Type = "BIKE", Name = "Bicycle", HourlyRate = 5, SpotsRequired = 1, CapacityPerSpot = 5 }
                        }
                    };
                    MinaFiler.SaveJson("config/config.json", config);
                }

                if (garage.ParkingSpots.Count == 0)
                {
                    garage.InitializeWithConfig(config);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        /// <summary>
        /// Laddar prislistan från textfil eller skapar standard om den inte finns
        /// </summary>
        static void LoadPriceList()
        {
            try
            {
                priceListContent = MinaFiler.ReadTextFile(config?.PriceListFile ?? "config/prices.txt");
                if (string.IsNullOrEmpty(priceListContent))
                {
                    priceListContent = @"# Prague Parking Price List
# All prices in CZK per started hour
# First 10 minutes are free

Car: 20 CZK/hour
Motorcycle: 10 CZK/hour  
Bus: 50 CZK/hour
Bicycle: 5 CZK/hour";
                    System.IO.File.WriteAllText(config?.PriceListFile ?? "config/prices.txt", priceListContent);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        /// <summary>
        /// Laddar parkeringsdata från tidigare session om tillgänglig
        /// </summary>
        static void LoadParkingData()
        {
            try
            {
                var savedData = MinaFiler.ReadJson<ParkingGarage>("parkingdata.json");
                if (savedData != null && savedData.ParkingSpots.Count > 0)
                {
                    garage = savedData;
                    AnsiConsole.MarkupLine("[green]Loaded saved parking data![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]No saved data found, starting fresh...[/]");
                    garage.InitializeWithConfig(config ?? new AppConfig { NumberOfParkingSpots = 100 });
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Could not load parking data: {ex.Message}[/]");
                AnsiConsole.MarkupLine("[yellow]Starting with fresh parking data...[/]");
                garage.InitializeWithConfig(config ?? new AppConfig { NumberOfParkingSpots = 100 });
            }
        }

        /// <summary>
        /// Sparar nuvarande parkeringsdata till JSON-fil
        /// </summary>
        static void SaveParkingData()
        {
            try
            {
                MinaFiler.SaveJson("parkingdata.json", garage);
                AnsiConsole.MarkupLine("[green]Parking data saved successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Could not save parking data: {ex.Message}[/]");
            }
        }

        /// <summary>
        /// Visar huvudmenyn och hanterar användarval
        /// </summary>
        static void ShowMainMenu()
        {
            Console.Clear();
            
            var panel = new Panel("[blue]Dobry den and welcome to David's parking lot in Prague![/]")
                .Header("Prague Parking System")
                .BorderColor(Color.Blue);

            AnsiConsole.Write(panel);

            var availableSpots = garage.GetAvailableSpotsCount();
            var totalSpots = garage.GetTotalSpotsCount();
            var occupiedSpots = garage.GetOccupiedSpotsCount();
            
            int busCount = 0;
            foreach (var spot in garage.ParkingSpots)
            {
                if (spot.Vehicles.Any(v => v is Bus))
                {
                    busCount++;
                }
            }
            
            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddRow("[yellow]Total spots:[/]", $"[green]{totalSpots}[/]");
            grid.AddRow("[yellow]Available spots:[/]", $"[green]{availableSpots}[/]");
            grid.AddRow("[yellow]Occupied spots:[/]", $"[red]{occupiedSpots}[/]");
            if (busCount > 0)
            {
                grid.AddRow("[yellow]Buses parked:[/]", $"[orange1]{busCount} (each uses 4 spots)[/]");
            }

            AnsiConsole.Write(grid);
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Park a vehicle",
                        "Pick up a vehicle", 
                        "Move a vehicle",
                        "Find vehicle by license plate",
                        "Show parking map",
                        "View price list",
                        "Reload price list",
                        "Save data manually",
                        "Exit"
                    }));

            switch (choice)
            {
                case "Park a vehicle":
                    ParkVehicle();
                    break;
                case "Pick up a vehicle":
                    PickUpVehicle();
                    break;
                case "Move a vehicle":
                    MoveVehicle();
                    break;
                case "Find vehicle by license plate":
                    FindVehicle();
                    break;
                case "Show parking map":
                    ShowParkingMap();
                    break;
                case "View price list":
                    ViewPriceList();
                    break;
                case "Reload price list":
                    ReloadPriceList();
                    break;
                case "Save data manually":
                    SaveParkingData();
                    AnsiConsole.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
                case "Exit":
                    SaveParkingData();
                    Environment.Exit(0);
                    break;
            }
        }

        /// <summary>
        /// Hanterar processen att parkera ett nytt fordon
        /// </summary>
        static void ParkVehicle()
        {
            if (config?.VehicleTypes == null)
            {
                AnsiConsole.MarkupLine("[red]Configuration not loaded properly![/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var vehicleType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select vehicle type:")
                    .AddChoices(config.VehicleTypes.Select(vt => vt.Name).ToArray()));

            var registration = AnsiConsole.Ask<string>("Enter registration number (Format: 1-2 letters + 1-4 numbers + 0-2 letters):");

            Vehicle vehicle = vehicleType switch
            {
                "Car" => new Car(registration),
                "Motorcycle" => new Motorcycle(registration),
                "Bus" => new Bus(registration),
                "Bicycle" => new Bicycle(registration),
                _ => throw new ArgumentException("Unknown vehicle type")
            };

            if (!vehicle.IsValidRegistration(registration))
            {
                AnsiConsole.MarkupLine("[red]Invalid registration number format![/]");
                AnsiConsole.MarkupLine("[yellow]Expected format: 1-2 letters, 1-4 numbers, 0-2 letters (max 10 chars)[/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var existingVehicle = garage.FindVehicle(registration);
            if (existingVehicle.found)
            {
                AnsiConsole.MarkupLine($"[red]Vehicle with registration {registration} is already parked at spot {existingVehicle.spotNumber}![/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (vehicle is Bus)
            {
                AnsiConsole.MarkupLine("[yellow]Note: Buses require 4 consecutive empty spots...[/]");
            }

            var result = garage.ParkVehicle(vehicle);
            if (result.success)
            {
                AnsiConsole.MarkupLine($"[green]Vehicle parked successfully at spot {result.spotNumber}![/]");
                if (vehicle is Bus)
                {
                    AnsiConsole.MarkupLine($"[yellow]Bus occupies spots {result.spotNumber} to {result.spotNumber + 3}[/]");
                }
                AnsiConsole.MarkupLine($"[yellow]Arrival time: {vehicle.ArrivalTime}[/]");
                SaveParkingData();
            }
            else
            {
                if (vehicle is Bus)
                {
                    AnsiConsole.MarkupLine("[red]No available consecutive spots for bus! Requires 4 empty spots in a row.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]No available parking spots![/]");
                }
            }

            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Hanterar processen att hämta och ta bort ett fordon
        /// </summary>
        static void PickUpVehicle()
        {
            var registration = AnsiConsole.Ask<string>("Enter registration number:");

            var result = garage.RemoveVehicle(registration);
            if (result.success && result.vehicle != null)
            {
                var vehicle = result.vehicle;
                var parkingDuration = vehicle.GetParkingDuration(DateTime.Now);
                
                AnsiConsole.MarkupLine($"[green]Vehicle {registration} removed from spot {result.spotNumber}[/]");
                if (vehicle is Bus)
                {
                    AnsiConsole.MarkupLine($"[yellow]Bus freed up spots {result.spotNumber} to {result.spotNumber + 3}[/]");
                }
                AnsiConsole.MarkupLine($"[yellow]Parking duration: {parkingDuration}[/]");
                AnsiConsole.MarkupLine($"[yellow]Parking fee: {result.fee} CZK[/]");
                SaveParkingData();
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Vehicle not found![/]");
            }

            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Hanterar flyttning av fordon från en parkeringsplats till en annan
        /// </summary>
        static void MoveVehicle()
        {
            var registration = AnsiConsole.Ask<string>("Enter registration number of vehicle to move:");

            var searchResult = garage.FindVehicle(registration);
            if (!searchResult.found || searchResult.vehicle == null)
            {
                AnsiConsole.MarkupLine("[red]Vehicle not found![/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            AnsiConsole.MarkupLine($"[yellow]Vehicle found at spot {searchResult.spotNumber}[/]");

            var newSpotNumber = AnsiConsole.Ask<int>("Enter new parking spot number (1-100):");
            if (newSpotNumber < 1 || newSpotNumber > garage.GetTotalSpotsCount())
            {
                AnsiConsole.MarkupLine($"[red]Invalid spot number! Must be between 1-{garage.GetTotalSpotsCount()}.[/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            if (searchResult.vehicle is Bus)
            {
                AnsiConsole.MarkupLine("[red]Cannot move buses! Buses require specific 4 consecutive spot allocation.[/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var removeResult = garage.RemoveVehicle(registration);
            if (!removeResult.success || removeResult.vehicle == null)
            {
                AnsiConsole.MarkupLine("[red]Failed to remove vehicle from current spot![/]");
                AnsiConsole.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var vehicle = removeResult.vehicle;
            var newSpot = garage.ParkingSpots[newSpotNumber - 1];
            if (newSpot.CanParkVehicle(vehicle))
            {
                newSpot.ParkVehicle(vehicle);
                SaveParkingData();
                AnsiConsole.MarkupLine($"[green]Vehicle moved successfully to spot {newSpotNumber}![/]");
            }
            else
            {
                var originalSpot = garage.ParkingSpots[searchResult.spotNumber - 1];
                originalSpot.ParkVehicle(vehicle);
                AnsiConsole.MarkupLine("[red]Target parking spot is occupied! Vehicle returned to original spot.[/]");
            }

            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Hittar och visar information om ett specifikt fordon
        /// </summary>
        static void FindVehicle()
        {
            var registration = AnsiConsole.Ask<string>("Enter registration number:");

            var result = garage.FindVehicle(registration);
            if (result.found && result.vehicle != null)
            {
                var vehicle = result.vehicle;
                var parkingDuration = vehicle.GetParkingDuration(DateTime.Now);
                
                AnsiConsole.MarkupLine($"[green]Vehicle found at spot {result.spotNumber}![/]");
                AnsiConsole.MarkupLine($"[yellow]Type: {vehicle.DisplayName}[/]");
                AnsiConsole.MarkupLine($"[yellow]Registration: {vehicle.RegistrationNumber}[/]");
                AnsiConsole.MarkupLine($"[yellow]Arrival Time: {vehicle.ArrivalTime}[/]");
                AnsiConsole.MarkupLine($"[yellow]Parking Duration: {parkingDuration}[/]");
                
                if (vehicle is Bus)
                {
                    AnsiConsole.MarkupLine($"[orange1]This bus occupies spots {result.spotNumber} to {result.spotNumber + 3}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Vehicle not found![/]");
            }

            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Visar en visuell karta över parkeringsgaraget
        /// </summary>
        static void ShowParkingMap()
        {
            AnsiConsole.MarkupLine("[underline blue]Parking Garage Map[/]");
            AnsiConsole.MarkupLine("[yellow]Shows registration numbers and vehicle types[/]\n");

            var mapLines = garage.GenerateDetailedParkingMap();
            foreach (var line in mapLines)
            {
                AnsiConsole.MarkupLine(line);
            }

            AnsiConsole.MarkupLine("\n[green]□ Available[/] [red]■ Occupied[/] [yellow]◐ Partially Full[/]");
            AnsiConsole.MarkupLine("[gray]MCx2 = 2 Motorcycles, BIKEx3 = 3 Bicycles[/]");

            var availableSpots = garage.GetAvailableSpotsCount();
            var totalSpots = garage.GetTotalSpotsCount();
            AnsiConsole.MarkupLine($"\n[yellow]Available: {availableSpots}/{totalSpots} spots[/]");

            AnsiConsole.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Visar prislistan 
        /// </summary>
        static void ViewPriceList()
        {
            var panel = new Panel(priceListContent)
                .Header("Price List")
                .BorderColor(Color.Yellow);

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Laddar om prislistan från fil
        /// </summary>
        static void ReloadPriceList()
        {
            try
            {
                LoadPriceList();
                AnsiConsole.MarkupLine("[green]Price list reloaded successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to reload price list: {ex.Message}[/]");
            }
            
            AnsiConsole.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
