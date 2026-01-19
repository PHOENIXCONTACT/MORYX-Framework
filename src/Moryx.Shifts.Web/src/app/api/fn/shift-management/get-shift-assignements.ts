/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ShiftAssignementModel } from '../../models/shift-assignement-model';

export interface GetShiftAssignements$Params {
}

export function getShiftAssignements(http: HttpClient, rootUrl: string, params?: GetShiftAssignements$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftAssignementModel>>> {
  const rb = new RequestBuilder(rootUrl, getShiftAssignements.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<ShiftAssignementModel>>;
    })
  );
}

getShiftAssignements.PATH = '/api/moryx/shifts/assignements';

