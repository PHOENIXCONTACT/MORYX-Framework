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
import {
  LauncherState,
  LauncherStateService,
} from '../services/launcher-state.service';
import { Constants } from '../constants';
import { NavigationButtonComponent } from '../navigation-button/navigation-button.component';

@Component({
  selector: 'app-page-layout',
  standalone: true,
  imports: [CommonModule],
  providers: [LauncherStateService],
  templateUrl: './page-layout.component.html',
  styleUrl: './page-layout.component.css',
})
export class PageLayoutComponent implements AfterContentInit {
  private KEY_NAME = 'Escape';
  @Input() fullscreenButton: string = 'fullscreen';
  @Input() operatorButton: string = 'operator-mode';
  state: LauncherState = <LauncherState>{
    fullscreen: false,
    operatorMode: false,
  };
  layout: HTMLElement | undefined;
  numberOfMenuOnTablet = 5;
  numberOfMenuOnLaptop = 7;
  numberOfMenuOnDesktop = 12;
  tableWidth = 640;
  laptopWidth = 1024;
  desktopWidth = 1280;
  fixedButtons: Element[] = [];
  defaultButtons: Element[] = [];

  constructor(
    private launcherState: LauncherStateService,
    private elementRef: ElementRef
  ) {
    const oldState = this.launcherState.getState();
    if (!oldState) return;
    this.state = oldState;
  }

  ngAfterContentInit(): void {
    this.layout = <HTMLElement>this.elementRef.nativeElement;
    var navButtons = Array.from(
      this.layout?.getElementsByTagName(
        Constants.WebComponentNames.NavigationButton
      ) ?? []
    );
    this.fixedButtons = navButtons.filter(
      (x) =>
        x.getAttribute(NavigationButtonComponent.POSITION_ATTRIBUTE) === 'fixed'
    );
    this.defaultButtons = navButtons.filter(
      (x) =>
        x.getAttribute(NavigationButtonComponent.POSITION_ATTRIBUTE) != 'fixed'
    ).reverse();
    this.hideMenuElementBasedOnScreenSize(
      window.innerWidth,
      window.innerHeight
    );
  }

  @HostListener('click', ['$event'])
  onClick(eventArg: Event) {
    const element: HTMLElement = <HTMLElement>eventArg.target;
    this.handleFullscreen(element);
    this.handleOperatorMode(element);

    this.launcherState.updateState(this.state);
  }

  handleOperatorMode(element: HTMLElement) {
    const operatorModeButtons = Array.from(
      document.getElementsByName(this.operatorButton)
    );
    if (
      !operatorModeButtons.length ||
      (element.getAttribute('name') != this.operatorButton &&
        !operatorModeButtons.some((x) => x.contains(element)))
    )
      return;
    this.state = { ...this.state, operatorMode: !this.state?.operatorMode };
  }

  handleFullscreen(element: HTMLElement) {
    const fullscreenModeButtons = Array.from(
      document.getElementsByName(this.fullscreenButton)
    );
    if (
      !fullscreenModeButtons.length ||
      (element.getAttribute('name') != this.fullscreenButton &&
        !fullscreenModeButtons.some((x) => x.contains(element)))
    )
      return;
    this.exitFullscreen();
  }

  @HostListener('window:keyup', ['$event'])
  onKeyUp(eventArg: KeyboardEvent) {
    if (eventArg.code != this.KEY_NAME) return;

    if (this.state.fullscreen)
      this.state = { ...this.state, fullscreen: false };
    if (this.state.operatorMode)
      this.state = { ...this.state, operatorMode: false };

    this.launcherState.updateState(this.state);
  }

  exitFullscreen() {
    this.state = { ...this.state, fullscreen: !this.state?.fullscreen };
  }

  @HostListener('window:resize', ['$event'])
  onScreenResize(event: UIEvent) {
    if (!this.state.operatorMode || this.state.fullscreen) return;

    this.displayMenuElements();
    this.hideMenuElementBasedOnScreenSize(
      window.innerWidth,
      window.innerHeight
    );
  }

  numberOfElementByScreenWidth(width: number): number {
    if (!this.state.operatorMode) return this.numberOfMenuOnDesktop;

    if (width <= this.tableWidth)
      //tablet
      return this.numberOfMenuOnTablet;
    if (width > this.tableWidth && width <= this.laptopWidth)
      // laptop
      return this.numberOfMenuOnLaptop;

    //desktop
    return this.numberOfMenuOnDesktop;
  }

  hideMenuElementBasedOnScreenSize(width: number, height: number) {
    this.handleScreenWidth(width);
    //TODO: handle screen height for Administrator mode (left bar menu)
  }

  handleScreenWidth(width: number) {
    var displayableElementsCount = this.numberOfElementByScreenWidth(width);
    if (!this.fixedButtons.length && !this.defaultButtons.length) return;

    if (
      this.defaultButtons.length + this.fixedButtons.length <=
      displayableElementsCount
    )
      return;

    var numberOfElementsToHide =
      this.defaultButtons.length +
      this.fixedButtons.length -
      displayableElementsCount;
    if (numberOfElementsToHide > this.defaultButtons.length) return;
    for (let index = 0; index < numberOfElementsToHide; index++) {
      const elementToHide = <HTMLElement>this.defaultButtons[index];
      elementToHide.style.display = 'none'; // hide the button
    }
  }

  displayMenuElements() {
    for (let index = 0; index < this.defaultButtons.length; index++) {
      const element = <HTMLElement>this.defaultButtons[index];
      element.style.display = 'block'; // display the button
    }
  }
}

