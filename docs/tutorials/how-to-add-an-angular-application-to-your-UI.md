# How to add an Angular application to your UI

## Create an Angular app

Create a new Angular app in the `./app` directory.
If you don't have the the Angular CLI installed yet, follow [these instructions](https://v17.angular.io/cli).

```bash
ng new my-module
```

> If you don't have preferences yourself, choose to use scss and don't select SSR when prompted

You can develop the app as if it was a standalone solution. Just make sure that the content of your app adjusts dynamically to the available space in the browser, i.e. set width and height of your top level element to 100%.
> For more information on how to build an angular app, start with [this tutorial](https://angular.dev/tutorials/first-app).

## Configure the build output of your Angular app

To build the angular app automatically when you build your C# project, you need to adjust some configurations in the **angular.json** file.

1. Change the output path from `dist/` to `../wwwroot/`
(That's the directory that is packed into nuget packages as static files by default)
2. Turn off the output hashing (by setting it to `none`)
(Otherwise you will have to change the references in the razor page after every build)
3. Turn off the inline optimization of fonts
(only required if you build the project behind a proxy)

Now, start building your app.
You can test it using `ng serve` and whenever you want to see your current status in the MORYX Launcher run `npm run build` and continue with the next section.

## Reference the Angular build output in your Razor page

Coming back to the Razor page you created in [How to create your UI](how-to-create-your-UI.md) the following lines need to be added to the *MyModule.cshtml*.
It adds a styles and scripts section to include the angular build outputs and also the application
root to anchor the angular app into the razor page.

```cshtml
@* // Add the styles from the angular project *@
@section Styles {
  <link rel="stylesheet" href="/_content/<YourC#ProjectName>/styles.css">
}

@* // Add the compiled javascript from the angular project *@
@section Scripts
{
  <script src="/_content/<YourC#ProjectName>/polyfills.js" type="module"></script>
  <script src="/_content/<YourC#ProjectName>/main.js" type="module"></script>
}

@* // Add the application root *@
<app-root></app-root>
```

When you start your MORYX application now, you will see the angular app placed within your razor page.

## (Optional) Attach building the Angular app to your msbuild process

To build the angular app automatically when you build your C# project, you need to add the following snippet to your `.csproj` file

```xml
<!-- MyModule.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Razor" InitialTargets="BuildUI">

  <PropertyGroup>
    <!-- Helpful if visual studio complains about anything -->
    <DefaultItemExcludes>app\**</DefaultItemExcludes>
  </PropertyGroup>
  <ItemGroup>
    <Content Update="@(Content)" Pack="false" />
    <Content Update="wwwroot\**" Pack="true" PackagePath="staticwebassets" />
  </ItemGroup>
  <!-- Automatically build the angular app, if it wasn't build before or you are running a release build -->
  <Target Name="BuildUI" Condition="('$(Configuration)'!='Debug' Or !Exists('wwwroot\main.js')) And ('$(NoBuild)' != 'true') ">
    <Exec WorkingDirectory="./app/" Command="npm ci" />
    <Exec WorkingDirectory="./app/" Command="npm run build:prod" />
  </Target>
</Project>
```

> Note: The additional **InitialTargets** in the project element right at the top of your .csprof file

These commands will install the necessary dependencies of your app and take care of building.
The condition makes sure that the build only happens if you haven't build the UI yet or you are running a Release build.

## (Optional) Establish a folder structure for more complex projects

The MORYX team agreed to use the following folder structure in all their Angular applications, feel invited to do so yourself.

```text
app
└───api                      // usually auto generated, for backend communication
|   └───services             // services related to backend communication
└───components
|   └───component1
|   |   └───nestedComponent
|   └───component2
|   |  ...
|
└───dialogs
└───extensions
└───models                   // for UI related models
└───services                 // for UI related services
```

## (Optional) Add angular Material as a component library

Follow the steps depicted [here](https://material.angular.io/guide/getting-started).

> Note: If you also use the ngx-web-framework and want to use the theme it contains, select to use a custom theme when prompted.

## (Optional) Make use of the APIs of your MORYX application

If you have an OpenAPI service reference (as provided by swagger) you may use the [ng-openapi-gen](https://www.npmjs.com/package/ng-openapi-gen) npm package to automatically generate the angular service instead of implementing it manually as shown in the Angular tutorial.

### Installation

First, install `ng-openapi-gen`

```bash
npm i -g ng-openapi-gen
```

### Generate the API services

If you intend to use one of the endpoints of your MORYX application save its service reference from [https://localhost:5000/swagger/v1/swagger.json](https://localhost:5000/swagger/v1/swagger.json) to the `MyModule` directory.
Next to the *swagger.json* create an `ng-openapi-gen.json` file with the following content

```json
{
    "$schema": "node_modules/ng-openapi-gen/ng-openapi-gen-schema.json",
    "input": "swagger.json",
    "output": "src/app/api",
    "ignoreUnusedModels": true,
    "includeTags": ["OrderManagement", "ProductManagement"],
    "defaultTag": "OrderManagement"
}
```

> Replace the input and included tags to match your requirements
After running the tool by executing `ng-openapi-gen`, you see the generated code in your angular app.

### Reference the generated ApiModule in the app

To use it in your components, you first need to provide the ApiModule to the angular app.
Therefore, add the following two providers in your *app.config.ts* file

```ts
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApiModule } from './api/api.module';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptorsFromDi()),
    importProvidersFrom(ApiModule.forRoot({rootUrl: environment.rootUrl }))
  ]
};
```

At this point, the app will complain that there is no environment file.

### Adjust the rootUrl based on the apps environment

To resolve the issue, you run the following command in the projects directory

```bash
ng generate environments
```

This creates two files *environment.ts* and *environment.development.ts*.
Add the following lines to the respective files

```ts
// environment.ts
export const environment = {
  rootUrl: '',
};
```

```ts
// environment.development.ts
export const environment = {
  rootUrl: 'https://localhost:5000',
};
```

and insert the following part into your *angular.json* file

```json
// angular.json - {"projects":"my-module":"architect":"build":"configurations":"development":{...}}
"development": {
  "fileReplacements": [
    {
      "replace": "src/environments/environment.ts",
      "with": "src/environments/environment.development.ts"
    }
  ]
}
```

### Call the API in your component

Now, get back to the component or service where you want the data to be retrieved.
Here is an example to call a methodnin the *app.component.ts*

```ts
export class AppComponent implements OnInit {
  constructor(private api: MyFacadeService) {
  }

  ngOnInit(): void {
    this.api.accessFacade$Json().subscribe(r => this.example = r)
  }
}
```

## (Optional) Configure asset paths in your Angular app to work in the Launcher

If you haven't done this for the last section, create two file in `src/environments/environment.ts` and `src/environments/environment.prod.ts`.
Here, you need to add a variable to change the path to your assets depending on whether you develop using `ng serve` or are running the app in the Launcher.
The following snippets provide an example of the new content of the files

```ts
// environment.ts
export const environment = {
  assets: "/",
};
```

```ts
// environment.prod.ts
export const environment = {
  // This is the path under which Razor pages place assets of referenced class libraries. Just replace the placeholder with the actual name of your project.
  assets: "/_content/<YourC#ProjectName>/",
};
```

* `assets` defines the path, in which Razor pages place assets of referenced class libraries. Replace the placeholder by your actual projects name
To access these environment variables in your code, refer to sources on the web like [this one](https://angular.io/guide/build).
For your assets, you then have to prefix the filenames. So, that your code would for example look like this:

```ts
  myImage = environment.assets + "assets/my-image.jpg"
```

Lastly, you need to tell Angular to switch the environment when you build for production.
For that add the following to your *angular.json* file (if you haven't done this for the last paragraph already)

```json
// angular.json - {"projects":"my-module":"architect":"build":"configurations":"development":{...}}
"development": {
  "fileReplacements": [
    {
      "replace": "src/environments/environment.ts",
      "with": "src/environments/environment.development.ts"
    }
  ]
}
```

## (Optional) Install ngx-web-framework package for MORYX styles and components

First you need to add an additional npm package source to your project.
For that, add an `.npmrc` in your *MyModule* directory, if it doens't exist yet.
Fill in the following content

```npmrc
registry=https://registry.npmjs.org/
@moryx:registry=https://www.myget.org/F/moryx-ci/auth/4cd70c3b-c8e8-4186-8c35-3d4c8789bbdd/npm/
```

Now run `npm i @moryx:ngx-web-framework`, which installs the package. If you want to use the MORYX theme settings, add a reference in your `angular.json` file

```json
// angular.json - {"projects":"my-module":"architect":"build":{...}}
"build": {
  "options": {
    "styles": [
      "node_modules/@moryx/ngx-web-framework/styles/moryx-theme.scss",
      "src/styles.scss"
    ],
  },
},
```

For further information and available components check out the [repository](https://github.com/MORYX-Industry/moryx-web).
