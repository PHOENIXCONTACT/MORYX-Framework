/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { WpcGroupModel as MoryxControlSystemWpcEndpointsModelsWpcGroupModel } from '../../models/Moryx/ControlSystem/Wpc/Endpoints/Models/wpc-group-model';

export interface GetWpcGroups$Params {
}

export function getWpcGroups(http: HttpClient, rootUrl: string, params?: GetWpcGroups$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemWpcEndpointsModelsWpcGroupModel>>> {
  const rb = new RequestBuilder(rootUrl, getWpcGroups.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxControlSystemWpcEndpointsModelsWpcGroupModel>>;
    })
  );
}

getWpcGroups.PATH = '/api/moryx/wpc/groups';
