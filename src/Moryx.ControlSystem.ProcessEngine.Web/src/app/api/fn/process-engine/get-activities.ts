/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ProcessActivityModel as MoryxControlSystemProcessesEndpointsProcessActivityModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/process-activity-model';

export interface GetActivities$Params {
  id: number;
}

export function getActivities(http: HttpClient, rootUrl: string, params: GetActivities$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsProcessActivityModel>>> {
  const rb = new RequestBuilder(rootUrl, getActivities.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsProcessActivityModel>>;
    })
  );
}

getActivities.PATH = '/api/moryx/processes/running/{id}/activities';
