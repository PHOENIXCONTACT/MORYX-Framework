/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { JobModel as MoryxControlSystemJobsEndpointsJobModel } from '../../models/Moryx/ControlSystem/Jobs/Endpoints/job-model';

export interface GetJob$Params {
  jobId: number;
}

export function getJob(http: HttpClient, rootUrl: string, params: GetJob$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemJobsEndpointsJobModel>> {
  const rb = new RequestBuilder(rootUrl, getJob.PATH, 'get');
  if (params) {
    rb.path('jobId', params.jobId, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxControlSystemJobsEndpointsJobModel>;
    })
  );
}

getJob.PATH = '/api/moryx/jobs/{jobId}';
