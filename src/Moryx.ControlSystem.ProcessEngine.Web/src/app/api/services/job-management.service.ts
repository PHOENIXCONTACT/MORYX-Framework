/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { ApiConfiguration } from '../api-configuration';
import { BaseService } from '../base-service';
import { RequestBuilder } from '../request-builder';
import { StrictHttpResponse } from '../strict-http-response';

import { JobModel } from '../models/job-model';

@Injectable({
  providedIn: 'root',
})
export class JobManagementService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getJob
   */
  static readonly GetJobPath = '/api/moryx/jobs/{jobId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getJob()` instead.
   *
   * This method doesn't expect any request body.
   */
  getJob$Response(params: {
    jobId: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<JobModel>> {

    const rb = new RequestBuilder(this.rootUrl, JobManagementService.GetJobPath, 'get');
    if (params) {
      rb.path('jobId', params.jobId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<JobModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getJob$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getJob(params: {
    jobId: number;
    context?: HttpContext
  }
): Observable<JobModel> {

    return this.getJob$Response(params).pipe(
      map((r: StrictHttpResponse<JobModel>) => r.body as JobModel)
    );
  }

  /**
   * Path part for operation getAll
   */
  static readonly getAllPath = '/api/moryx/jobs';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<JobModel>>> {

    const rb = new RequestBuilder(this.rootUrl, JobManagementService.getAllPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<JobModel>>;
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
    context?: HttpContext
  }
): Observable<Array<JobModel>> {

    return this.getAll$Response(params).pipe(
      map((r: StrictHttpResponse<Array<JobModel>>) => r.body as Array<JobModel>)
    );
  }

  /**
   * Path part for operation complete
   */
  static readonly CompletePath = '/api/moryx/jobs/{jobId}/complete';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `complete()` instead.
   *
   * This method doesn't expect any request body.
   */
  complete$Response(params: {
    jobId: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, JobManagementService.CompletePath, 'post');
    if (params) {
      rb.path('jobId', params.jobId, {});
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
   * To access the full response (for headers, for example), `complete$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  complete(params: {
    jobId: number;
    context?: HttpContext
  }
): Observable<void> {

    return this.complete$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation abort
   */
  static readonly AbortPath = '/api/moryx/jobs/{jobId}/abort';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `abort()` instead.
   *
   * This method doesn't expect any request body.
   */
  abort$Response(params: {
    jobId: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, JobManagementService.AbortPath, 'post');
    if (params) {
      rb.path('jobId', params.jobId, {});
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
   * To access the full response (for headers, for example), `abort$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  abort(params: {
    jobId: number;
    context?: HttpContext
  }
): Observable<void> {

    return this.abort$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation progressStream
   */
  static readonly ProgressStreamPath = '/api/moryx/jobs/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `progressStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  progressStream$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, JobManagementService.ProgressStreamPath, 'get');
    if (params) {
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
   * To access the full response (for headers, for example), `progressStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  progressStream(params?: {
    context?: HttpContext
  }
): Observable<void> {

    return this.progressStream$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
