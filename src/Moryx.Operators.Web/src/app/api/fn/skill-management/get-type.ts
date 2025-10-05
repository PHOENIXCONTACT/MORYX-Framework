/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { SkillTypeModel as MoryxOperatorsEndpointsSkillTypeModel } from '../../models/Moryx/Operators/Endpoints/skill-type-model';

export interface GetType$Params {
  id: number;
}

export function getType(http: HttpClient, rootUrl: string, params: GetType$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>> {
  const rb = new RequestBuilder(rootUrl, getType.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>;
    })
  );
}

getType.PATH = '/api/moryx/skills/types/{id}';
