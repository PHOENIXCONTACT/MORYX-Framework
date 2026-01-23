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

import { AdviceContext } from '../../models/advice-context';

export interface GetAdviceContext$Params {
  guid: string;
}

export function getAdviceContext(http: HttpClient, rootUrl: string, params: GetAdviceContext$Params, context?: HttpContext): Observable<StrictHttpResponse<AdviceContext>> {
  const rb = new RequestBuilder(rootUrl, getAdviceContext.PATH, 'get');
  if (params) {
    rb.path('guid', params.guid, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<AdviceContext>;
    })
  );
}

getAdviceContext.PATH = '/api/moryx/orders/{guid}/advice';

