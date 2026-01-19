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

import { OperationChangedModel } from '../../models/operation-changed-model';

export interface OperationStream$Params {
}

export function operationStream(http: HttpClient, rootUrl: string, params?: OperationStream$Params, context?: HttpContext): Observable<StrictHttpResponse<OperationChangedModel>> {
  const rb = new RequestBuilder(rootUrl, operationStream.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<OperationChangedModel>;
    })
  );
}

operationStream.PATH = '/api/moryx/orders/stream';

