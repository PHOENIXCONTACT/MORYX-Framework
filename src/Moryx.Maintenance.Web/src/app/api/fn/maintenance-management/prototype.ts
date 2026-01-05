/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';
import { Entry } from '@moryx/ngx-web-framework';


export interface Prototype$Params {
}

export function prototype(http: HttpClient, rootUrl: string, params?: Prototype$Params, context?: HttpContext): Observable<StrictHttpResponse<Entry>> {
  const rb = new RequestBuilder(rootUrl, prototype.PATH, 'get');
  if (params) {
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

prototype.PATH = '/api/moryx/maintenances/prototype';
