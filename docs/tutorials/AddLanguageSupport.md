# How to add Language Support to a project

Language support based on needs can be added to two different layers, backend and front-end.

## Backend

In the **Startup.cs** file of the **StartProject.Asp** project, find the function **ConfigureServices(ISeviceCollection services)** and add the following line of code to its body

```csharp
services.AddLocalization();
```

Next, to the body of the function **Configure(IApplicationBuilder app, IWebHostEnvironment env)** just after *app.UserAuthorization();* add the following line that comes from Moryx.Launcher (install the package if it is not):

```csharp
services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
      new CultureInfo("de-DE"),
      new CultureInfo("en-US"),
      new CultureInfo("it-it"),
      new CultureInfo("zh-Hans"),
      new CultureInfo("pl-PL")
    };
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
```

Next, you need to move to the project that needs translation coming from the backend and add the *Properties* folder if it doesn't exist and under that folder, create new Resource file for each supported language conventionally named like the followings:

- Strings.resx
- Strings.de.resx
- Strings.it.resx

You can fill in the table with the strings needing translation.

**Important** - Each .Web project needs translations with the following values as they represent the module's name in the Demo or another running application. The values are:

- Module_Description
- Module_Title

You can see them being used under the *Pages* folder, the .cshtml file, in the following way:

```csharp
@attribute [Display(ResourceType = typeof(Strings), Name = nameof(Strings.Module_Title), Description = "Module_Description")]
```

**Important** - If you click on the arrow behind the file, you will see another file opening under it named as **Strings.Designer.cs**. After you update the values in Strings.resx, all the values in that file become internal making them inaccessible in html templates. You will need to replace all the internals with public to be able to use them.

## Front-end

In the root directory of the angular application where the angular.json file exists, run the following console commands:

- npm install @ngx-translate/core --save
- npm install @ngx-translate/http-loader --save

Afterwards, you need to open the file **app.config.ts** and add the following function before the `export const appConfig`. Importing the missing libraries for it should be easy:

```javascript
export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + 'assets/languages/'
  );
}
```

Next, you will need to have this piece of a code `TranslateModule.forRoot(....)` in the providers section of **app.config.ts** beside other imported modules separated by comma:

```javascript
{
  providers: [
        //other imports
        importProvidersFrom(
          TranslateModule.forRoot({
                loader: {
                  provide: TranslateLoader,
                  useFactory: httpTranslateLoaderFactory,
                  deps: [HttpClient],
                },
              }),
               //other imports
            ),
     //other imports
  ]
}
```

Now, let's initialize your TranslationConstants class that holds your translation strings. Create a class called **translation-constants.extensions.ts** under the extensions folder of the project and then add the following body to it for the supported languages:

```javascript
export class TranslationConstants {
  public static readonly LANGUAGES = {
    EN: 'en',
    DE: 'de',
    IT: 'it',
  };
}
```

Now, you have the necessary modules and libraries added. You will need to use them! In the **app.component.ts**, inject two services:

- TranslateService (*coming from the library you installed earlier*)
- LanguageService (*coming from moryx-web. If you don't have it, refer to the Readme of moryx-web*)

Inject them in the constructor simply like:

- private languageSupport: LanguageSupport
- public translate: TranslateService (*translate should be public to be used in html template*)

And then have the following piece of code in the body of constructor of **app.component.ts**

```javascript
this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang(this.languageService.getDefaultLanguage());
```

Besides, wherever you have defined the variables for this module, add the following variable in the following way. You should be able to import it as you created the class earlier. Notice the equal sign instead of colons:

```javascript
TranslationConstants = TranslationConstants;
```

Next stop, the translations! You need to create translations for each string you want to translate using json files. Go to the folder *assets* and then create a folder named *languages* and then create json files for each supported language like:

- de.json
- en.json
- it.json

Next, fill in the strings that need to be translated for each component within each json file. You may look at the existing files in other projects to have a better understanding.

Next, you will need to have references to the created strings from the last step in the TranslationConstants class. Please have a look at an existing project to see the structure.

### How to use

You can either use them in the html templates or the ts files.

- Html template: you will need to use the translate pipe which you earlier injected. As an example:

```javascript
{{ TranslationConstants.APP.APPLY_FILTER | translate }}
```

- Ts file: First you need to have the observable extensions that has the method of .toAsync(). That is a method internally developed. You can then either make a function with the translations you want at the top if there are several translations like:

```javascript
async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate.get([
        TranslationConstants.APP.SNACK_BAR
        TranslationConstants.APP.SUCCESS])
    .toAsync();
  }
```

and then use each of those translations seperately in the following way:

```javascript
const translations = await this.getTranslations();
this.snackBar.open(translations[TranslationConstants.APP.SNACK_BAR]);
```

Or, if it is just one translation, you can mix them both together in the following way:

```javascript
const translations = await this.translate.get([
    TranslationConstants.APP.SNACK_BAR]).toAsync();
this.snackBar.open(translations[TranslationConstants.APP.SNACK_BAR]);
```
