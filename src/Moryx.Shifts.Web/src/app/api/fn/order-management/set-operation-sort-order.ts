/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';


export interface SetOperationSortOrder$Params {
  guid: string;
      body?: number
}

export function setOperationSortOrder(http: HttpClient, rootUrl: string, params: SetOperationSortOrder$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
  const rb = new RequestBuilder(rootUrl, setOperationSortOrder.PATH, 'put');
  if (params) {
    rb.path('guid', params.guid, {});
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

setOperationSortOrder.PATH = '/api/moryx/orders/{guid}/position';
