# Prague-Parking-V2 is an application that resembles a parking lot management system written in C#. 

The purpose of Prague-Parking-V2 is to deliver a user friendly experience for customers when managing their vehicle inside of a parking lot. 

# Features: 
* Park a vehicle
* Pick up a vehicle
* Move a vehicle form one parking spot to another
* Show parking map. This feature gives the user relevant status information of all the parking spots.
* View the current parking prices for different vehicle types.
* Reload price list config file
* Save praking lot data 

# System requirments to run Prague-Parking-V2:
- .NET SDK 9.0 

# Instructions to install and run Prague-Parking-V2:
## 1. Clone the repository
```bash
git clone https://github.com/NilsDavid01/Prague-Parking-V2.git
```
## 2. Navigate to the project directory
```bash
cd Prague-Parking-V2/
```
## 3. Build the application
```bash
dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal
```
## 4. Run the application
```bash
dotnet run --project src/MainApp/MainApp.csproj --configuration Release
```
# Download and install Prague-Parking-V2 the fast way by using only one Windows Powershell command:
```bash
git clone https://github.com/NilsDavid01/Prague-Parking-V2.git; cd Prague-Parking-V2; dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal; dotnet run --project src/MainApp/MainApp.csproj --configuration Release
```

# Download and install Prague-Parking-V2 the fast way by using only one macOS/Linux/Unix bash shell command:
```bash
git clone https://github.com/NilsDavid01/Prague-Parking-V2.git && cd Prague-Parking-V2/ && dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal && dotnet run --project src/MainApp/MainApp.csproj --configuration Release
```
# Command to run all of the MSTest testing tasks:
```bash
dotnet test tests/UnitTests/UnitTests.csproj --verbosity normal
```



