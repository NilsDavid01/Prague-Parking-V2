using Newtonsoft.Json;

namespace PragueParking
{
    /// <summary>
    /// Representerar en motorcykel
    /// Motorcyklar kan dela parkeringsplatser (upp till 2 per plats)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Motorcycle : Vehicle
    {
        [JsonProperty]
        public override string Type => "MC";
        
        public override decimal HourlyRate => 10;
        public override int RequiredSpots => 1;
        public override string DisplayName => "Motorcycle";

        public Motorcycle() : base() { }
        
        public Motorcycle(string registrationNumber) : base(registrationNumber) { }
    }

    /// <summary>
    /// Representerar en buss
    /// Bussar tar flera parkeringsplatser på grund av sin storlek
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Bus : Vehicle
    {
        [JsonProperty]
        public override string Type => "BUS";
        
        public override decimal HourlyRate => 50;
        public override int RequiredSpots => 4;
        public override string DisplayName => "Bus";

        public Bus() : base() { }
        
        public Bus(string registrationNumber) : base(registrationNumber) { }
        
        /// <summary>
        /// Överskriver validering för att tillåta längre registreringsnummer för bussar
        /// </summary>
        public override bool IsValidRegistration(string registration)
        {
            return !string.IsNullOrWhiteSpace(registration) && registration.Length <= 12;
        }
    }

    /// <summary>
    /// Representerar en cykel
    /// Cyklar är små och flera kan få plats på en parkeringsplats
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Bicycle : Vehicle
    {
        [JsonProperty]
        public override string Type => "BIKE";
        
        public override decimal HourlyRate => 5;
        public override int RequiredSpots => 1;
        public override string DisplayName => "Bicycle";

        public Bicycle() : base() { }
        
        public Bicycle(string registrationNumber) : base(registrationNumber) { }
        
        /// <summary>
        /// Överskriver validering för cykelregistreringsnummer
        /// </summary>
        public override bool IsValidRegistration(string registration)
        {
            return !string.IsNullOrWhiteSpace(registration) && registration.Length <= 8;
        }
    }
}
