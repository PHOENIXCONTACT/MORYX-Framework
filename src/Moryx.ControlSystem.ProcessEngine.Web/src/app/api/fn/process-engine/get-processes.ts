/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { JobProcessModel as MoryxControlSystemProcessesEndpointsJobProcessModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/job-process-model';

export interface GetProcesses$Params {
  productInstanceId: number;
}

export function getProcesses(http: HttpClient, rootUrl: string, params: GetProcesses$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>> {
  const rb = new RequestBuilder(rootUrl, getProcesses.PATH, 'get');
  if (params) {
    rb.path('productInstanceId', params.productInstanceId, {});
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

getProcesses.PATH = '/api/moryx/processes/instance/{productInstanceId}';
