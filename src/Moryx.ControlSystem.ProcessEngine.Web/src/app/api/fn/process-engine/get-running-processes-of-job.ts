/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { JobProcessModel as MoryxControlSystemProcessesEndpointsJobProcessModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/job-process-model';

export interface GetRunningProcessesOfJob$Params {
  jobId?: number;
  allProcesses?: boolean;
}

export function getRunningProcessesOfJob(http: HttpClient, rootUrl: string, params?: GetRunningProcessesOfJob$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>> {
  const rb = new RequestBuilder(rootUrl, getRunningProcessesOfJob.PATH, 'get');
  if (params) {
    rb.query('jobId', params.jobId, {});
    rb.query('allProcesses', params.allProcesses, {});
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

getRunningProcessesOfJob.PATH = '/api/moryx/processes/job';
