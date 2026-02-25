/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-dropdown-item',
  imports: [],
  templateUrl: './dropdown-item.html',
  styleUrl: './dropdown-item.css',
  host: {
    '(click)': 'handleClick($event)'
  }
})
export class DropdownItem {
  closeOnClick = input(true);
  onClick = output<DropdownItemClickEventArg>();

  handleClick(event: Event) {
    this.onClick.emit(<DropdownItemClickEventArg>{closeOnClick: this.closeOnClick()});
  }
}

export interface DropdownItemClickEventArg {
  closeOnClick: boolean;
}
