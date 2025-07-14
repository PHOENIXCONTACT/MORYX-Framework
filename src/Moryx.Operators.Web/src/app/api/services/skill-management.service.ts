/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { create } from '../fn/skill-management/create';
import { create_1 } from '../fn/skill-management/create-1';
import { Create_1$Params } from '../fn/skill-management/create-1';
import { Create$Params } from '../fn/skill-management/create';
import { deleteSkill } from '../fn/skill-management/delete-skill';
import { DeleteSkill$Params } from '../fn/skill-management/delete-skill';
import { deleteType_1 } from '../fn/skill-management/delete-type-1';
import { DeleteType_1$Params } from '../fn/skill-management/delete-type-1';
import { getSkill } from '../fn/skill-management/get-skill';
import { GetSkill$Params } from '../fn/skill-management/get-skill';
import { getSkills } from '../fn/skill-management/get-skills';
import { GetSkills$Params } from '../fn/skill-management/get-skills';
import { getType } from '../fn/skill-management/get-type';
import { GetType$Params } from '../fn/skill-management/get-type';
import { getTypePrototype } from '../fn/skill-management/get-type-prototype';
import { GetTypePrototype$Params } from '../fn/skill-management/get-type-prototype';
import { getTypes_1 } from '../fn/skill-management/get-types-1';
import { GetTypes_1$Params } from '../fn/skill-management/get-types-1';
import { SkillModel as MoryxOperatorsEndpointsSkillModel } from '../models/Moryx/Operators/Endpoints/skill-model';
import { SkillTypeModel as MoryxOperatorsEndpointsSkillTypeModel } from '../models/Moryx/Operators/Endpoints/skill-type-model';
import { update_3 } from '../fn/skill-management/update-3';
import { Update_3$Params } from '../fn/skill-management/update-3';

@Injectable({ providedIn: 'root' })
export class SkillManagementService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `getTypes_1()` */
  static readonly GetTypes_1Path = '/api/moryx/skills/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTypes_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypes_1$Response(params?: GetTypes_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsEndpointsSkillTypeModel>>> {
    return getTypes_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getTypes_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypes_1(params?: GetTypes_1$Params, context?: HttpContext): Observable<Array<MoryxOperatorsEndpointsSkillTypeModel>> {
    return this.getTypes_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxOperatorsEndpointsSkillTypeModel>>): Array<MoryxOperatorsEndpointsSkillTypeModel> => r.body)
    );
  }

  /** Path part for operation `update_3()` */
  static readonly Update_3Path = '/api/moryx/skills/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update_3()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  update_3$Response(params?: Update_3$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return update_3(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `update_3$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  update_3(params?: Update_3$Params, context?: HttpContext): Observable<void> {
    return this.update_3$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `create()` */
  static readonly CreatePath = '/api/moryx/skills/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `create()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  create$Response(params?: Create$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>> {
    return create(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `create$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  create(params?: Create$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsSkillTypeModel> {
    return this.create$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>): MoryxOperatorsEndpointsSkillTypeModel => r.body)
    );
  }

  /** Path part for operation `getType()` */
  static readonly GetTypePath = '/api/moryx/skills/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getType()` instead.
   *
   * This method doesn't expect any request body.
   */
  getType$Response(params: GetType$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>> {
    return getType(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getType$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getType(params: GetType$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsSkillTypeModel> {
    return this.getType$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>): MoryxOperatorsEndpointsSkillTypeModel => r.body)
    );
  }

  /** Path part for operation `deleteType_1()` */
  static readonly DeleteType_1Path = '/api/moryx/skills/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteType_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteType_1$Response(params: DeleteType_1$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return deleteType_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `deleteType_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteType_1(params: DeleteType_1$Params, context?: HttpContext): Observable<void> {
    return this.deleteType_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `getTypePrototype()` */
  static readonly GetTypePrototypePath = '/api/moryx/skills/types/prototype';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTypePrototype()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypePrototype$Response(params?: GetTypePrototype$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>> {
    return getTypePrototype(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getTypePrototype$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypePrototype(params?: GetTypePrototype$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsSkillTypeModel> {
    return this.getTypePrototype$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsSkillTypeModel>): MoryxOperatorsEndpointsSkillTypeModel => r.body)
    );
  }

  /** Path part for operation `getSkills()` */
  static readonly GetSkillsPath = '/api/moryx/skills';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getSkills()` instead.
   *
   * This method doesn't expect any request body.
   */
  getSkills$Response(params?: GetSkills$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsEndpointsSkillModel>>> {
    return getSkills(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getSkills$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getSkills(params?: GetSkills$Params, context?: HttpContext): Observable<Array<MoryxOperatorsEndpointsSkillModel>> {
    return this.getSkills$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxOperatorsEndpointsSkillModel>>): Array<MoryxOperatorsEndpointsSkillModel> => r.body)
    );
  }

  /** Path part for operation `create_1()` */
  static readonly Create_1Path = '/api/moryx/skills';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `create_1()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  create_1$Response(params?: Create_1$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillModel>> {
    return create_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `create_1$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  create_1(params?: Create_1$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsSkillModel> {
    return this.create_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsSkillModel>): MoryxOperatorsEndpointsSkillModel => r.body)
    );
  }

  /** Path part for operation `getSkill()` */
  static readonly GetSkillPath = '/api/moryx/skills/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getSkill()` instead.
   *
   * This method doesn't expect any request body.
   */
  getSkill$Response(params: GetSkill$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsSkillModel>> {
    return getSkill(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getSkill$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getSkill(params: GetSkill$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsSkillModel> {
    return this.getSkill$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsSkillModel>): MoryxOperatorsEndpointsSkillModel => r.body)
    );
  }

  /** Path part for operation `deleteSkill()` */
  static readonly DeleteSkillPath = '/api/moryx/skills/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteSkill()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteSkill$Response(params: DeleteSkill$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return deleteSkill(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `deleteSkill$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteSkill(params: DeleteSkill$Params, context?: HttpContext): Observable<void> {
    return this.deleteSkill$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
