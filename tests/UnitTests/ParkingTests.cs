using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking;
using System;

namespace UnitTests
{
    [TestClass]
    public class ParkingTests
    {
        [TestMethod]
        public void ParkCar_ShouldOccupyOneSpot()
        {
            // Förbered testdata
            var garage = new ParkingGarage();
            garage.Initialize(10);
            var car = new Car("ABC123");

            // Utför testet
            var result = garage.ParkVehicle(car);

            // Verifiera resultatet
            Assert.IsTrue(result.success);
            Assert.AreEqual(1, result.spotNumber);
        }

        [TestMethod]
        public void RemoveVehicle_ShouldFreeUpSpot()
        {
            // Skapa garage och parkera fordon
            var garage = new ParkingGarage();
            garage.Initialize(10);
            var car = new Car("ABC123");
            garage.ParkVehicle(car);

            // Ta bort fordonet
            var result = garage.RemoveVehicle("ABC123");

            // Kontrollera att platsen frigjorts
            Assert.IsTrue(result.success);
            Assert.AreEqual(10, garage.GetAvailableSpotsCount());
        }

        [TestMethod]
        public void CalculateParkingFee_First10MinutesFree()
        {
            // Skapa bil parkerad i 5 minuter
            var car = new Car("ABC123");
            car.ArrivalTime = DateTime.Now.AddMinutes(-5); // Parkerad 5 minuter sedan

            // Beräkna parkeringsavgift
            var fee = car.CalculateParkingFee(DateTime.Now);

            // Verifiera att avgiften är 0 för kort parkering
            Assert.AreEqual(0, fee);
        }

        [TestMethod]
        public void CalculateParkingFee_OneHourShouldChargeFullRate()
        {
            // Bil parkerad i 1 timme och 1 minut
            var car = new Car("ABC123");
            car.ArrivalTime = DateTime.Now.AddHours(-1).AddMinutes(-1); // 1h 1min

            // Beräkna avgift
            var fee = car.CalculateParkingFee(DateTime.Now);

            // Kontrollera att 1 timme debiteras
            Assert.AreEqual(20, fee); // 1 timme * 20 CZK
        }

        [TestMethod]
        public void FindVehicle_ShouldReturnCorrectSpot()
        {
            // Parkera bil i garaget
            var garage = new ParkingGarage();
            garage.Initialize(10);
            var car = new Car("ABC123");
            garage.ParkVehicle(car);

            // Sök efter fordonet
            var result = garage.FindVehicle("ABC123");

            // Verifiera att fordonet hittas på rätt plats
            Assert.IsTrue(result.found);
            Assert.AreEqual(1, result.spotNumber);
            Assert.IsNotNull(result.vehicle);
            Assert.AreEqual("ABC123", result.vehicle.RegistrationNumber);
        }

        [TestMethod]
        public void ParkMotorcycle_CanShareSpotWithAnotherMotorcycle()
        {
            // Skapa två motorcyklar
            var garage = new ParkingGarage();
            garage.Initialize(10);
            var mc1 = new Motorcycle("MC001");
            var mc2 = new Motorcycle("MC002");

            // Parkera båda motorcyklarna
            var result1 = garage.ParkVehicle(mc1);
            var result2 = garage.ParkVehicle(mc2);

            // Kontrollera att de delar samma plats
            Assert.IsTrue(result1.success);
            Assert.IsTrue(result2.success);
            // Båda motorcyklarna ska vara på samma plats
            Assert.AreEqual(result1.spotNumber, result2.spotNumber);
        }

        [TestMethod]
        public void ParkBus_ShouldRequireFourConsecutiveSpots()
        {
            // Skapa garage och buss
            var garage = new ParkingGarage();
            garage.Initialize(10);
            var bus = new Bus("BUS001");

            // Försök parkera bussen
            var result = garage.ParkVehicle(bus);

            // Verifiera att bussen kräver 4 platser
            Assert.IsTrue(result.success);
            // Bussen ska parkeras på en av de första 7 platserna för att få 4 platser i rad
            Assert.IsTrue(result.spotNumber >= 1 && result.spotNumber <= 7);
        }

        [TestMethod]
        public void Vehicle_ValidRegistrationNumber_ShouldPassValidation()
        {
            // Skapa bil med giltigt registreringsnummer
            var car = new Car("AB123");

            // Validera registreringsnumret
            var isValid = car.IsValidRegistration("AB123");

            // Kontrollera att valideringen godkänns
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void Vehicle_InvalidRegistrationNumber_ShouldFailValidation()
        {
            // Skapa bil med ogiltigt registreringsnummer
            var car = new Car("");

            // Validera ogiltigt nummer
            var isValid = car.IsValidRegistration("INVALID_REGISTRATION_TOO_LONG");

            // Verifiera att valideringen misslyckas
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ParkingSpot_PartiallyFull_ShouldShowCorrectSymbol()
        {
            // Skapa parkeringsplats med en motorcykel
            var spot = new ParkingSpot(1);
            var mc1 = new Motorcycle("MC001");

            // Parkera motorcykeln och hämta symbolsymbol
            spot.ParkVehicle(mc1);
            var symbol = spot.GetMapSymbol();

            // Kontrollera att delvis full symbol visas
            Assert.AreEqual('◐', symbol); // Ska visa delvis full symbol
        }
    }
}