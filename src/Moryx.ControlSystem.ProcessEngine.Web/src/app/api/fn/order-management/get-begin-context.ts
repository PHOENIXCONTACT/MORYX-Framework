/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { BeginContext as MoryxOrdersBeginContext } from '../../models/Moryx/Orders/begin-context';

export interface GetBeginContext$Params {
  guid: string;
}

export function getBeginContext(http: HttpClient, rootUrl: string, params: GetBeginContext$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOrdersBeginContext>> {
  const rb = new RequestBuilder(rootUrl, getBeginContext.PATH, 'get');
  if (params) {
    rb.path('guid', params.guid, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxOrdersBeginContext>;
    })
  );
}

getBeginContext.PATH = '/api/moryx/orders/{guid}/begin';
