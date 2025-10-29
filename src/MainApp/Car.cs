using Newtonsoft.Json;

namespace PragueParking
{
    /// <summary>
    /// Representerar en bil
    /// Ärver från Vehicle basklass och implementerar bil-specifika egenskaper
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Car : Vehicle
    {
        [JsonProperty]
        public override string Type => "CAR";
        
        public override decimal HourlyRate => 20;
        public override int RequiredSpots => 1;
        public override string DisplayName => "Car";

        public Car() : base() { }
        
        public Car(string registrationNumber) : base(registrationNumber) { }
    }
}
