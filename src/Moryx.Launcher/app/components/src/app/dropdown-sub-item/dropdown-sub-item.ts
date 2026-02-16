/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, output } from '@angular/core';
import { DropdownItem } from "../dropdown-item/dropdown-item";
import { DropdownContainer } from "../dropdown-container/dropdown-container";
import { DropdownMenu } from "../dropdown-menu/dropdown-menu";

@Component({
  selector: 'app-dropdown-sub-item',
  imports: [DropdownMenu],
  templateUrl: './dropdown-sub-item.html',
  styleUrl: './dropdown-sub-item.css'
})
export class DropdownSubItem {
  closeOnClick = input(false);
  onClick = output<boolean>();
  class = input('');

  handleClick() {
    this.onClick.emit(this.closeOnClick());
  }
}

