/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { add } from '../fn/operator-management/add';
import { Add$Params } from '../fn/operator-management/add';
import { deleteOperator } from '../fn/operator-management/delete-operator';
import { DeleteOperator$Params } from '../fn/operator-management/delete-operator';
import { get_2 } from '../fn/operator-management/get-2';
import { Get_2$Params } from '../fn/operator-management/get-2';
import { getAll_5 } from '../fn/operator-management/get-all-5';
import { GetAll_5$Params } from '../fn/operator-management/get-all-5';
import { getDefaultOperator } from '../fn/operator-management/get-default-operator';
import { GetDefaultOperator$Params } from '../fn/operator-management/get-default-operator';
import { getOperatorsByResource } from '../fn/operator-management/get-operators-by-resource';
import { GetOperatorsByResource$Params } from '../fn/operator-management/get-operators-by-resource';
import { getResources } from '../fn/operator-management/get-resources';
import { GetResources$Params } from '../fn/operator-management/get-resources';
import { getResourcesByOperator } from '../fn/operator-management/get-resources-by-operator';
import { GetResourcesByOperator$Params } from '../fn/operator-management/get-resources-by-operator';
import { AssignableOperator as MoryxOperatorsAssignableOperator } from '../models/Moryx/Operators/assignable-operator';
import { ExtendedOperatorModel as MoryxOperatorsEndpointsExtendedOperatorModel } from '../models/Moryx/Operators/Endpoints/extended-operator-model';
import { ResourceModel as MoryxOperatorsEndpointsResourceModel } from '../models/Moryx/Operators/Endpoints/resource-model';
import { signIn } from '../fn/operator-management/sign-in';
import { SignIn$Params } from '../fn/operator-management/sign-in';
import { signOut } from '../fn/operator-management/sign-out';
import { SignOut$Params } from '../fn/operator-management/sign-out';
import { update_1 } from '../fn/operator-management/update-1';
import { Update_1$Params } from '../fn/operator-management/update-1';

