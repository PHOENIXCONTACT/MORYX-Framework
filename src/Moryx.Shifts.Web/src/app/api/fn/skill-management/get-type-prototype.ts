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

import { SkillTypeModel } from '../../models/skill-type-model';

export interface GetTypePrototype$Params {
}

export function getTypePrototype(http: HttpClient, rootUrl: string, params?: GetTypePrototype$Params, context?: HttpContext): Observable<StrictHttpResponse<SkillTypeModel>> {
  const rb = new RequestBuilder(rootUrl, getTypePrototype.PATH, 'get');
  if (params) {
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<SkillTypeModel>;
    })
  );
}

getTypePrototype.PATH = '/api/moryx/skills/types/prototype';

