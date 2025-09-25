/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ActivityResourceModel as MoryxControlSystemProcessesEndpointsActivityResourceModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/activity-resource-model';

export interface GetTargets$Params {
  id: number;
}

export function getTargets(http: HttpClient, rootUrl: string, params: GetTargets$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>> {
  const rb = new RequestBuilder(rootUrl, getTargets.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>;
    })
  );
}

getTargets.PATH = '/api/moryx/processes/running/{id}/targets';
