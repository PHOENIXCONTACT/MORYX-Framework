/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-auth-button',
  standalone: true,
  imports: [],
  templateUrl: './auth-button.component.html',
  styleUrl: './auth-button.component.css',
})
export class SignInButtonComponent implements OnInit {
  @Input() authurl: string | undefined;
  isLoggedIn = false;
  userName = '';

  ngOnInit(): void {
    this.checkSignedIn();
  }

  signIn() {
    window.location.assign(`${this.authurl}/login?returnUrl=${location.href}`);
  }

  async signOut() {
    await fetch(this.authurl + '/api/auth/signOut', {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
    })
      .then(() => {
        this.isLoggedIn = false;
        this.userName = '';
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
    this.isLoggedIn = true;
    this.userName = user;
  }
}

