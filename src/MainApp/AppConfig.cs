using System.Collections.Generic;

namespace PragueParking
{
    /// <summary>
    /// Konfigurationsklass för parkeringssystemet
    /// Innehåller inställningar som kan modifieras via config.json
    /// </summary>
    public class AppConfig
    {
        public int NumberOfParkingSpots { get; set; } = 100;
        public List<VehicleConfig> VehicleTypes { get; set; } = new List<VehicleConfig>();
        public string PriceListFile { get; set; } = "config/prices.txt";
        public CapacityConfig Capacities { get; set; } = new CapacityConfig();
    }

    /// <summary>
    /// Konfiguration för ett specifikt fordonsslag
    /// Definierar parkeringsregler och priser för varje fordonsslag
    /// </summary>
    public class VehicleConfig
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public int SpotsRequired { get; set; }
        public int CapacityPerSpot { get; set; } = 1;
    }

    /// <summary>
    /// Kapacitetskonfiguration för olika fordonsslag
    /// </summary>
    public class CapacityConfig
    {
        public int CarsPerSpot { get; set; } = 1;
        public int MotorcyclesPerSpot { get; set; } = 2;
        public int BusesPerSpot { get; set; } = 1;
        public int BicyclesPerSpot { get; set; } = 5;
        public int BusSpotsRequired { get; set; } = 4;
    }
}
