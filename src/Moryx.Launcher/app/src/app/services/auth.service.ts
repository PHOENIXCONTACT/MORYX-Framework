/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
/** Tracks authentication state and provides sign-out functionality. */
export class AuthService {
  private cookieService = inject(CookieService);
  private http = inject(HttpClient);

  /** Set by the server in MoryxIdentityDefaults. */
  private readonly cookieName = 'moryx_user';

  private readonly _authBaseAddress = signal<string | undefined>(undefined);
  readonly authBaseAddress = this._authBaseAddress.asReadonly();

  /** Whether the server has authentication enabled. */
  readonly authConfigured = computed(() => !!this._authBaseAddress());

  /** Whether the current user is signed in. */
  private readonly _isLoggedIn = signal<boolean>(false);
  readonly isLoggedIn = this._isLoggedIn.asReadonly();

  /** Display name of the signed-in user. */
  private readonly _userName = signal<string>('');
  readonly userName = this._userName.asReadonly();

  /** Called by the app root to configure the authentication base address. */
  setAuthBaseAddress(url: string | undefined): void {
    this._authBaseAddress.set(url);
  }

  checkSignedIn(): void {
    const user = this.cookieService.get(this.cookieName);
    if (!user) {
      return;
    }
    this._isLoggedIn.set(true);
    this._userName.set(user);
  }

  signOut(): void {
    firstValueFrom(this.http.post(this._authBaseAddress() + '/api/auth/signOut', {}, {
      withCredentials: true,
    })).then(() => {
      this._isLoggedIn.set(false);
      this._userName.set('');
      window.location.assign('/');
    }).catch((err) => console.log(err));
  }
}
