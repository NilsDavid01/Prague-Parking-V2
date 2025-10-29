using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PragueParking
{
    /// <summary>
    /// Representerar hela parkeringsgaraget som innehåller flera parkeringsplatser
    /// Hanterar parkeringsoperationer över alla platser
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ParkingGarage
    {
        [JsonProperty]
        public List<ParkingSpot> ParkingSpots { get; set; } = new List<ParkingSpot>();
        
        [JsonProperty]
        public string Name { get; set; } = "David's Parking Lot in Prague";

        public ParkingGarage() { }

        /// <summary>
        /// Initierar garaget med ett specifikt antal tomma parkeringsplatser
        /// </summary>
        public void Initialize(int numberOfSpots)
        {
            ParkingSpots.Clear();
            for (int i = 0; i < numberOfSpots; i++)
            {
                ParkingSpots.Add(new ParkingSpot(i + 1));
            }
        }

        /// <summary>
        /// Initierar garaget med konfigurationsbaserade platser
        /// </summary>
        public void InitializeWithConfig(AppConfig config)
        {
            ParkingSpots.Clear();
            for (int i = 0; i < config.NumberOfParkingSpots; i++)
            {
                ParkingSpots.Add(new ParkingSpot(i + 1));
            }
        }

        /// <summary>
        /// Parkerar ett fordon på första tillgängliga plats(er)
        /// </summary>
        public (bool success, int spotNumber) ParkVehicle(Vehicle vehicle)
        {
            if (vehicle is Bus)
            {
                return ParkBus((Bus)vehicle);
            }

            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                var spot = ParkingSpots[i];
                if (spot.CanParkVehicle(vehicle))
                {
                    vehicle.ArrivalTime = DateTime.Now;
                    spot.ParkVehicle(vehicle);
                    return (true, spot.SpotNumber);
                }
            }
            return (false, -1);
        }

        /// <summary>
        /// Special parkeringslogik för bussar som kräver 4 på varandra följande platser
        /// </summary>
        private (bool success, int spotNumber) ParkBus(Bus bus)
        {
            for (int i = 0; i <= ParkingSpots.Count - 4; i++)
            {
                bool consecutiveSpotsAvailable = true;
                
                for (int j = 0; j < 4; j++)
                {
                    var spot = ParkingSpots[i + j];
                    if (!spot.IsAvailable || spot.Vehicles.Count > 0)
                    {
                        consecutiveSpotsAvailable = false;
                        break;
                    }
                }

                if (consecutiveSpotsAvailable)
                {
                    bus.ArrivalTime = DateTime.Now;
                    ParkingSpots[i].ParkVehicle(bus);
                    
                    for (int j = 1; j < 4; j++)
                    {
                        ParkingSpots[i + j].MarkAsOccupiedByBus(bus.RegistrationNumber);
                    }
                    
                    return (true, ParkingSpots[i].SpotNumber);
                }
            }
            return (false, -1);
        }

        /// <summary>
        /// Tar bort ett fordon från garaget via registreringsnummer
        /// </summary>
        public (bool success, int spotNumber, decimal fee, Vehicle? vehicle) RemoveVehicle(string registrationNumber)
        {
            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                var vehicle = ParkingSpots[i].GetVehicle(registrationNumber);
                if (vehicle != null)
                {
                    decimal fee = vehicle.CalculateParkingFee(DateTime.Now);
                    
                    if (vehicle is Bus)
                    {
                        RemoveBusOccupancy(registrationNumber, i);
                    }
                    
                    ParkingSpots[i].RemoveVehicle(registrationNumber);
                    return (true, ParkingSpots[i].SpotNumber, fee, vehicle);
                }
                
                if (ParkingSpots[i].IsOccupiedByBus(registrationNumber))
                {
                    var busSpot = FindBusMainSpot(registrationNumber);
                    if (busSpot != -1)
                    {
                        return RemoveVehicle(registrationNumber);
                    }
                }
            }
            return (false, -1, 0, null);
        }

        /// <summary>
        /// Tar bort bussockupationsmarkörer från platser
        /// </summary>
        private void RemoveBusOccupancy(string registrationNumber, int mainSpotIndex)
        {
            for (int j = 1; j < 4 && mainSpotIndex + j < ParkingSpots.Count; j++)
            {
                ParkingSpots[mainSpotIndex + j].ClearBusOccupancy(registrationNumber);
            }
        }

        /// <summary>
        /// Hittar huvudplatsen där en buss faktiskt är parkerad
        /// </summary>
        private int FindBusMainSpot(string registrationNumber)
        {
            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                var vehicle = ParkingSpots[i].GetVehicle(registrationNumber);
                if (vehicle != null && vehicle is Bus)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Hittar ett fordon i garaget via registreringsnummer
        /// </summary>
        public (bool found, int spotNumber, Vehicle? vehicle) FindVehicle(string registrationNumber)
        {
            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                var vehicle = ParkingSpots[i].GetVehicle(registrationNumber);
                if (vehicle != null)
                {
                    return (true, ParkingSpots[i].SpotNumber, vehicle);
                }
            }
            return (false, -1, null);
        }

        /// <summary>
        /// Räknar hur många parkeringsplatser som för närvarande är tillgängliga
        /// </summary>
        public int GetAvailableSpotsCount()
        {
            int availableCount = 0;
            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                if (ParkingSpots[i].IsAvailable && !ParkingSpots[i].IsBusOccupied)
                {
                    availableCount++;
                }
            }
            return availableCount;
        }

        /// <summary>
        /// Hämtar det totala antalet parkeringsplatser i garaget
        /// </summary>
        public int GetTotalSpotsCount()
        {
            return ParkingSpots.Count;
        }

        /// <summary>
        /// Hämtar antalet ockuperade platser
        /// </summary>
        public int GetOccupiedSpotsCount()
        {
            return GetTotalSpotsCount() - GetAvailableSpotsCount();
        }

        /// <summary>
        /// Genererar en visuell karta över parkeringsgaraget
        /// </summary>
        public List<string> GenerateParkingMap()
        {
            var map = new List<string>();
            int spotsPerRow = 10;
            
            for (int i = 0; i < ParkingSpots.Count; i += spotsPerRow)
            {
                var rowSymbols = new List<char>();
                var rowNumbers = new List<string>();
                
                for (int j = 0; j < spotsPerRow && i + j < ParkingSpots.Count; j++)
                {
                    var spot = ParkingSpots[i + j];
                    rowSymbols.Add(spot.GetMapSymbol());
                    rowNumbers.Add((i + j + 1).ToString("000"));
                }
                
                map.Add($"Spots {i + 1:000}-{i + spotsPerRow:000}: " + string.Join(" ", rowSymbols));
                map.Add("Numbers:      " + string.Join(" ", rowNumbers));
                map.Add("");
            }
            
            return map;
        }

        /// <summary>
        /// Genererar en detaljerad karta med registreringsnummer
        /// </summary>
        public List<string> GenerateDetailedParkingMap()
        {
            var map = new List<string>();
            int spotsPerRow = 5;
            
            for (int i = 0; i < ParkingSpots.Count; i += spotsPerRow)
            {
                var spotNumbers = new List<string>();
                for (int j = 0; j < spotsPerRow && i + j < ParkingSpots.Count; j++)
                {
                    spotNumbers.Add($"Spot {(i + j + 1):000}");
                }
                map.Add(string.Join("    ", spotNumbers));
                
                var statusRow = new List<string>();
                for (int j = 0; j < spotsPerRow && i + j < ParkingSpots.Count; j++)
                {
                    var spot = ParkingSpots[i + j];
                    statusRow.Add(spot.GetMapSymbol().ToString().PadRight(3));
                }
                map.Add(string.Join("    ", statusRow));
                
                var regNumbersRow = new List<string>();
                for (int j = 0; j < spotsPerRow && i + j < ParkingSpots.Count; j++)
                {
                    var spot = ParkingSpots[i + j];
                    regNumbersRow.Add(spot.GetRegistrationDisplay().PadRight(12));
                }
                map.Add(string.Join("    ", regNumbersRow));
                
                var typesRow = new List<string>();
                for (int j = 0; j < spotsPerRow && i + j < ParkingSpots.Count; j++)
                {
                    var spot = ParkingSpots[i + j];
                    typesRow.Add(spot.GetVehicleTypeDisplay().PadRight(12));
                }
                map.Add(string.Join("    ", typesRow));
                
                map.Add("");
            }
            
            return map;
        }
    }
}
