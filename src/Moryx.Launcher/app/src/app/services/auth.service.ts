/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
/** Tracks authentication state and provides sign-out functionality. */
export class AuthService {
  private cookieService = inject(CookieService);

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

  async signOut(): Promise<void> {
    await fetch(this.authBaseAddress + '/api/auth/signOut', {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
    })
      .then(() => {
        this.isLoggedIn.set(false);
        this.userName.set('');
        window.location.assign('/');
      })
      .catch((err) => console.log(err));
  }
}
