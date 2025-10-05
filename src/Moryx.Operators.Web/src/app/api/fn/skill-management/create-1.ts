/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { SkillCreationContextModel as MoryxOperatorsEndpointsSkillCreationContextModel } from '../../models/Moryx/Operators/Endpoints/skill-creation-context-model';
import { SkillModel as MoryxOperatorsEndpointsSkillModel } from '../../models/Moryx/Operators/Endpoints/skill-model';

export interface Create_1$Params {
      body?: MoryxOperatorsEndpointsSkillCreationContextModel
}

export function create_1(http: HttpClient, rootUrl: string, params?: Create_1$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillModel>> {
  const rb = new RequestBuilder(rootUrl, create_1.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<MoryxOperatorsEndpointsSkillModel>;
    })
  );
}

create_1.PATH = '/api/moryx/skills';
