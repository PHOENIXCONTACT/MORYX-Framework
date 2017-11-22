---
uid: ModelSetupGuide
---
Model Setup Guide
=================

# Introduction
In many projects including Marvin it is common to trade database dumps like black market goods. Importing files into the database is another quite common request for all sorts of projects. Therefor one of the first features of the Runtime was to provide setups associated with a model that can either create database entries hard coded for a certain application or import data from a file by specifying a file name regex. The idea was to provide a common interface for all this tasks and a simple UI on the maintenance to make this process as easy as possible.

# Setup types

## Hard coded
The easiest setup is a hard coded one as it can be as simple as creating a single entity in the database. On the GUI this type is named "Model" and to be honest I am not really sure, why we called it that but people seem to be getting along with it. Hard coded setups are usually the best choice, when a concrete product or application must register static type information into a more generic platform or product model for example the Marvin.UserManagement model. Applications like GTIS register their concrete role based access model using a hard coded model. Since each model loads its setups at runtime it is a nice way to provide database initialization by deployment.

## File based
Another way to import data into the database is from a file. File based setups share the same API as hard coded once but differ by exporting a regex to filter valid files. On the maintenance they are displayed 0 to n times depending on the number of found files. If one of this is selected the server side setups is invoked with the full path to the selected file. The setup may than parse the file and import its content into the relational data model.

# Tutorials
So much for history and theory, show me the code already. For our tutorials we will use a rather simple reference model that contains a couple of entities and relations between them. This tutorial does not include model merge of any kind. I assume that if you understood model merge and model setups you will be capable of doing this yourself.

**Our sample model**
As a sample model we will use a small datamodel that could be used to manage a companies staff. It will have employee, address and relation data. Do not use it as a design reference as it is neither optimized nor normalized. It serves the plain purpose of demonstrating how to insert data into the database with a setup. The schematic model is shown below: 

![](images/StaffModel.png)

## Hard coded setup 
As a first example we will start by creating some key employees hard coded. It will create the boss as well as his assistant and an in-turn. To create a model setup project select the ModelSetupTemplate from the create project dialog. Next we follow the instructions placed in the comments to fill all the missing spots of the template. The most important of those is to declare the target model of this setup. Models are identified by their namespace which must not be copied but rather referenced from the generated ModelConstants.cs file. It is referenced as *ModelName*.Constants.Namespace or in our case StaffConstants.Namespace. Next you can optionally rename the class to anything more meaningful however the class name does not appear on any UI so don't waste too much time on the perfect class name.

### Infrastructure

First we have to take care of the attribute and read only properties required by the Runtime. They are necessary for the framework to find, match and map the setups correctly:

1. Sort order: In many cases you will have more than one setup for a model. If any of these setups depend on each other and must be executed in a certain order please assign the sort order accordingly. Otherwise just return 1.
2. Name: Short and hopefully unique name of this setup.
3. Description: Short description to indicate what this setup will do and wich data is created.
4. SupportedFileRegex: The regex for files we can import. Since this setup is hard coded leave it empty.

You should end up with something like this:

````cs
[ModelSetup(TargetModelNamespace = StaffConstants.Namespace)]
public class HardCodedSetup : IModelSetup
{
    public int SortOrder => 1;
    public string Name => "Hardcoded setup";
    public string Description => "This setup will create Thomas and Dennis";
    public string SupportedFileRegex => string.Empty;

    [...]
}
````

### Repositories
If you are familiar with the Runtime database API you may skip this section. Even though projects build on the Runtime use the EntityFramework as ORM it is wrapped in interfaces. The IUnitOfWork wraps the DbContext and transaction while IRepository implementations wrap the DbSets and provide most of the common database access and modification methods. It is usually good design to move all repository declarations to the top of the method. That way you do not mix operations with boiler plate and colleagues can spot all accessed tables easily. In our example we will use both repositories - those for Employee and Address.

### Entity creation
In our setup we will create a supervisor and two subordinates together with their address (which is obviously fictional). For entity creation we will use the generated Create-overload that accepts all non-nullable properties. This way we make sure to not forget any mandatory properties.

````cs
public void Execute(IUnitOfWork openContext, string setupData)
{
    var employeeRepo = openContext.GetRepository<IEmployeeRepository>();
    var addressRepo = openContext.GetRepository<IAddressRepository>();

    // Create the one and only boss
    var andreas = employeeRepo.Create("Andreas", "Schreiber", new DateTime(1980, 7, 10));
    andreas.Addresses.Add(addressRepo.Create("Bossway", 1, 12345, "NRW"));

    var thomas = employeeRepo.Create("Thomas", "Fuchs", new DateTime(1991, 4, 21));
    thomas.Addresses.Add(addressRepo.Create("Highway to perfection", 42, 42007, "NRW"));
    thomas.Supervisor = andreas;

    var dennis = employeeRepo.Create("Dennis", "Beuchler", new DateTime(1991, 10, 7));
    dennis.Addresses.Add(addressRepo.Create("Pretty drive", 7, 54321, "NRW");
    dennis.Supervisor = andreas;

    openContext.Save();
}
````

## File based setup 
In many cases we have to import from other system through some export formats or we create our own import file standard. In any case we have to read a files content and insert its information into the database. For our example we will use a very simple file format that does not require any complex file parsing. If you have to parse CSV or Excel files, please refer to the CsvHelper or EPPlus documentation.

### Infrastructure
The infrastructure properties of file based setups are the same except for the SupportedFileRegex. Here we must define a naming scheme to map files to this setup. In our example we will use "EmployeeCard_\w+\.txt" to match files like "EmployeeCard_Thomas.txt".

````cs
public string SupportedFileRegex => @"EmployeeCard_\w+\.txt";
````

### Entity creation
For our setup we will assume that each line contains one or more properties always in the same format. The following file format should be imported:

````
Thomas Fuchs
21.04.1991
Highway to perfection:42
12345 NRW
Andreas Schreiber
````

A setup parsing the file and creating the entity might look likes this:

````cs
public void Execute(IUnitOfWork openContext, string setupData)
{
    var employeeRepo = openContext.GetRepository<IEmployeeRepository>();
    var addressRepo = openContext.GetRepository<IAddressRepository>();

    var index = 0;
    var lines = File.ReadAllLines(setupData);

    // Line 1: Full name, Line 2: Birthday
    var name = lines[index++].Split(' ');
    var birthDay = DateTime.Parse(lines[index++]);

    // Line 3: Street and number
    var streetAndNumber = lines[index++].Split(':');
    var street = streetAndNumber[0];
    var number = int.Parse(streetAndNumber[1]);

    // Line 4: Zip code and state
    var zipAndState = lines[index++].Split(' ');
    var zip = int.Parse(zipAndState[0]);
    var state = zipAndState[1];

    // Line 5: Optional supervisor
    var supervisor = new[] { string.Empty, string.Empty };
    if (index < lines.Length)
        supervisor = lines[index].Split(' ');

    // Create entity
    var employee = employeeRepo.Create(name[0], name[1], birthDay);
    employee.Addresses.Add(addressRepo.Create(street, number, zip, state));
    employee.Supervisor = employeeRepo.GetMatch(supervisor[0], supervisor[1]);

    openContext.Save();
}
````