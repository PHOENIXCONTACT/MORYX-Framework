/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, ElementRef, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MoryxShell, SearchRequestCallback, SearchSuggestion } from './shell';
import { localLanguage } from '../utils';

@Component({
  selector: 'app-search-box',
  imports: [FormsModule],
  templateUrl: './search-box.html',
  styleUrl: './search-box.css'
})
export class SearchBox implements OnInit {

  @Input() placeholder: string = "Search...";

  disabled: boolean = true;
  subscriber: SearchRequestCallback = function (term: string, complete: boolean) {
  };
  suggestions: Array<SearchSuggestion> = [];
  searchValue: string = "";
  id: string = "search-box-" + Math.floor(Math.random() * 100);

  constructor(private elementRef: ElementRef) {
  }

  ngOnInit(): void {
    const shell = new Object() as MoryxShell;

    shell.initSearchBar = this.initSearchBar.bind(this);
    shell.updateSuggestions = this.updateSuggestions.bind(this);
    shell.initLanguage = this.initLanguage.bind(this);
    //Export shell object as API for modules
    window.shell = shell;
  }

  @HostListener('window:click', ['$event'])
  handleClickOutSideTheSearchBar(event: Event) {
    const element = <HTMLElement>event.target;
    const searchbox = <HTMLElement>this.elementRef.nativeElement;

    if (element.id === this.id || searchbox.contains(element)) return;

    this.suggestions = [];
  }

  updateSuggestions(suggestions: SearchSuggestion[]) {
    this.suggestions = suggestions;
  }

  initLanguage(): string {
    const localeString = localLanguage();
    ;
    const languageString = localeString.slice(0, 2);
    return languageString;
  }

  initSearchBar(callback: SearchRequestCallback, disableSearchBox: boolean): void {
    this.subscriber = callback;
    this.disabled = disableSearchBox;
    this.searchValue = '';
    this.updateSuggestions([]);
  }

  searchKeyUp(event: KeyboardEvent) {
    const input = <HTMLInputElement>event.target;
    const value = input.value;
    var complete = event.keyCode === 13;
    this.subscriber(value, complete);
    if (complete || value === '') {
      this.searchValue = '';
      this.suggestions = [];
    }
  }

  search() {
    this.subscriber(this.searchValue, true);
    this.searchValue = "";
  }

}

declare global {
  interface Window {
    shell: MoryxShell;
  }
}


