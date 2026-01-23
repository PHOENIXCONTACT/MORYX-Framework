/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from '@angular/router';
import { ManagementComponent } from './components/management/management.component';
import { EditorComponent } from './components/sessions/editor/editor.component';
import { SessionsComponent } from './components/sessions/sessions.component';

export const routes: Routes = [
  {
    path: 'session',
    component: SessionsComponent,
    children: 
    [
        { path: ':token', component: EditorComponent }
    ],
  },
  { path: 'management', component: ManagementComponent },
  { path: '', redirectTo: 'management', pathMatch: 'full' },
];

