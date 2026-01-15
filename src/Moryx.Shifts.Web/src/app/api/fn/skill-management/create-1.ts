/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { SkillCreationContextModel } from '../../models/skill-creation-context-model';
import { SkillModel } from '../../models/skill-model';

export interface Create_1$Params {
      body?: SkillCreationContextModel
}

export function create_1(http: HttpClient, rootUrl: string, params?: Create_1$Params, context?: HttpContext): Observable<StrictHttpResponse<SkillModel>> {
  const rb = new RequestBuilder(rootUrl, create_1.PATH, 'post');
  if (params) {
    rb.body(params.body, 'application/*+json');
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

create_1.PATH = '/api/moryx/skills';

