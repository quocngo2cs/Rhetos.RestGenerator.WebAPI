# Rhetos.RestGenerator.WebAPI
An ASP.NET WebAPI REST generator for Rhetos.
##### Step 1: run "nuget restore -configFile NuGet.Config Rhetos.RestGenerator.sln inside Rhetos.RestGenerator folder.
##### Step 2: rebuild solution with Release mode.
##### Step 3: run CreatePackage.bat then copy two packages: Rhetos.WebApiRestGenerator.{version-number}.nupkg and Autofac.Integration.WebApi.4.0.1.nupkg inside LocalPackageSource into "Install" folder of Rhetos project.
##### Step 4: add those lines to RhetosPackages.config in Rhetos project
          <package id="Rhetos.WebApiRestGenerator" source="..\..\Install" />
	        <package id="Autofac.Integration.WebApi" source = "..\..\Install" />
##### Step 5: run SetupRhetosServer.bat in Rhetos project
