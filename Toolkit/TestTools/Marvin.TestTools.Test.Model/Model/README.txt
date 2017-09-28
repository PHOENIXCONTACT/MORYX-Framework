Congratulations!

You managed to create the model "TestModel" using one of Thomas self written developer tools. To get started
with your own database open any file explorer and navigate to the model directory referenced by this project.
There you will find a file named "TestModel.edml". Open it with the devArt EntityDeveloper. You can now start
creating classes and connecting them with associations.

The output settings will allready be configured and all generated classes will be in the Marvin.Test.Model
namespace independent of the folder the are in.

If you chose ModelInheritance (aka ModelMerge) you can declare Inheritance on classes by using the extended
property "InheritedClass" provided by the MarvinInheritance.tmpl template. Make sure that inherited classes set 
their Id property on override and the StoreGeneratedPattern to None. You also have to specify the absolute path to 
the output directory of Marvin.Base.Model.dll. This directory must also contain all dependencies of the library.  

The files "TestModel.edml", "TestModel.edps" and "TestModel.Diagram1.view" can be excluded from the project. They
were only included because Visual Studio refuses to copy files within a project template if they are not part of
the project itself.

Have fun and all the best
Thomas