/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { MaintenanceOrderDto as MoryxMaintenanceEndpointsDtosMaintenanceOrderDto } from '../../models/Moryx/Maintenance/Endpoints/Dtos/maintenance-order-dto';

export interface Stream$Params {
}

export function stream(http: HttpClient, rootUrl: string, params?: Stream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxMaintenanceEndpointsDtosMaintenanceOrderDto>> {
  const rb = new RequestBuilder(rootUrl, stream.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxMaintenanceEndpointsDtosMaintenanceOrderDto>;
    })
  );
}

stream.PATH = '/api/moryx/maintenances/stream';
