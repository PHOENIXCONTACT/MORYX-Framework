/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { SkillModel } from '../../models/skill-model';

export interface GetSkill$Params {
  id: number;
}

export function getSkill(http: HttpClient, rootUrl: string, params: GetSkill$Params, context?: HttpContext): Observable<StrictHttpResponse<SkillModel>> {
  const rb = new RequestBuilder(rootUrl, getSkill.PATH, 'get');
  if (params) {
    rb.path('id', params.id, {});
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<SkillModel>;
    })
  );
}

getSkill.PATH = '/api/moryx/skills/{id}';
