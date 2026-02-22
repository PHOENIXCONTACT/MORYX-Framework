# Security Guidlines for configuring MORYX-Media

The media module is a special concern when it comes to application security.
As it allows users to upload files of their own, you need to make sure that these files do not pose a security risk to your application.
For that the Media module has built-in validation mechanisms to ensure integrity of the uploaded files to a certain degree.
However, this is not at all a bullet prove security mechanism without adequate configurations on the application's system.

## Select a seperate directory tree for the Application and the stored files

In the [ModuleConfig](https://github.com/PHOENIXCONTACT/MORYX-Framework/blob/future/src/Moryx.Media.Server/ModuleController/ModuleConfig.cs) of the Media Module you are able to configure the location for all of the uploaded files to be stored in.
By default the path is configured to a directory beside your application's working directory.
If you need to change this configuration make sure to keep the two folders separated.

## Remove executive privilages from the directory

After you have configured the location the Media module is storing its files in, make sure to remove the execution privelages from the directory.
Instructions on how to do this for different operating systems are available on the web.
