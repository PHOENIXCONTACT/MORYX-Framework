/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftModel as MoryxShiftsEndpointsShiftModel } from '../../models/Moryx/Shifts/Endpoints/shift-model';

export interface GetShifts_1$Params {
  earliestDate?: string;
  latestDate?: string;
}

export function getShifts_1(http: HttpClient, rootUrl: string, params?: GetShifts_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxShiftsEndpointsShiftModel>>> {
  const rb = new RequestBuilder(rootUrl, getShifts_1.PATH, 'get');
  if (params) {
    rb.query('earliestDate', params.earliestDate, {});
    rb.query('latestDate', params.latestDate, {});
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

getShifts_1.PATH = '/api/moryx/shifts/filter';
