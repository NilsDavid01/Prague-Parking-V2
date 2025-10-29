using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PragueParking
{
    /// <summary>
    /// Representerar en enskild parkeringsplats som kan innehålla flera fordon
    /// Hanterar parkeringslogik och fordonshantering för en plats
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ParkingSpot
    {
        [JsonProperty]
        public int SpotNumber { get; set; }
        
        [JsonProperty]
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        
        [JsonProperty]
        public bool IsBusOccupied { get; set; }
        
        [JsonProperty]
        public string OccupiedByBus { get; set; } = string.Empty;

        [JsonIgnore]
        public int Capacity 
        { 
            get 
            {
                if (IsBusOccupied)
                    return 0;
                    
                if (Vehicles.Count == 0)
                    return 1;
                
                var firstVehicle = Vehicles.First();
                if (firstVehicle is Motorcycle || firstVehicle is Bicycle)
                {
                    return firstVehicle is Motorcycle ? 2 : 5;
                }
                else
                {
                    return 1;
                }
            }
        }
        
        [JsonIgnore]
        public bool IsAvailable => AvailableSpace > 0 && !IsBusOccupied;
        
        [JsonIgnore]
        public int AvailableSpace 
        { 
            get 
            {
                if (IsBusOccupied)
                    return 0;
                    
                if (Vehicles.Count == 0)
                    return Capacity;
                
                var firstVehicle = Vehicles.First();
                
                if (firstVehicle is Car || firstVehicle is Bus)
                    return 0;
                
                if (firstVehicle is Motorcycle)
                    return Math.Max(0, 2 - Vehicles.Count);
                
                if (firstVehicle is Bicycle)
                    return Math.Max(0, 5 - Vehicles.Count);
                
                return 0;
            }
        }

        public ParkingSpot() { }

        public ParkingSpot(int spotNumber)
        {
            SpotNumber = spotNumber;
        }

        /// <summary>
        /// Markerar denna plats som upptagen av en buss
        /// </summary>
        public void MarkAsOccupiedByBus(string busRegistration)
        {
            IsBusOccupied = true;
            OccupiedByBus = busRegistration;
        }

        /// <summary>
        /// Rensar bussockupation från denna plats
        /// </summary>
        public void ClearBusOccupancy(string busRegistration)
        {
            if (OccupiedByBus == busRegistration)
            {
                IsBusOccupied = false;
                OccupiedByBus = string.Empty;
            }
        }

        /// <summary>
        /// Kontrollerar om denna plats är ockuperad av en specifik buss
        /// </summary>
        public bool IsOccupiedByBus(string busRegistration)
        {
            return IsBusOccupied && OccupiedByBus == busRegistration;
        }

        /// <summary>
        /// Kontrollerar om ett fordon kan parkera på denna plats
        /// </summary>
        public bool CanParkVehicle(Vehicle vehicle)
        {
            if (IsBusOccupied)
                return false;
            
            if (Vehicles.Count == 0)
                return true;
            
            var firstVehicle = Vehicles.First();
            
            if (firstVehicle.GetType() != vehicle.GetType())
                return false;
            
            if (vehicle is Car || vehicle is Bus)
                return false;
            
            if (vehicle is Motorcycle)
                return Vehicles.Count < 2;
            
            if (vehicle is Bicycle)
                return Vehicles.Count < 5;
            
            return false;
        }

        /// <summary>
        /// Parkerar ett fordon på denna plats
        /// </summary>
        public void ParkVehicle(Vehicle vehicle)
        {
            if (!CanParkVehicle(vehicle))
                throw new InvalidOperationException($"Not enough space in parking spot {SpotNumber} for {vehicle.DisplayName}");
            
            Vehicles.Add(vehicle);
        }

        /// <summary>
        /// Tar bort ett fordon från denna plats via registreringsnummer
        /// </summary>
        public bool RemoveVehicle(string registrationNumber)
        {
            var vehicle = Vehicles.FirstOrDefault(v => 
                v.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase));
            
            if (vehicle != null)
            {
                Vehicles.Remove(vehicle);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Hittar ett fordon på denna plats via registreringsnummer
        /// </summary>
        public Vehicle? GetVehicle(string registrationNumber)
        {
            return Vehicles.FirstOrDefault(v => 
                v.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returnerar en strängrepresentation av parkeringsplatsens innehåll
        /// </summary>
        public override string ToString()
        {
            if (IsBusOccupied)
                return $"Bus-Occupied by {OccupiedByBus}";
            
            if (Vehicles.Count == 0)
                return "Empty";
            
            return string.Join(" | ", Vehicles.Select(v => $"{v.Type}#{v.RegistrationNumber}"));
        }

        /// <summary>
        /// Hämtar en visuell representation av platsstatus för parkeringskartan
        /// </summary>
        public char GetMapSymbol()
        {
            if (IsBusOccupied) 
                return '■';
            
            if (Vehicles.Count == 0) 
                return '□';
            
            if (!IsAvailable) 
                return '■';
            
            return '◐';
        }

        /// <summary>
        /// Hämtar registreringsnummer för visning på kartan
        /// </summary>
        public string GetRegistrationDisplay()
        {
            if (IsBusOccupied)
                return $"Bus:{OccupiedByBus}";
            
            if (Vehicles.Count == 0)
                return "Empty";
            
            if (Vehicles.Count == 1)
                return Vehicles[0].RegistrationNumber;
            
            return $"{Vehicles[0].RegistrationNumber}(+{Vehicles.Count - 1})";
        }

        /// <summary>
        /// Hämtar fordonsslag för visning på kartan
        /// </summary>
        public string GetVehicleTypeDisplay()
        {
            if (IsBusOccupied)
                return "BUS";
            
            if (Vehicles.Count == 0)
                return "---";
            
            var distinctTypes = Vehicles.Select(v => v.Type).Distinct().ToList();
            if (distinctTypes.Count == 1)
            {
                var firstVehicle = Vehicles.First();
                if (firstVehicle is Motorcycle && Vehicles.Count > 1)
                    return $"MCx{Vehicles.Count}";
                else if (firstVehicle is Bicycle && Vehicles.Count > 1)
                    return $"BIKEx{Vehicles.Count}";
                else
                    return firstVehicle.Type;
            }
            
            return "Mixed";
        }
    }
}
