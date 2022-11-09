## Organization Structure Web API ##

### Description ###

Organization Structure Web API enables management of an organization hierarchy consisting of companies which have 0 to N divisions that which 0 to N projects which have 0 to N departments. Parallel to this structure exist employees. Every organization structure element has one employee which is the lead of the element. All of the mentioned entities are stored in a database which is accessible via REST API.

### Notes ###

* The project is runnable out of the box using the provided solution. Tested on Microsoft Windows [Version 10.0.19044.2130].
* Please provide a valid connection string in `appsettings.json` in order to be able to interact with the database and use this application.
* Please use migration to create the empty database: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli. A TSQL script to create the DB is included too: createOrganizationStructureDatabase.sql. It was generated using EF Core Tools and have not been tested. Use at your own risk.
* The solution contains also test but not in a runnable form as of October 20th 2022.
