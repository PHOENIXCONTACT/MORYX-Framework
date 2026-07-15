/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
/** Tracks authentication state and provides sign-out functionality. */
export class AuthService {
  private cookieService = inject(CookieService);
  private http = inject(HttpClient);

  authBaseAddress: string | undefined = undefined;

  /** Whether the server has authentication enabled. */
  authConfigured = signal<boolean>(false);

  /** Whether the current user is signed in. */
  isLoggedIn = signal<boolean>(false);

  /** Display name of the signed-in user. */
  userName = signal<string>('');

  checkSignedIn(): void {
    const user = this.cookieService.get('moryx_user');
    if (!user) {
      return;
    }
    this.isLoggedIn.set(true);
    this.userName.set(user);
  }

  signOut(): void {
    firstValueFrom(this.http.post(this.authBaseAddress + '/api/auth/signOut', {}, {
      withCredentials: true,
    })).then(() => {
      this.isLoggedIn.set(false);
      this.userName.set('');
      window.location.assign('/');
    }).catch((err) => console.log(err));
  }
}
