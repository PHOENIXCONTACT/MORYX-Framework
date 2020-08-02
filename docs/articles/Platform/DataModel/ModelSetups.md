---
uid: Model.ModelSetups
---
# Model Setup Guide

In many projects including MORYX it is common to trade database dumps like black market goods. Importing files into the database is another quite common request for all sorts of projects. Therefor one of the first features of the Runtime was to provide setups associated with a model that can either create database entries hard coded for a certain application or import data from a file by specifying a file name regex. The idea was to provide a common interface for all this tasks and a simple UI on the maintenance to make this process as easy as possible.

## Setup types

**Hard coded**
The easiest setup is a hard coded one as it can be as simple as creating a single entity in the database. On the GUI this type is named "Model" and to be honest I am not really sure, why we called it that but people seem to be getting along with it. Hard coded setups are usually the best choice, when a concrete product or application must register static type information into a more generic platform or product model for example the Moryx.UserManagement model. Applications like GTIS register their concrete role based access model using a hard coded model. Since each model loads its setups at runtime it is a nice way to provide database initialization by deployment.

**File based**
Another way to import data into the database is from a file. File based setups share the same API as hard coded once but differ by exporting a regex to filter valid files. On the maintenance they are displayed 0 to n times depending on the number of found files. If one of this is selected the server side setups is invoked with the full path to the selected file. The setup may than parse the file and import its content into the relational data model.