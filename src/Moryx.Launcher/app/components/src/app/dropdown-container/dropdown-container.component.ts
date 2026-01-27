/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { AfterContentInit, Component, ElementRef, Input, OnDestroy } from '@angular/core';
import { Constants } from '../constants';
import { DropdownItemClickEventArg, DropdownItemComponent } from '../dropdown-item/dropdown-item.component';

@Component({
  selector: 'app-dropdown-container',
  imports: [],
  templateUrl: './dropdown-container.component.html',
  styleUrl: './dropdown-container.component.css'
})
export class DropdownContainerComponent implements AfterContentInit, OnDestroy {

  @Input() class: string = "";

  constructor(private elementRef: ElementRef) {

  }

  ngAfterContentInit(): void {
    this.subscribe();
  }

  ngOnDestroy(): void {

  }

  unsubscribe() {
    //get access to the elements in the container
    const items = this.dropdownContainer().getElementsByTagName(Constants.WebComponentNames.DropdownItem);
    const elements = Array.from(items);

    elements.forEach(dropdownItem => {
      let dropdownItemHtlElement = <HTMLElement>dropdownItem;
      dropdownItemHtlElement.removeEventListener("onClick",
        ((e: CustomEvent) => this.handleDropdownItemClick(e, this.dropdownContainer())) as EventListener);
    });

  }

  subscribe() {
    //get access to the elements in the container
    const items = this.dropdownContainer().getElementsByTagName(Constants.WebComponentNames.DropdownItem);
    const elements = Array.from(items);

    elements.forEach(dropdownItem => {
      let dropdownItemHtlElement = <HTMLElement>dropdownItem;
      dropdownItemHtlElement.addEventListener("onClick",
        ((e: CustomEvent) => this.handleDropdownItemClick(e, this.dropdownContainer())) as EventListener);
    });

  }

  handleDropdownItemClick(event: CustomEvent, container: HTMLElement) {
    const dropdownClickEventArg = <DropdownItemClickEventArg>event.detail;
    if (!dropdownClickEventArg.closeOnClick) return;

    this.hideContainer(container);
  }

  dropdownContainer() {
    return <HTMLElement>this.elementRef.nativeElement;
  }

  hideContainer(container: HTMLElement) {
    if (!container) return;
    container.style.display = "none";
  }

}

