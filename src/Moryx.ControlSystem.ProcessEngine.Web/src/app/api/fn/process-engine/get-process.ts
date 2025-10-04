/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { JobProcessModel as MoryxControlSystemProcessesEndpointsJobProcessModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/job-process-model';

export interface GetProcess$Params {
  id: number;
}

export function getProcess(http: HttpClient, rootUrl: string, params: GetProcess$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsJobProcessModel>> {
  const rb = new RequestBuilder(rootUrl, getProcess.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxControlSystemProcessesEndpointsJobProcessModel>;
    })
  );
}

getProcess.PATH = '/api/moryx/processes/running/{id}';
