/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component } from '@angular/core';
import { HorizontalModuleNav } from '../../navigation/horizontal-module-nav/horizontal-module-nav';
import { LayoutBase } from '../layout-base';

@Component({
  selector: 'app-operator-layout',
  imports: [HorizontalModuleNav],
  templateUrl: './operator-layout.html',
  styleUrl: './operator-layout.scss',
})
export class OperatorLayout extends LayoutBase {
}
