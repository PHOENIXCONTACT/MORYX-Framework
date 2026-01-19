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

import { ShiftTypeModel } from '../../models/shift-type-model';

export interface GetShiftTypes$Params {
}

export function getShiftTypes(http: HttpClient, rootUrl: string, params?: GetShiftTypes$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftTypeModel>>> {
  const rb = new RequestBuilder(rootUrl, getShiftTypes.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<ShiftTypeModel>>;
    })
  );
}

getShiftTypes.PATH = '/api/moryx/shifts/types';

