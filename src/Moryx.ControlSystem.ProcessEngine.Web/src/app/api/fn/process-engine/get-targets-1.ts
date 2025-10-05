/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ActivityResourceModel as MoryxControlSystemProcessesEndpointsActivityResourceModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/activity-resource-model';

export interface GetTargets_1$Params {
  id: number;
  activityId: number;
}

export function getTargets_1(http: HttpClient, rootUrl: string, params: GetTargets_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>> {
  const rb = new RequestBuilder(rootUrl, getTargets_1.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
    rb.path('activityId', params.activityId, {});
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

getTargets_1.PATH = '/api/moryx/processes/running/{id}/targets/{activityId}';
