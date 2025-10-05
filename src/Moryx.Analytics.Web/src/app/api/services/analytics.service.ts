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

import { ChangedDashboardInformation } from '../models/changed-dashboard-information';
import { DashboardInformation } from '../models/dashboard-information';

@Injectable({
  providedIn: 'root',
})
export class AnalyticsService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getAllDashboards
   */
  static readonly GetAllDashboardsPath = '/api/moryx/analytics';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAllDashboards()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllDashboards$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<DashboardInformation>>> {

    const rb = new RequestBuilder(this.rootUrl, AnalyticsService.GetAllDashboardsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<DashboardInformation>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAllDashboards$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAllDashboards(params?: {
    context?: HttpContext
  }
): Observable<Array<DashboardInformation>> {

    return this.getAllDashboards$Response(params).pipe(
      map((r: StrictHttpResponse<Array<DashboardInformation>>) => r.body as Array<DashboardInformation>)
    );
  }

  /**
   * Path part for operation editDashboard
   */
  static readonly EditDashboardPath = '/api/moryx/analytics';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `editDashboard()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  editDashboard$Response(params?: {
    name?: string;
    context?: HttpContext
    body?: ChangedDashboardInformation
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, AnalyticsService.EditDashboardPath, 'put');
    if (params) {
      rb.query('name', params.name, {});
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
   * To access the full response (for headers, for example), `editDashboard$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  editDashboard(params?: {
    name?: string;
    context?: HttpContext
    body?: ChangedDashboardInformation
  }
): Observable<void> {

    return this.editDashboard$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation addDashboard
   */
  static readonly AddDashboardPath = '/api/moryx/analytics';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `addDashboard()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addDashboard$Response(params?: {
    context?: HttpContext
    body?: DashboardInformation
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, AnalyticsService.AddDashboardPath, 'post');
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
   * To access the full response (for headers, for example), `addDashboard$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addDashboard(params?: {
    context?: HttpContext
    body?: DashboardInformation
  }
): Observable<void> {

    return this.addDashboard$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation removeDashboard
   */
  static readonly RemoveDashboardPath = '/api/moryx/analytics';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `removeDashboard()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  removeDashboard$Response(params?: {
    name?: string;
    context?: HttpContext
    body?: string
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, AnalyticsService.RemoveDashboardPath, 'delete');
    if (params) {
      rb.query('name', params.name, {});
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
   * To access the full response (for headers, for example), `removeDashboard$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  removeDashboard(params?: {
    name?: string;
    context?: HttpContext
    body?: string
  }
): Observable<void> {

    return this.removeDashboard$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
