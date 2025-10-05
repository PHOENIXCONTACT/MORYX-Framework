/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftModel as MoryxShiftsEndpointsShiftModel } from '../../models/Moryx/Shifts/Endpoints/shift-model';

export interface GetShifts$Params {
}

export function getShifts(http: HttpClient, rootUrl: string, params?: GetShifts$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxShiftsEndpointsShiftModel>>> {
  const rb = new RequestBuilder(rootUrl, getShifts.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxShiftsEndpointsShiftModel>>;
    })
  );
}

getShifts.PATH = '/api/moryx/shifts';
