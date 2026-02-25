/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from '@angular/router';
import { Management } from './components/management/management';
import { Editor } from './components/sessions/editor/editor';
import { Sessions } from './components/sessions/sessions';

export const routes: Routes = [
  {
    path: 'session',
    component: Sessions,
    children:
    [
        { path: ':token', component: Editor }
    ],
  },
  { path: 'management', component: Management },
  { path: '', redirectTo: 'management', pathMatch: 'full' },
];