@Injectable({ providedIn: 'root' })
export class OperatorManagementService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `getAll_5()` */
  static readonly GetAll_5Path = '/api/moryx/operators';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll_5()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll_5$Response(params?: GetAll_5$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsAssignableOperator>>> {
    return getAll_5(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getAll_5$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll_5(params?: GetAll_5$Params, context?: HttpContext): Observable<Array<MoryxOperatorsAssignableOperator>> {
    return this.getAll_5$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxOperatorsAssignableOperator>>): Array<MoryxOperatorsAssignableOperator> => r.body)
    );
  }

  /** Path part for operation `add()` */
  static readonly AddPath = '/api/moryx/operators';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `add()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  add$Response(params?: Add$Params, context?: HttpContext): Observable<StrictHttpResponse<string>> {
    return add(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `add$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  add(params?: Add$Params, context?: HttpContext): Observable<string> {
    return this.add$Response(params, context).pipe(
      map((r: StrictHttpResponse<string>): string => r.body)
    );
  }

  /** Path part for operation `deleteOperator()` */
  static readonly DeleteOperatorPath = '/api/moryx/operators';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteOperator()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteOperator$Response(params?: DeleteOperator$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return deleteOperator(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `deleteOperator$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteOperator(params?: DeleteOperator$Params, context?: HttpContext): Observable<void> {
    return this.deleteOperator$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `get_2()` */
  static readonly Get_2Path = '/api/moryx/operators/{identifier}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `get_2()` instead.
   *
   * This method doesn't expect any request body.
   */
  get_2$Response(params: Get_2$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsExtendedOperatorModel>> {
    return get_2(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `get_2$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  get_2(params: Get_2$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsExtendedOperatorModel> {
    return this.get_2$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsExtendedOperatorModel>): MoryxOperatorsEndpointsExtendedOperatorModel => r.body)
    );
  }

  /** Path part for operation `update_1()` */
  static readonly Update_1Path = '/api/moryx/operators/{identifier}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update_1()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  update_1$Response(params: Update_1$Params, context?: HttpContext): Observable<StrictHttpResponse<string>> {
    return update_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `update_1$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  update_1(params: Update_1$Params, context?: HttpContext): Observable<string> {
    return this.update_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<string>): string => r.body)
    );
  }

  /** Path part for operation `getOperatorsByResource()` */
  static readonly GetOperatorsByResourcePath = '/api/moryx/operators/get-operators-by-resource/{resourceId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getOperatorsByResource()` instead.
   *
   * This method doesn't expect any request body.
   */
  getOperatorsByResource$Response(params: GetOperatorsByResource$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsEndpointsExtendedOperatorModel>>> {
    return getOperatorsByResource(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getOperatorsByResource$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getOperatorsByResource(params: GetOperatorsByResource$Params, context?: HttpContext): Observable<Array<MoryxOperatorsEndpointsExtendedOperatorModel>> {
    return this.getOperatorsByResource$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxOperatorsEndpointsExtendedOperatorModel>>): Array<MoryxOperatorsEndpointsExtendedOperatorModel> => r.body)
    );
  }

  /** Path part for operation `getResourcesByOperator()` */
  static readonly GetResourcesByOperatorPath = '/api/moryx/operators/{identifier}/resources';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getResourcesByOperator()` instead.
   *
   * This method doesn't expect any request body.
   */
  getResourcesByOperator$Response(params: GetResourcesByOperator$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsEndpointsResourceModel>>> {
    return getResourcesByOperator(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getResourcesByOperator$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getResourcesByOperator(params: GetResourcesByOperator$Params, context?: HttpContext): Observable<Array<MoryxOperatorsEndpointsResourceModel>> {
    return this.getResourcesByOperator$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxOperatorsEndpointsResourceModel>>): Array<MoryxOperatorsEndpointsResourceModel> => r.body)
    );
  }

  /** Path part for operation `getDefaultOperator()` */
  static readonly GetDefaultOperatorPath = '/api/moryx/operators/default';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getDefaultOperator()` instead.
   *
   * This method doesn't expect any request body.
   */
  getDefaultOperator$Response(params?: GetDefaultOperator$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxOperatorsEndpointsExtendedOperatorModel>> {
    return getDefaultOperator(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getDefaultOperator$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getDefaultOperator(params?: GetDefaultOperator$Params, context?: HttpContext): Observable<MoryxOperatorsEndpointsExtendedOperatorModel> {
    return this.getDefaultOperator$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxOperatorsEndpointsExtendedOperatorModel>): MoryxOperatorsEndpointsExtendedOperatorModel => r.body)
    );
  }

  /** Path part for operation `signIn()` */
  static readonly SignInPath = '/api/moryx/operators/signin';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `signIn()` instead.
   *
   * This method doesn't expect any request body.
   */
  signIn$Response(params?: SignIn$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return signIn(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `signIn$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  signIn(params?: SignIn$Params, context?: HttpContext): Observable<void> {
    return this.signIn$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `signOut()` */
  static readonly SignOutPath = '/api/moryx/operators/signout';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `signOut()` instead.
   *
   * This method doesn't expect any request body.
   */
  signOut$Response(params?: SignOut$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return signOut(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `signOut$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  signOut(params?: SignOut$Params, context?: HttpContext): Observable<void> {
    return this.signOut$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `getResources()` */
  static readonly GetResourcesPath = '/api/moryx/operators/resources';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getResources()` instead.
   *
   * This method doesn't expect any request body.
   */
  getResources$Response(params?: GetResources$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxOperatorsEndpointsResourceModel>>> {
    return getResources(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getResources$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getResources(params?: GetResources$Params, context?: HttpContext): Observable<Array<MoryxOperatorsEndpointsResourceModel>> {
    return this.getResources$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxOperatorsEndpointsResourceModel>>): Array<MoryxOperatorsEndpointsResourceModel> => r.body)
    );
  }

}
