/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input } from '@angular/core';

@Component({
  selector: 'app-navigation-button',
  imports: [],
  templateUrl: './navigation-button.html',
  styleUrl: './navigation-button.css'
})
export class NavigationButton {
  public static POSITION_ATTRIBUTE = "position";
  position = input<NavigationButtonPosition>('default');
}

export type NavigationButtonPosition = "fixed" | "default";
