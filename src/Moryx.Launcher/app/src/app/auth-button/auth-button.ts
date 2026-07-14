/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../services/auth.service';
import { TranslationConstants } from '../translation-constants';

@Component({
  selector: 'app-auth-button',
  imports: [MatButton, MatIconModule, TranslatePipe],
  templateUrl: './auth-button.html',
  styleUrl: './auth-button.scss',
})
export class AuthButton implements OnInit {
  private authService = inject(AuthService);

  protected isLoggedIn = this.authService.isLoggedIn;
  protected userName = this.authService.userName;

  protected TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.authService.checkSignedIn();
  }

  protected signIn() {
    window.location.assign(`${this.authService.authBaseAddress()}/login?returnUrl=${location.href}`);
  }

  protected async signOut() {
    return this.authService.signOut();
  }
}
