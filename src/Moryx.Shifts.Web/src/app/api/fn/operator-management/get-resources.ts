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

import { AttendableResourceModel } from '../../models/attendable-resource-model';

export interface GetResources$Params {
  operatorIdentifier: string;
}

export function getResources(http: HttpClient, rootUrl: string, params: GetResources$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<AttendableResourceModel>>> {
  const rb = new RequestBuilder(rootUrl, getResources.PATH, 'get');
  if (params) {
    rb.path('operatorIdentifier', params.operatorIdentifier, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<AttendableResourceModel>>;
    })
  );
}

getResources.PATH = '/api/moryx/operators/resources/{operatorIdentifier}';

