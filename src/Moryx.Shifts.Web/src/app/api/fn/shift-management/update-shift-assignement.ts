/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftAssignementModel as MoryxShiftsEndpointsShiftAssignementModel } from '../../models/Moryx/Shifts/Endpoints/shift-assignement-model';

export interface UpdateShiftAssignement$Params {
      body?: MoryxShiftsEndpointsShiftAssignementModel
}

export function updateShiftAssignement(http: HttpClient, rootUrl: string, params?: UpdateShiftAssignement$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, updateShiftAssignement.PATH, 'put');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: '*/*', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
    })
  );
}

updateShiftAssignement.PATH = '/api/moryx/shifts/assignements';
