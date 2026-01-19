/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import {
  AfterContentInit,
  Component,
  ElementRef,
  HostListener,
  Input,
} from '@angular/core';
import { Constants } from '../constants';

@Component({
  selector: 'app-dropdown-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dropdown-menu.component.html',
  styleUrl: './dropdown-menu.component.css',
})
export class DropdownMenuComponent implements AfterContentInit {
  @Input() align: DropdownMenuAlignement = 'right';
  @Input() position: DropdownMenuPosition = 'bottom';
  id: string = "dropdown-menu-"+Math.floor(Math.random() * 100);
  left: number = 0;
  @Input() level: 'root' | 'child' = 'root';
  container: HTMLElement | undefined;
  top: number = 0;
  constructor(private elementRef: ElementRef) {}

  ngAfterContentInit(): void {
    this.container = this.mainContainer();
    this.initializePosition();
    this.defaultVisibility();
  }

  initializePosition() {
    if (!this.container || this.level != 'root') return;

    const button = <HTMLElement>this.dropdownButton();
    const buttonRectangle = button.getBoundingClientRect();
    const buttonHeight = button.offsetHeight;
    const buttonWidth = button.offsetWidth;

    if (this.align === 'left') {
      if(this.position === 'bottom'){
        this.left = 0;
        this.top = buttonRectangle.y + buttonHeight;
      }
      if(this.position === 'top'){
        this.left = 0;
        this.top = -this.container.offsetHeight;
      }
    }
    
    if (this.align === 'right') {
      if(this.position === 'bottom'){
        this.left = -this.container.offsetWidth + buttonWidth;
        this.top = buttonRectangle.top + buttonHeight;
      }
      if(this.position === 'top'){
        this.left = -this.container.offsetWidth + buttonWidth;
        this.top = -this.container.offsetHeight;
      }
    }
  }

  setContainerPosition() {
    if (!this.container) return;
    this.container.style.left = this.left + 'px';
    this.container.style.top = this.top + 'px';

    this.checkForScreenCollision(this.container);
  }

  private checkForScreenCollision(container: HTMLElement) {
    let screen = {
      x: 0,
      y: 0,
      width: window.innerWidth,
      height: window.innerHeight,
    };
    
    // check left side
    if (container.getBoundingClientRect().x < screen.x) {
      container.style.left = screen.x + 'px'; // pull the object back onto the screen
    }

    // check right side
    if (
      container.getBoundingClientRect().x +
      container.getBoundingClientRect().width >
      screen.width
    ) {
      const diff = screen.width - container.getBoundingClientRect().x;
      container.style.left = -diff + 'px'; // pull the object back onto the screen
    }
  }

  onClick(event: Event) {
    if (this.level === 'child') this.alignSubitemToContainer(event);

    this.toggleVisibility();
  }

  // TO-DO: autoposition sub item container base on visibility in the screen (if the width of the container is visible on the left put it there otherwise put it on the right)
  alignSubitemToContainer(event: Event) {
    const subItemClicked = this.findParentByTag(Constants.WebComponentNames.DropdownSubItem,<HTMLElement>event.target);

    if(!subItemClicked) return;
    const subItemClickedRectangle = subItemClicked.getBoundingClientRect();
    this.left = -subItemClickedRectangle.width;
    this.top = 0;

  }

  dropdownButton() {
    return this.dropdownElement().childNodes[0];
  }

  @HostListener('window:click', ['$event'])
  click(event: Event) {
    const element = <HTMLElement>event.target;
    if (this.dropdownElement()?.contains(element)) return;

    this.defaultVisibility();
  }

  defaultVisibility() {
    if (!this.container) return;
    this.container.style.display = 'none';
    this.container.style.left = '-100000px'; // make the menu invisible
    this.container.style.zIndex = "999";
  }

  toggleVisibility() {
    if (!this.container) return;

    if (this.container.style.display === 'block') {
      this.defaultVisibility();
      return;
    } else {
      this.container.style.display = 'block';
      this.setContainerPosition();
    }
  }

  dropdownElement() {
    return <HTMLElement>this.elementRef.nativeElement;
  }

  mainContainer() {
    const result = Array.from(
      this.dropdownElement().getElementsByTagName(
        Constants.WebComponentNames.DropdownContainer
      ) ?? []
    );
    return result.length ? <HTMLElement>result[0] : undefined;
  }

  findParentByTag(tag: string , element: HTMLElement): HTMLElement | undefined{
    if(!element) return undefined;
    if(tag === element.tagName.toLowerCase()) return element;

    return this.findParentByTag(tag,<HTMLElement>element.parentElement);
  }
}

export type DropdownMenuAlignement = 'left' | 'right';
export type DropdownMenuPosition = 'top' | 'bottom';
