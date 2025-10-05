/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { BeginModel as MoryxOrdersEndpointsBeginModel } from '../../models/Moryx/Orders/Endpoints/begin-model';

export interface BeginOperation$Params {
  guid: string;
      body?: MoryxOrdersEndpointsBeginModel
}

export function beginOperation(http: HttpClient, rootUrl: string, params: BeginOperation$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, beginOperation.PATH, 'post');
  if (params) {
    rb.path('guid', params.guid, {});
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: '*/*', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
    })
  );
}

beginOperation.PATH = '/api/moryx/orders/{guid}/begin';
