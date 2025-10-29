Prague-Parking-V2 is a prototype that resembles a parking lot management system written in C#. 

The purpose of Prague-Parking-V2 is to deliver a user friendly exprerience for customers when managing their vehicle. 

Features: 
* Park a vehicle
* Pick up a vehicle
* Move a vehicle form one parking spot to another
* Show parking map. This feature gives the user relevant status information of all the parking spots.
* View the current parking prices for different vehicle types. 

System requirmemts to run Prague-Parking-V2:
* .NET SDK 9.0 needs to be installed

Instructions to install and run Prague-Parking-V2:
$ git clone https://github.com/NilsDavid01/Prague-Parking-V2.git
$ cd Prague-Parking-V2/
$ dotnet build PragueParkingV2.sln --configuration Release --verbosity minimal
$ dotnet run --project src/MainApp/MainApp.csproj --configuration Release

