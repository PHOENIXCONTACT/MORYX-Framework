/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { JobProcessModel as MoryxControlSystemProcessesEndpointsJobProcessModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/job-process-model';

export interface GetRunningProcesses$Params {
}

export function getRunningProcesses(http: HttpClient, rootUrl: string, params?: GetRunningProcesses$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>> {
  const rb = new RequestBuilder(rootUrl, getRunningProcesses.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>;
    })
  );
}

getRunningProcesses.PATH = '/api/moryx/processes/running';
