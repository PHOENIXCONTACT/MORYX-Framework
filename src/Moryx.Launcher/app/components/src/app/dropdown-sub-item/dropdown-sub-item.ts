/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, EventEmitter, Input, Output } from '@angular/core';
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
  @Input() closeOnClick: boolean = false;
  @Output() onClick = new EventEmitter<boolean>();
  @Input() class: string = "";


  handleClick() {
    this.onClick.emit(this.closeOnClick);
  }
}

