/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { NotificationModel } from '../models/notification-model';

@Injectable({
  providedIn: 'root',
})
export class NotificationPublisherService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getAll
   */
  static readonly GetAllPath = '/api/moryx/notifications';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll$Response(params?: {
  }): Observable<StrictHttpResponse<Array<NotificationModel>>> {

    const rb = new RequestBuilder(this.rootUrl, NotificationPublisherService.GetAllPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<NotificationModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAll$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll(params?: {
  }): Observable<Array<NotificationModel>> {

    return this.getAll$Response(params).pipe(
      map((r: StrictHttpResponse<Array<NotificationModel>>) => r.body as Array<NotificationModel>)
    );
  }

  /**
   * Path part for operation get
   */
  static readonly GetPath = '/api/moryx/notifications/{guid}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `get()` instead.
   *
   * This method doesn't expect any request body.
   */
  get$Response(params: {
    guid: string;
  }): Observable<StrictHttpResponse<NotificationModel>> {

    const rb = new RequestBuilder(this.rootUrl, NotificationPublisherService.GetPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<NotificationModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `get$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  get(params: {
    guid: string;
  }): Observable<NotificationModel> {

    return this.get$Response(params).pipe(
      map((r: StrictHttpResponse<NotificationModel>) => r.body as NotificationModel)
    );
  }

  /**
   * Path part for operation acknowledge
   */
  static readonly AcknowledgePath = '/api/moryx/notifications/{guid}/acknowledge';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `acknowledge()` instead.
   *
   * This method doesn't expect any request body.
   */
  acknowledge$Response(params: {
    guid: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, NotificationPublisherService.AcknowledgePath, 'post');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `acknowledge$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  acknowledge(params: {
    guid: string;
  }): Observable<void> {

    return this.acknowledge$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
