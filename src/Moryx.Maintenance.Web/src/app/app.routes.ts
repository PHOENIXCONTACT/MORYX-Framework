import { Routes } from '@angular/router';
import { MaintenancesComponent } from './components/maintenances/maintenances.component';
import { MaintenanceComponent } from './components/maintenance/maintenance.component';

export const routes: Routes = [
    { path: "maintenances", component: MaintenancesComponent },
    { path: "maintenances/:id", component: MaintenanceComponent },
    
    {path: '', redirectTo: '/maintenances', pathMatch: 'full'}
];
