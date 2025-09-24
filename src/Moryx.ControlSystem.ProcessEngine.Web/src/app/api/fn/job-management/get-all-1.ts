/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { JobModel as MoryxControlSystemJobsEndpointsJobModel } from '../../models/Moryx/ControlSystem/Jobs/Endpoints/job-model';

export interface GetAll_1$Params {
}

export function getAll_1(http: HttpClient, rootUrl: string, params?: GetAll_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemJobsEndpointsJobModel>>> {
  const rb = new RequestBuilder(rootUrl, getAll_1.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxControlSystemJobsEndpointsJobModel>>;
    })
  );
}

getAll_1.PATH = '/api/moryx/jobs';
