/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-auth-button',
  imports: [MatButton, MatIconModule],
  templateUrl: './auth-button.html',
  styleUrl: './auth-button.scss',
})
export class AuthButton implements OnInit {
  private authService = inject(AuthService);

  isLoggedIn = this.authService.isLoggedIn;
  userName = this.authService.userName;

  ngOnInit(): void {
    this.authService.checkSignedIn();
  }

  signIn() {
    window.location.assign(`${this.authService.authBaseAddress}/login?returnUrl=${location.href}`);
  }

  async signOut() {
    return this.authService.signOut();
  }
}
