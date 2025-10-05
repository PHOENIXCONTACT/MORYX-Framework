/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ProcessHolderGroupModel as MoryxControlSystemProcessesEndpointsProcessHolderGroupModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/process-holder-group-model';
import { ApiResponse } from '../../models/Moryx/ControlSystem/Processes/Endpoints/api-response-model';

export interface GetProcessHolderGroups$Params {
}

export function getProcessHolderGroups(http: HttpClient, rootUrl: string, params?: GetProcessHolderGroups$Params, context?: HttpContext): Observable<StrictHttpResponse<ApiResponse<Array<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>>>> {
  const rb = new RequestBuilder(rootUrl, getProcessHolderGroups.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<ApiResponse<Array<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>>>;
    })
  );
}

getProcessHolderGroups.PATH = '/api/moryx/processes/holders/groups';
