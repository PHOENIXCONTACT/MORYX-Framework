/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { AfterContentInit, AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { localLanguage } from '../utils';

@Component({
  selector: 'app-language-selector',
  imports: [],
  templateUrl: './language-selector.html',
  styleUrl: './language-selector.css'
})
export class LanguageSelector {
  @Input() language: string = "de-DE";
  @Input() class: string = "";

  currentLanguage() {
    return localLanguage();
  }

  isChecked() {
    return this.currentLanguage() === this.language;
  }

  onClick() {
    let CookieDate = new Date;
    CookieDate.setFullYear(CookieDate.getFullYear() + 1);
    const cookieString = `c=${this.language}|uic=${this.language}`;
    const encodedCookie = escape(cookieString);
    document.cookie =
      '.AspNetCore.Culture=' + encodedCookie +
      '; expires=' + CookieDate.toUTCString() +
      '; path=/;';
    location.reload();
  }
}

