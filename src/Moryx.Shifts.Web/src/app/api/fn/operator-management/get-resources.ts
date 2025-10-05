/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ResourceModel as MoryxOperatorsEndpointsResourceModel } from '../../models/Moryx/Operators/Endpoints/resource-model';

export interface GetResources$Params {
}

export function getResources(http: HttpClient, rootUrl: string, params?: GetResources$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsEndpointsResourceModel>>> {
  const rb = new RequestBuilder(rootUrl, getResources.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxOperatorsEndpointsResourceModel>>;
    })
  );
}

getResources.PATH = '/api/moryx/operators/resources';
