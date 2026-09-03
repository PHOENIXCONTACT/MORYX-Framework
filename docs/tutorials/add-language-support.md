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

### 1. Install dependencies

In the `app/` directory of the Angular project, install the translation packages:

```bash
npm install @ngx-translate/core --save
npm install @ngx-translate/http-loader --save
```

### 2. Create the TranslationConstants class

Create a file called **translation-constants.ts** under `src/app/` with the supported languages and your translation keys:

```typescript
export class TranslationConstants {
  public static readonly LANGUAGES = ['en', 'de', 'it', 'zh'];

  public static readonly APP = {
    TITLE: 'APP.TITLE',
    // add more keys as needed
  };
}
```

### 3. Register Angular locale data

In **app.config.ts**, import the [global locale data](https://angular.dev/guide/i18n/import-global-variants) for each non-English language at the top of the file:

```typescript
import "@angular/common/locales/global/de";
import "@angular/common/locales/global/it";
import "@angular/common/locales/global/zh";
```

### 4. Configure app.config.ts

Set up the providers in **app.config.ts** using `provideMoryxLocalization` from `@moryx/ngx-web-framework/i18n`. This single provider handles Angular locale switching, ngx-translate language registration, and fallback language configuration — no manual setup in `app.ts` is needed.

```typescript
export const appConfig: ApplicationConfig = {
  providers: [
    // ...

    // Configure translation loader
    provideTranslateService({
      loader: provideTranslateHttpLoader({
        prefix: environment.assets + 'assets/languages/',
        suffix: '.json'
      }),
    }),

    // Provides Angular locale and configures ngx-translate
    provideMoryxLocalization(TranslationConstants.LANGUAGES)
  ],
};
```

### 5. Create translation files

Under `src/assets/languages/`, create a JSON file for each supported language:

- `en.json`
- `de.json`
- `it.json`
- `zh.json`

Fill in the translation keys matching the structure in your `TranslationConstants`. For example, `en.json`:

```json
{
  "APP": {
    "TITLE": "My Module"
  }
}
```

Add the corresponding keys to `TranslationConstants` so they can be referenced type-safely in code.

### How to use

You can use translations in both HTML templates and TypeScript files.

**HTML template** — use the `translate` pipe:

```html
{{ TranslationConstants.APP.TITLE | translate }}
```

**TypeScript file** — use `firstValueFrom` with `TranslateService.get()`:

```typescript
import { firstValueFrom } from 'rxjs';

const translations = await firstValueFrom(this.translateService.get([
  TranslationConstants.APP.SNACK_BAR,
  TranslationConstants.APP.SUCCESS
]));
this.snackBar.open(translations[TranslationConstants.APP.SNACK_BAR]);
```
