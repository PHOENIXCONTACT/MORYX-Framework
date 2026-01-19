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

import { ShiftAssignementCreationContextModel } from '../../models/shift-assignement-creation-context-model';
import { ShiftAssignementModel } from '../../models/shift-assignement-model';

export interface CreateShiftAssignement$Params {
      body?: ShiftAssignementCreationContextModel
}

export function createShiftAssignement(http: HttpClient, rootUrl: string, params?: CreateShiftAssignement$Params, context?: HttpContext): Observable<StrictHttpResponse<ShiftAssignementModel>> {
  const rb = new RequestBuilder(rootUrl, createShiftAssignement.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ShiftAssignementModel>;
    })
  );
}

createShiftAssignement.PATH = '/api/moryx/shifts/assignements';

