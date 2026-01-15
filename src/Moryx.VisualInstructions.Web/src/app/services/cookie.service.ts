/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CookieService {

  constructor() { }

  getCookie(name: string) {
    let ca: Array<string> = document.cookie.split(';');
    let caLen: number = ca.length;
    let cookieName = `${name}=`;
    let c: string;

    for (let i: number = 0; i < caLen; i += 1) {
      c = ca[i].replace(/^\s+/g, '');
      if (c.indexOf(cookieName) == 0) {
        return decodeURI(c.substring(cookieName.length, c.length));
      }
    }
    return undefined;
  }

  setCookie(name: string, value: string, expireDays: number, path: string = '') {
    let d: Date = new Date();
    d.setTime(d.getTime() + expireDays * 24 * 60 * 60 * 1000);
    let expires: string = `expires=${d.toUTCString()}`;
    let cpath: string = path ? `; path=${path}` : '; path=/';
    if (environment.production)
      document.cookie = `${name}=${encodeURI(value)}; ${expires}${cpath}`;
    else
      document.cookie = `${name}=${encodeURI(value)}; ${expires}${cpath}; samesite=none; secure`;
  }
}

