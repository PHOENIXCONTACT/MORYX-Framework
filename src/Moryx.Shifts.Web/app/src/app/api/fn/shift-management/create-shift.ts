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

import { ShiftCreationContextModel } from '../../models/shift-creation-context-model';
import { ShiftModel } from '../../models/shift-model';

export interface CreateShift$Params {
      body?: ShiftCreationContextModel
}

export function createShift(http: HttpClient, rootUrl: string, params?: CreateShift$Params, context?: HttpContext): Observable<StrictHttpResponse<ShiftModel>> {
  const rb = new RequestBuilder(rootUrl, createShift.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ShiftModel>;
    })
  );
}

createShift.PATH = '/api/moryx/shifts';

