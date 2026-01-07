import { Injectable, NgZone } from '@angular/core';
import { MaintenanceManagementService } from '../api/services';
import { BehaviorSubject } from 'rxjs';
import { Maintenance, mapFrom } from '../models/maintenance';
import { MaintenanceOrderDto } from '../api/models/Moryx/Maintenance/Endpoints/Dtos/maintenance-order-dto';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceStreamService {

  $updateMaintenanceOrder = new BehaviorSubject<Maintenance | undefined>(undefined);

  constructor(
    private zone : NgZone,
    private maintenanceService : MaintenanceManagementService
  ) { 
    this.publishUpdates();
  }

   private publishUpdates() {
    const eventSource = new EventSource(this.maintenanceService.rootUrl + MaintenanceManagementService.StreamPath);
    eventSource.onmessage = event => {
      const dto = JSON.parse(event.data) as MaintenanceOrderDto;
      this.zone.run(() => {
        const mapped = mapFrom(dto);
        this.$updateMaintenanceOrder.next(mapped)
      });      
    };
  }
}
