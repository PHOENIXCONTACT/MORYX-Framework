/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OperationLogMessageModel as MoryxOrdersEndpointsOperationLogMessageModel } from '../../models/Moryx/Orders/Endpoints/operation-log-message-model';

export interface GetLogs$Params {
  guid: string;
}

export function getLogs(http: HttpClient, rootUrl: string, params: GetLogs$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOrdersEndpointsOperationLogMessageModel>> {
  const rb = new RequestBuilder(rootUrl, getLogs.PATH, 'get');
  if (params) {
    rb.path('guid', params.guid, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxOrdersEndpointsOperationLogMessageModel>;
    })
  );
}

getLogs.PATH = '/api/moryx/orders/{guid}/logs';
