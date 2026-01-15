/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from '@angular/router';
import { FactoryBoardComponent } from './components/factory-board/factory-board.component';

export const routes: Routes = [
  {
    path: 'factory/:id',
    component: FactoryBoardComponent,
  },
  {
    path: 'root',
    component: FactoryBoardComponent,
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'root',
  },
];

