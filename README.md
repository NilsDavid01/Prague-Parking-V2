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

# System requirmemts to run Prague-Parking-V2:
- .NET SDK 9.0 

# Instructions to install and run Prague-Parking-V2:
# Clone the repository
$ git clone https://github.com/NilsDavid01/Prague-Parking-V2.git

# Navigate to the project directory
$ cd Prague-Parking-V2/

# Build the application
$ dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal

# Run the application
$ dotnet run --project src/MainApp/MainApp.csproj --configuration Release

# Download and install Prague-Parking-V2 the fast way by using only one Windows Powershell command:
git clone https://github.com/NilsDavid01/Prague-Parking-V2.git; cd Prague-Parking-V2; dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal; dotnet run --project src/MainApp/MainApp.csproj --configuration Release

# Download and install Prague-Parking-V2 the fast way by using only one Linux bash shell command:
$ git clone https://github.com/NilsDavid01/Prague-Parking-V2.git && cd Prague-Parking-V2/ && dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal && dotnet run --project src/MainApp/MainApp.csproj --configuration Release

