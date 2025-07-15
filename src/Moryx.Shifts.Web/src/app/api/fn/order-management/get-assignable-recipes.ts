/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { OperationModel as MoryxOrdersEndpointsOperationModel } from '../../models/Moryx/Orders/Endpoints/operation-model';

export interface GetAssignableRecipes$Params {
  identifier?: string;
  revision?: number;
}

export function getAssignableRecipes(http: HttpClient, rootUrl: string, params?: GetAssignableRecipes$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOrdersEndpointsOperationModel>>> {
  const rb = new RequestBuilder(rootUrl, getAssignableRecipes.PATH, 'get');
  if (params) {
    rb.query('identifier', params.identifier, {});
    rb.query('revision', params.revision, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxOrdersEndpointsOperationModel>>;
    })
  );
}

getAssignableRecipes.PATH = '/api/moryx/orders/recipes';
