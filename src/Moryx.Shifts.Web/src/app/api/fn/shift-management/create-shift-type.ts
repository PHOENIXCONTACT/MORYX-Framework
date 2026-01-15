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

import { ShiftTypeCreationContextModel } from '../../models/shift-type-creation-context-model';
import { ShiftTypeModel } from '../../models/shift-type-model';

export interface CreateShiftType$Params {
      body?: ShiftTypeCreationContextModel
}

export function createShiftType(http: HttpClient, rootUrl: string, params?: CreateShiftType$Params, context?: HttpContext): Observable<StrictHttpResponse<ShiftTypeModel>> {
  const rb = new RequestBuilder(rootUrl, createShiftType.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ShiftTypeModel>;
    })
  );
}

createShiftType.PATH = '/api/moryx/shifts/types';

