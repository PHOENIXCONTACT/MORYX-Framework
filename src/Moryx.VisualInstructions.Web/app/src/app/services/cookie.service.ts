/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CookieService {

  getCookie(name: string) {
    const ca: Array<string> = document.cookie.split(';');
    const caLen: number = ca.length;
    const cookieName = `${name}=`;
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
    const d: Date = new Date();
    d.setTime(d.getTime() + expireDays * 24 * 60 * 60 * 1000);
    const expires: string = `expires=${d.toUTCString()}`;
    const cpath: string = path ? `; path=${path}` : '; path=/';
    if (environment.production) {
      document.cookie = `${name}=${encodeURI(value)}; ${expires}${cpath}`;
    } else {
      document.cookie = `${name}=${encodeURI(value)}; ${expires}${cpath}; samesite=none; secure`;
    }
  }
}

