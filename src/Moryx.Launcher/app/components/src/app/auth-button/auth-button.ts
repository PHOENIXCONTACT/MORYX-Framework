/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-auth-button',
  imports: [],
  templateUrl: './auth-button.html',
  styleUrl: './auth-button.css'
})
export class SignInButton implements OnInit {
  authurl = input<string | undefined>(undefined);
  isLoggedIn = signal<boolean>(false);
  userName = signal<string>('');

  ngOnInit(): void {
    this.checkSignedIn();
  }

  signIn() {
    window.location.assign(`${this.authurl()}/login?returnUrl=${location.href}`);
  }

  async signOut() {
    await fetch(this.authurl() + '/api/auth/signOut', {
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

  checkSignedIn() {
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
}

