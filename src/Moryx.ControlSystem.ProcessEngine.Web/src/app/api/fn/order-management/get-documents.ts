/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { DocumentModel as MoryxOrdersEndpointsDocumentModel } from '../../models/Moryx/Orders/Endpoints/document-model';

export interface GetDocuments$Params {
  guid: string;
}

export function getDocuments(http: HttpClient, rootUrl: string, params: GetDocuments$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOrdersEndpointsDocumentModel>>> {
  const rb = new RequestBuilder(rootUrl, getDocuments.PATH, 'get');
  if (params) {
    rb.path('guid', params.guid, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<MoryxOrdersEndpointsDocumentModel>>;
    })
  );
}

getDocuments.PATH = '/api/moryx/orders/{guid}/documents';
