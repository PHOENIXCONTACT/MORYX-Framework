/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OperatorModel as MoryxOperatorsEndpointsOperatorModel } from '../../models/Moryx/Operators/Endpoints/operator-model';

export interface Update_1$Params {
  identifier: string;
      body?: MoryxOperatorsEndpointsOperatorModel
}

export function update_1(http: HttpClient, rootUrl: string, params: Update_1$Params, context?: HttpContext): Observable<StrictHttpResponse<string>> {
  const rb = new RequestBuilder(rootUrl, update_1.PATH, 'put');
  if (params) {
    rb.path('identifier', params.identifier, {});
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<string>;
    })
  );
}

update_1.PATH = '/api/moryx/operators/{identifier}';
