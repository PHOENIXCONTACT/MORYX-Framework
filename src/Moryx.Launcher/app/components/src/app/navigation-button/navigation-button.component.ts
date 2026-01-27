/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-navigation-button',
  imports: [],
  templateUrl: './navigation-button.component.html',
  styleUrl: './navigation-button.component.css'
})
export class NavigationButtonComponent {
  public static POSITION_ATTRIBUTE = "position";
  @Input() position: NavigationButtonPosition = "default";
}

export type NavigationButtonPosition = "fixed" | "default";
