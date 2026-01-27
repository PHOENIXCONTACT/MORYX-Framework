/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
  selector: 'app-dropdown-item',
  imports: [],
  templateUrl: './dropdown-item.html',
  styleUrl: './dropdown-item.css'
})
export class DropdownItem {

  @Input() closeOnClick: boolean = true;
  @Output() onClick = new EventEmitter<DropdownItemClickEventArg>();

  @HostListener('click', ['$event'])
  handleClick(event: Event) {
    this.onClick.emit(<DropdownItemClickEventArg>{closeOnClick: this.closeOnClick});
  }

}

export interface DropdownItemClickEventArg {
  closeOnClick: boolean;
}
