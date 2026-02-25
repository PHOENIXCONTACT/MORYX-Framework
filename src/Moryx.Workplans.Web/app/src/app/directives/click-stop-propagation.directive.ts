/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Directive } from '@angular/core';

@Directive({
  selector: '[click-stop-propagation]',
  host: {
    '(click)': 'onClick($event)'
  }
})
export class ClickStopPropagationDirective {

  onClick(event: any): void {
    event.stopPropagation();
  }

}

