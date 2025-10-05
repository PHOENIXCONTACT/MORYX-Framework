/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { abort } from '../fn/job-management/abort';
import { Abort$Params } from '../fn/job-management/abort';
import { complete } from '../fn/job-management/complete';
import { Complete$Params } from '../fn/job-management/complete';
import { getAll_1 } from '../fn/job-management/get-all-1';
import { GetAll_1$Params } from '../fn/job-management/get-all-1';
import { getJob } from '../fn/job-management/get-job';
import { GetJob$Params } from '../fn/job-management/get-job';
import { JobModel as MoryxControlSystemJobsEndpointsJobModel } from '../models/Moryx/ControlSystem/Jobs/Endpoints/job-model';
import { progressStream } from '../fn/job-management/progress-stream';
import { ProgressStream$Params } from '../fn/job-management/progress-stream';

@Injectable({ providedIn: 'root' })
export class JobManagementService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `getJob()` */
  static readonly GetJobPath = '/api/moryx/jobs/{jobId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getJob()` instead.
   *
   * This method doesn't expect any request body.
   */
  getJob$Response(params: GetJob$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemJobsEndpointsJobModel>> {
    return getJob(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getJob$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getJob(params: GetJob$Params, context?: HttpContext): Observable<MoryxControlSystemJobsEndpointsJobModel> {
    return this.getJob$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxControlSystemJobsEndpointsJobModel>): MoryxControlSystemJobsEndpointsJobModel => r.body)
    );
  }

  /** Path part for operation `getAll_1()` */
  static readonly GetAll_1Path = '/api/moryx/jobs';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll_1$Response(params?: GetAll_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemJobsEndpointsJobModel>>> {
    return getAll_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getAll_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll_1(params?: GetAll_1$Params, context?: HttpContext): Observable<Array<MoryxControlSystemJobsEndpointsJobModel>> {
    return this.getAll_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemJobsEndpointsJobModel>>): Array<MoryxControlSystemJobsEndpointsJobModel> => r.body)
    );
  }

  /** Path part for operation `complete()` */
  static readonly CompletePath = '/api/moryx/jobs/{jobId}/complete';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `complete()` instead.
   *
   * This method doesn't expect any request body.
   */
  complete$Response(params: Complete$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return complete(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `complete$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  complete(params: Complete$Params, context?: HttpContext): Observable<void> {
    return this.complete$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `abort()` */
  static readonly AbortPath = '/api/moryx/jobs/{jobId}/abort';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `abort()` instead.
   *
   * This method doesn't expect any request body.
   */
  abort$Response(params: Abort$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return abort(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `abort$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  abort(params: Abort$Params, context?: HttpContext): Observable<void> {
    return this.abort$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `progressStream()` */
  static readonly ProgressStreamPath = '/api/moryx/jobs/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `progressStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  progressStream$Response(params?: ProgressStream$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return progressStream(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `progressStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  progressStream(params?: ProgressStream$Params, context?: HttpContext): Observable<void> {
    return this.progressStream$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
