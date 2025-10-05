/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ProcessHolderGroupModel as MoryxControlSystemProcessesEndpointsProcessHolderGroupModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/process-holder-group-model';

export interface GroupStream$Params {
}

export function groupStream(http: HttpClient, rootUrl: string, params?: GroupStream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>> {
  const rb = new RequestBuilder(rootUrl, groupStream.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>;
    })
  );
}

groupStream.PATH = '/api/moryx/processes/holders/stream';
