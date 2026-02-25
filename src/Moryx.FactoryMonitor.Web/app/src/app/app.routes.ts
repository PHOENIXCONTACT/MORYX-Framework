/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from '@angular/router';
import { FactoryBoard } from './components/factory-board/factory-board';

export const routes: Routes = [
  {
    path: 'factory/:id',
    component: FactoryBoard,
  },
  {
    path: 'root',
    component: FactoryBoard,
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'root',
  },
];

