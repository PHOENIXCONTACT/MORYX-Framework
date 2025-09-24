/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OperationChangedModel as MoryxOrdersEndpointsModelsOperationChangedModel } from '../../models/Moryx/Orders/Endpoints/Models/operation-changed-model';

export interface OperationStream$Params {
}

export function operationStream(http: HttpClient, rootUrl: string, params?: OperationStream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOrdersEndpointsModelsOperationChangedModel>> {
  const rb = new RequestBuilder(rootUrl, operationStream.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxOrdersEndpointsModelsOperationChangedModel>;
    })
  );
}

operationStream.PATH = '/api/moryx/orders/stream';
