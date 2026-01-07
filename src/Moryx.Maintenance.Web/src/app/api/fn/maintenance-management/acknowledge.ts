/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { Acknowledgement as MoryxMaintenanceAcknowledgement } from '../../models/Moryx/Maintenance/acknowledgement';

export interface Acknowledge$Params {
  id: number;
      body?: MoryxMaintenanceAcknowledgement
}

export function acknowledge(http: HttpClient, rootUrl: string, params: Acknowledge$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, acknowledge.PATH, 'put');
  if (params) {
    rb.path('id', params.id, {});
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'text', accept: '*/*', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
    })
  );
}

acknowledge.PATH = '/api/moryx/maintenances/{id}/acknowledge';
