/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftAssignementModel as MoryxShiftsEndpointsShiftAssignementModel } from '../../models/Moryx/Shifts/Endpoints/shift-assignement-model';
import { ShiftAssignementCreationContextModel as MoryxShiftsShiftAssignementCreationContextModel } from '../../models/Moryx/Shifts/shift-assignement-creation-context-model';

export interface CreateShiftAssignement$Params {
      body?: MoryxShiftsShiftAssignementCreationContextModel
}

export function createShiftAssignement(http: HttpClient, rootUrl: string, params?: CreateShiftAssignement$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxShiftsEndpointsShiftAssignementModel>> {
  const rb = new RequestBuilder(rootUrl, createShiftAssignement.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxShiftsEndpointsShiftAssignementModel>;
    })
  );
}

createShiftAssignement.PATH = '/api/moryx/shifts/assignements';
