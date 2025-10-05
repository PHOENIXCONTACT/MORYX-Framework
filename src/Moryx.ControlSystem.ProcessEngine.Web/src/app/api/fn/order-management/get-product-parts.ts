/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ProductPartModel as MoryxOrdersEndpointsProductPartModel } from '../../models/Moryx/Orders/Endpoints/product-part-model';

export interface GetProductParts$Params {
  guid: string;
}

export function getProductParts(http: HttpClient, rootUrl: string, params: GetProductParts$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOrdersEndpointsProductPartModel>>> {
  const rb = new RequestBuilder(rootUrl, getProductParts.PATH, 'get');
  if (params) {
    rb.path('guid', params.guid, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxOrdersEndpointsProductPartModel>>;
    })
  );
}

getProductParts.PATH = '/api/moryx/orders/{guid}/productparts';
