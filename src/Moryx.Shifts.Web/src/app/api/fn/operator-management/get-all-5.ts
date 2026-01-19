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

import { AssignableOperator } from '../../models/assignable-operator';

export interface GetAll_5$Params {
}

export function getAll_5(http: HttpClient, rootUrl: string, params?: GetAll_5$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<AssignableOperator>>> {
  const rb = new RequestBuilder(rootUrl, getAll_5.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<AssignableOperator>>;
    })
  );
}

getAll_5.PATH = '/api/moryx/operators';

