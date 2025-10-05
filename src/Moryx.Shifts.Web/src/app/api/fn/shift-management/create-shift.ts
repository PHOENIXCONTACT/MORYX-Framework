/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftModel as MoryxShiftsEndpointsShiftModel } from '../../models/Moryx/Shifts/Endpoints/shift-model';
import { ShiftCreationContextModel as MoryxShiftsShiftCreationContextModel } from '../../models/Moryx/Shifts/shift-creation-context-model';

export interface CreateShift$Params {
      body?: MoryxShiftsShiftCreationContextModel
}

export function createShift(http: HttpClient, rootUrl: string, params?: CreateShift$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxShiftsEndpointsShiftModel>> {
  const rb = new RequestBuilder(rootUrl, createShift.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxShiftsEndpointsShiftModel>;
    })
  );
}

createShift.PATH = '/api/moryx/shifts';
