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
  private readonly _authConfigured = signal<boolean>(false);
  readonly authConfigured = this._authConfigured.asReadonly();

  /** Whether the current user is signed in. */
  private readonly _isLoggedIn = signal<boolean>(false);
  readonly isLoggedIn = this._isLoggedIn.asReadonly();

  /** Display name of the signed-in user. */
  private readonly _userName = signal<string>('');
  readonly userName = this._userName.asReadonly();

  /** Called by the app root to indicate whether the server has authentication enabled. */
  setAuthConfigured(value: boolean): void {
    this._authConfigured.set(value);
  }

  checkSignedIn(): void {
    const user = this.cookieService.get('moryx_user');
    if (!user) {
      return;
    }
    this._isLoggedIn.set(true);
    this._userName.set(user);
  }

  signOut(): void {
    firstValueFrom(this.http.post(this.authBaseAddress + '/api/auth/signOut', {}, {
      withCredentials: true,
    })).then(() => {
      this._isLoggedIn.set(false);
      this._userName.set('');
      window.location.assign('/');
    }).catch((err) => console.log(err));
  }
}
