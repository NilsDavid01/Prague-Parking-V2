## Dag 1: Projektstart
Jag skapade .NET-lösning med tre projekt: MainApp (huvudprogram), UnitTests (tester) och DataAccess (fillagring). Jag implementerade de fem olika grundläggande klasserna ParkingSpot, ParkingGarage, Vehicle, Car samt Motorcycle. Vehicle är en abstrakt basklass med gemensamma egenskaper. DataAccess hanterar den JSON-serialisering som upprätthåller typer vid ett arv. Vehicle-klassen designades med abstrakta egenskaper för Type, HourlyRate, RequiredSpots och DisplayName för att säkerställa korrekt implementering i underklasserna. Jag skapade MinaFiler-klassen med metoder för ReadJson, SaveJson och ReadTextFile.
## Dag 2: Fordonssystem
Jag utvecklade följande fordonstyper: Bil (20 CZK/timme, 1 plats), Motorcykel (10 CZK/timme, 2 per plats), Buss (50 CZK/timme, 4 platser), Cykel (5 CZK/timme, 5 per plats). ParkingSpot kan hantera olika fordonstyper och olika kapaciteter. Särskild parkeringslogik för bussar som kräver 4 platser i rad.
Det var väldigt utmanande för mig att hitta en lösning för att hantera bussar som kräver 4 platser. Detta löste jag genom att implementera ParkBus-metoden som söker igenom alla tillgängliga parkeringsplatser för att leta upp en sekvens av fyra tomma platser.  
## Dag 3: Garage och data
ParkingGarage samordnar alla platser. Jag skapade parkeringsoperationerna som gör det möjligt att parkera, att hämta och att söka upp. 4 tomma platser i rad söks upp när en buss ska parkeras. Data sparas och laddas automatiskt. Prisberäkning med tio gratisminuter, sedan per timme som har påbörjats.
Jag förbättrade datahanteringen genom att implementera automatisk sparning och laddning av pareringsdata vid programmets start och avslutning. 
AppConfig-klassen skapade jag för att kunna hantera konfigurationsinställningar som antal parkeringsplatser och fordonstyper.
## Dag 4: Användargränssnitt
Spectre.Console-menyer som består av 8 olika alternativ: parkera, hämta, flytta, söka, karta, prislista, ladda om, avsluta. Menysystemet är användarvänligt med tydliga val och visuellt tilltalande komponenter. Validering av registreringsnummer utförs varje gång en person anger ett registreringsnummer. Felhantering med tydliga meddelanden framstår när ogiltiga inputs anges. Parkeringstiden och avgiften visas vid utcheckning av fordon.
## Dag 5: Parkeringskarta
Idag började jag jobba på parkeringskartan. Parkeringskartan visar symboler för varje plats som indikerar statusen av den platsen. Symbolernas betydelse:  □ tom, ■ full, ◐ delvis full. Varje upptagen plats indikerar ett regnummer och fordonstyp. Det finns 5 platser per rad. När flera fordon står parkerade på samma plats, visas ett regnummer tillsammans med antalet fordon och vilken typ av fordon som står parkerad. 4 platser markeras av en parkerad buss. 
Målet med parkeringskartan är att leverera användbar och viktig information om varje parkeringsplats för användaren. 
## Dag 6: Konfigurationssystem och prislista
JSON-konfiguration av antal platser, fordonstyper samt priser och kapaciteter. Textbaserad prislista med #-kommentarer. Prislistan kan laddas om utan någon omstart. Automatisk standardkonfiguration vid fel.
Jag skapade AppConfig-klassen med egenskaper för antal parkeringsplatser, fordonstyper och kapacitetsinställningar. Konfigurationen lagras i JSON-format för enkel redigering.
 ReloadPriceList-metoden läser in den uppdaterade prislistan från fil och applicerar den omedelbart utan att man ska behöva starta om programmet. 
## Dag 7: Fordonshantering och validering
Valideringsprocessen för registreringsnummer utförs med hjälp av regex. Det finns olika regler per fordonstyp. Det finns en specifik funktion för att flytta på alla olika fordonstyper, förutom bussar. Bussar har en egen funktion för att flytta. GetParkingDuration-metoden skapar en läsbar sträng med den exakta tiden som fordonet har stått parkerad. Situations-specifika felmeddelanden uppstår när olika typer av fel inträffar, såsom till exempel fordon redan parkerat, ogiltigt registreringsnummer, otillräckligt med platser för bussar och generell brist på parkeringsplatser.
## Dag 8: MSTest
MSTest-tester för kärnfunktioner som parkering, avgiftsberäkning, bussparkering, validering och kartvisning utförs. Testar 10 minuters gratisperiod. Verifierar platsfrigöring och kapacitetshantering.
Alla tester är sparade i filen ParkingTests.cs 
Testerna består av: 
ParkCar_ShouldOccupyOneSpot som verifierar att en bil tar en plats, 
RemoveVehicle_ShouldFreeUpSpot som kontrollerar att platserna frigörs korrekt, 
CalculateParkingFee_First10MinutesFree som säkerställer att de första 10 minuterna är gratis, 
ParkBus_ShouldRequireFourConsecutiveSpots som verifierar att bussar kräver fyra 
sammanhängande platser och att programmet hittar tillgängliga platser. 
ParkingSpot_PartiallyFull_ShouldShowCorrectSymbol kontrollerar att delvis fulla platser visas med korrekt symbol (◐). 
CalculateParkingFee_OneHourShouldChargeFullRate ser till att timpriset är korrekt för varje fordon. 
FindVehicle_ShouldReturnCorrectSpot ser till att sökfunktionen för fordon fungerar korrekt.
ParkMotorcycle_CanShareSpotWithAnotherMotorcycle ser till att 2 motorcycklar kan dela på en på en parkeringsplats. 
Vehicle_ValidRegistrationNumber_ShouldPassValidation och Vehicle_InvalidRegistrationNumber_ShouldFailValidation ser till att validationsprocessen för giltiga registreringsnummer är korrekt. 
## Dag 9: Systemtest
Jag testade hela systemflödet: från själva parkeringen fram till utcheckningen. Verifierade fordonsdelning på platser. Genomförde tester kring parkering för bussar. Såg till att alla konfigurationsfiler fungerade.
## Dag 10: Avslutning
Jag förbättrade användargränssnittet med färgkodning. Kodgranskning och kravverifiering genomfördes. Jag såg till att alla krav var uppfyllda, därefter laddade jag upp projektet till github. 

