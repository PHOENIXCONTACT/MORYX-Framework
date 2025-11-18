/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { SkillModel } from '../../models/skill-model';

export interface GetSkills$Params {
}

export function getSkills(http: HttpClient, rootUrl: string, params?: GetSkills$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<SkillModel>>> {
  const rb = new RequestBuilder(rootUrl, getSkills.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<Array<SkillModel>>;
    })
  );
}

getSkills.PATH = '/api/moryx/skills';
