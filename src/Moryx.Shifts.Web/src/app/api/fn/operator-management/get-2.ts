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

import { ExtendedOperatorModel } from '../../models/extended-operator-model';

export interface Get_2$Params {
  identifier: string;
}

export function get_2(http: HttpClient, rootUrl: string, params: Get_2$Params, context?: HttpContext): Observable<StrictHttpResponse<ExtendedOperatorModel>> {
  const rb = new RequestBuilder(rootUrl, get_2.PATH, 'get');
  if (params) {
    rb.path('identifier', params.identifier, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ExtendedOperatorModel>;
    })
  );
}

get_2.PATH = '/api/moryx/operators/{identifier}';

