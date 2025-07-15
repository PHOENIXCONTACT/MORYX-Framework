/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftTypeModel as MoryxShiftsEndpointsShiftTypeModel } from '../../models/Moryx/Shifts/Endpoints/shift-type-model';

export interface GetShiftTypes$Params {
}

export function getShiftTypes(http: HttpClient, rootUrl: string, params?: GetShiftTypes$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxShiftsEndpointsShiftTypeModel>>> {
  const rb = new RequestBuilder(rootUrl, getShiftTypes.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxShiftsEndpointsShiftTypeModel>>;
    })
  );
}

getShiftTypes.PATH = '/api/moryx/shifts/types';
