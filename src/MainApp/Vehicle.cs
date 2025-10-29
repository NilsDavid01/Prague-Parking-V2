using System;
using Newtonsoft.Json;

namespace PragueParking
{
    /// <summary>
    /// Abstrakt basklass som representerar ett fordon i parkeringssystemet
    /// Innehåller gemensamma egenskaper och metoder för alla fordonsslag
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Vehicle
    {
        [JsonProperty]
        public string RegistrationNumber { get; set; } = string.Empty;
        
        [JsonProperty]
        public DateTime ArrivalTime { get; set; }
        
        public abstract string Type { get; }
        public abstract decimal HourlyRate { get; }
        public abstract int RequiredSpots { get; }
        public abstract string DisplayName { get; }

        /// <summary>
        /// Standardkonstruktor för JSON deserialisering
        /// </summary>
        protected Vehicle() { }

        /// <summary>
        /// Skapar ett nytt fordon med registreringsnummer
        /// </summary>
        protected Vehicle(string registrationNumber)
        {
            RegistrationNumber = registrationNumber;
            ArrivalTime = DateTime.Now;
        }

        /// <summary>
        /// Validerar om ett registreringsnummer matchar förväntat format
        /// </summary>
        public virtual bool IsValidRegistration(string registration)
        {
            string regexPattern = @"^[A-Za-z]{1,2}[0-9]{1,4}[A-Za-z]{0,2}$";
            return !string.IsNullOrWhiteSpace(registration) && 
                   registration.Length <= 10 &&
                   System.Text.RegularExpressions.Regex.IsMatch(registration, regexPattern);
        }

        /// <summary>
        /// Beräknar parkeringsavgiften baserat på parkerad tid
        /// </summary>
        public decimal CalculateParkingFee(DateTime departureTime)
        {
            TimeSpan parkingDuration = departureTime - ArrivalTime;
            double totalMinutes = parkingDuration.TotalMinutes;
            
            // Första 10 minuterna är gratis
            if (totalMinutes <= 10)
                return 0;

            // Dra av de gratis 10 minuterna
            double chargedMinutes = totalMinutes - 10;
            
            // Beräkna timmar (rundar alltid upp till närmaste timme)
            int hours = (int)Math.Ceiling(chargedMinutes / 60.0);
            
            // Se till att minst 1 timme debiteras efter gratisperiod
            hours = Math.Max(1, hours);
            
            return hours * HourlyRate;
        }

        /// <summary>
        /// Hämtar parkeringstiden som en läsbar sträng
        /// </summary>
        public string GetParkingDuration(DateTime currentTime)
        {
            TimeSpan duration = currentTime - ArrivalTime;
            return $"{(int)duration.TotalHours}h {duration.Minutes}m";
        }
    }
}
