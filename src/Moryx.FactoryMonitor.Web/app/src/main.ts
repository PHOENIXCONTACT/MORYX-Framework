/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { bootstrapApplication } from '@angular/platform-browser';
import { App } from '@app/app';
import { appConfig } from '@app/app.config';

bootstrapApplication(App, appConfig)
  .catch(err => {
    console.error(err)
    // Render a simple error instead of a blank page when loading the factory fails
    document.body.innerHTML = `
      <div style="
        height: 100vh;
        display: flex;
        align-items: center;
        justify-content: center;
        margin: 0;
        padding: 24px;
        box-sizing: border-box;
        font-family: Roboto, Arial, sans-serif;
        color: #333;
        text-align: center;
      ">
        <div style="
          display: flex;
          flex-direction: column;
          align-items: center;
          gap: 12px;
        ">
          <span class="material-symbols-outlined" style="font-size: 48px; color: #666;">
            refresh
          </span>

          <div style="font-size: 18px; line-height: 1.5;">
            Could not load application data.<br>
            Please
            <a href="" onclick="window.location.reload(); return false;">reload</a>
            the page or contact your administrator.
          </div>
        </div>
      </div>
    `;
});
