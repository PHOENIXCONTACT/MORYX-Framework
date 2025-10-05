/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftAssignementModel as MoryxShiftsEndpointsShiftAssignementModel } from '../../models/Moryx/Shifts/Endpoints/shift-assignement-model';

export interface GetDummy$Params {
}

export function getDummy(http: HttpClient, rootUrl: string, params?: GetDummy$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxShiftsEndpointsShiftAssignementModel>>> {
  const rb = new RequestBuilder(rootUrl, getDummy.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxShiftsEndpointsShiftAssignementModel>>;
    })
  );
}

getDummy.PATH = '/api/moryx/shifts/dummy';
