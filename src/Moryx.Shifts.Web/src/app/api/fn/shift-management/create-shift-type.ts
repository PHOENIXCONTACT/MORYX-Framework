/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftTypeModel as MoryxShiftsEndpointsShiftTypeModel } from '../../models/Moryx/Shifts/Endpoints/shift-type-model';
import { ShiftTypeCreationContextModel as MoryxShiftsShiftTypeCreationContextModel } from '../../models/Moryx/Shifts/shift-type-creation-context-model';

export interface CreateShiftType$Params {
      body?: MoryxShiftsShiftTypeCreationContextModel
}

export function createShiftType(http: HttpClient, rootUrl: string, params?: CreateShiftType$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxShiftsEndpointsShiftTypeModel>> {
  const rb = new RequestBuilder(rootUrl, createShiftType.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxShiftsEndpointsShiftTypeModel>;
    })
  );
}

createShiftType.PATH = '/api/moryx/shifts/types';
