/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpContext } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { WorkplanModel } from '../models/workplan-model';

@Injectable({
  providedIn: 'root',
})
export class WorkplanService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getAllWorkplans
   */
  static readonly GetAllWorkplansPath = '/api/moryx/workplans';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAllWorkplans()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllWorkplans$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<WorkplanModel>>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanService.GetAllWorkplansPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<WorkplanModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAllWorkplans$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllWorkplans(params?: {
    context?: HttpContext
  }
): Observable<Array<WorkplanModel>> {

    return this.getAllWorkplans$Response(params).pipe(
      map((r: StrictHttpResponse<Array<WorkplanModel>>) => r.body as Array<WorkplanModel>)
    );
  }

  /**
   * Path part for operation saveWorkplan
   */
  static readonly SaveWorkplanPath = '/api/moryx/workplans';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `saveWorkplan()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveWorkplan$Response(params?: {
    context?: HttpContext
    body?: WorkplanModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanService.SaveWorkplanPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `saveWorkplan$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveWorkplan(params?: {
    context?: HttpContext
    body?: WorkplanModel
  }
): Observable<void> {

    return this.saveWorkplan$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getVersions
   */
  static readonly GetVersionsPath = '/api/moryx/workplans/{id}/versions';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getVersions()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVersions$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<WorkplanModel>>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanService.GetVersionsPath, 'get');
    if (params) {
      rb.path('id', params.id, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<WorkplanModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getVersions$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVersions(params: {
    id: number;
    context?: HttpContext
  }
): Observable<Array<WorkplanModel>> {

    return this.getVersions$Response(params).pipe(
      map((r: StrictHttpResponse<Array<WorkplanModel>>) => r.body as Array<WorkplanModel>)
    );
  }

  /**
   * Path part for operation getWorkplan
   */
  static readonly GetWorkplanPath = '/api/moryx/workplans/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getWorkplan()` instead.
   *
   * This method doesn't expect any request body.
   */
  getWorkplan$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<WorkplanModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanService.GetWorkplanPath, 'get');
    if (params) {
      rb.path('id', params.id, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getWorkplan$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getWorkplan(params: {
    id: number;
    context?: HttpContext
  }
): Observable<WorkplanModel> {

    return this.getWorkplan$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanModel>) => r.body as WorkplanModel)
    );
  }

  /**
   * Path part for operation updateWorkplan
   */
  static readonly UpdateWorkplanPath = '/api/moryx/workplans/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateWorkplan()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateWorkplan$Response(params: {
    id: string;
    context?: HttpContext
    body?: WorkplanModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanService.UpdateWorkplanPath, 'put');
    if (params) {
      rb.path('id', params.id, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `updateWorkplan$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateWorkplan(params: {
    id: string;
    context?: HttpContext
    body?: WorkplanModel
  }
): Observable<void> {

    return this.updateWorkplan$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation deleteWorkplan
   */
  static readonly DeleteWorkplanPath = '/api/moryx/workplans/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteWorkplan()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteWorkplan$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanService.DeleteWorkplanPath, 'delete');
    if (params) {
      rb.path('id', params.id, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `deleteWorkplan$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteWorkplan(params: {
    id: number;
    context?: HttpContext
  }
): Observable<void> {

    return this.deleteWorkplan$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
