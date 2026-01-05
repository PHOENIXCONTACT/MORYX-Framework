/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';
import { Entry } from '@moryx/ngx-web-framework';


export interface Get$Params {
  id: number;
}

export function get(http: HttpClient, rootUrl: string, params: Get$Params, context?: HttpContext): Observable<StrictHttpResponse<Entry>> {
  const rb = new RequestBuilder(rootUrl, get.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Entry>;
    })
  );
}

get.PATH = '/api/moryx/maintenances/{id}';
