/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Directive, HostListener } from '@angular/core';

@Directive({
    selector: '[click-stop-propagation]',
    standalone: false
})
export class ClickStopPropagationDirective {

  @HostListener("click", ["$event"])
  public onClick(event: any): void
  {
      event.stopPropagation();
  }

}

