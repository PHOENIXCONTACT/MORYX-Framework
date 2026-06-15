/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {Injectable, signal} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  authBaseAddress: string | undefined = undefined;
  authConfigured = signal<boolean>(false);
  isLoggedIn = signal<boolean>(false);
  userName = signal<string>('');

  checkSignedIn(): void {
    const cookies = document.cookie.split(';').map((c) => c.trim());
    const userCookie = cookies.filter((c) => c.includes('moryx_user'));
    if (!userCookie.length) {
      return;
    }

    const equalSignIndex = userCookie[0].indexOf('=');
    const user = userCookie[0].substring(equalSignIndex + 1);
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
