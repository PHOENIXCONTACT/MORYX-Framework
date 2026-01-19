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

export interface GetOperatorsByResource$Params {
  resourceId: number;
}

export function getOperatorsByResource(http: HttpClient, rootUrl: string, params: GetOperatorsByResource$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ExtendedOperatorModel>>> {
  const rb = new RequestBuilder(rootUrl, getOperatorsByResource.PATH, 'get');
  if (params) {
    rb.path('resourceId', params.resourceId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<ExtendedOperatorModel>>;
    })
  );
}

getOperatorsByResource.PATH = '/api/moryx/operators/get-operators-by-resource/{resourceId}';

