/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Routes } from "@angular/router";
import { CardsComponent } from "./components/cards/cards.component";
import { HistoryComponent } from "./components/history/history.component";
import { SummariesComponent } from "./components/summaries/summaries.component";

export const routes: Routes = [
    {path: "cards", component: CardsComponent },
    {path: "summary", component: SummariesComponent },
    {path: "", pathMatch:"full", redirectTo : "cards"}
];

