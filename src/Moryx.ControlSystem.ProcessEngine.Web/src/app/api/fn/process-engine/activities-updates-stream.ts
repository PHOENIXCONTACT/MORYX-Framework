/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { ProcessActivityModel as MoryxControlSystemProcessesEndpointsProcessActivityModel } from '../../models/Moryx/ControlSystem/Processes/Endpoints/process-activity-model';

export interface ActivitiesUpdatesStream$Params {
}

export function activitiesUpdatesStream(http: HttpClient, rootUrl: string, params?: ActivitiesUpdatesStream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessActivityModel>> {
  const rb = new RequestBuilder(rootUrl, activitiesUpdatesStream.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessActivityModel>;
    })
  );
}

activitiesUpdatesStream.PATH = '/api/moryx/processes/stream/activities';
