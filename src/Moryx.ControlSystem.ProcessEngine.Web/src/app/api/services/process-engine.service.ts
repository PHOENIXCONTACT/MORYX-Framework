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

import { ActivityResourceModel } from '../models/activity-resource-model';
import { JobProcessModel } from '../models/job-process-model';
import { ProcessActivityModel } from '../models/process-activity-model';

@Injectable({
  providedIn: 'root',
})
export class ProcessEngineService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getRunningProcessesOfJob
   */
  static readonly GetRunningProcessesOfJobPath = '/api/moryx/processes/job';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRunningProcessesOfJob()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcessesOfJob$Response(params?: {
    jobId?: number;
    allProcesses?: boolean;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<JobProcessModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetRunningProcessesOfJobPath, 'get');
    if (params) {
      rb.query('jobId', params.jobId, {});
      rb.query('allProcesses', params.allProcesses, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<JobProcessModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getRunningProcessesOfJob$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcessesOfJob(params?: {
    jobId?: number;
    allProcesses?: boolean;
    context?: HttpContext
  }
): Observable<Array<JobProcessModel>> {

    return this.getRunningProcessesOfJob$Response(params).pipe(
      map((r: StrictHttpResponse<Array<JobProcessModel>>) => r.body as Array<JobProcessModel>)
    );
  }

  /**
   * Path part for operation getRunningProcesses
   */
  static readonly GetRunningProcessesPath = '/api/moryx/processes/running';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRunningProcesses()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcesses$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<JobProcessModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetRunningProcessesPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<JobProcessModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getRunningProcesses$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcesses(params?: {
    context?: HttpContext
  }
): Observable<Array<JobProcessModel>> {

    return this.getRunningProcesses$Response(params).pipe(
      map((r: StrictHttpResponse<Array<JobProcessModel>>) => r.body as Array<JobProcessModel>)
    );
  }

  /**
   * Path part for operation getProcesses
   */
  static readonly GetProcessesPath = '/api/moryx/processes/instance/{productInstanceId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProcesses()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcesses$Response(params: {
    productInstanceId: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<JobProcessModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetProcessesPath, 'get');
    if (params) {
      rb.path('productInstanceId', params.productInstanceId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<JobProcessModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getProcesses$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcesses(params: {
    productInstanceId: number;
    context?: HttpContext
  }
): Observable<Array<JobProcessModel>> {

    return this.getProcesses$Response(params).pipe(
      map((r: StrictHttpResponse<Array<JobProcessModel>>) => r.body as Array<JobProcessModel>)
    );
  }

  /**
   * Path part for operation getProcess
   */
  static readonly GetProcessPath = '/api/moryx/processes/running/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProcess()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcess$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<JobProcessModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetProcessPath, 'get');
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
        return r as StrictHttpResponse<JobProcessModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getProcess$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcess(params: {
    id: number;
    context?: HttpContext
  }
): Observable<JobProcessModel> {

    return this.getProcess$Response(params).pipe(
      map((r: StrictHttpResponse<JobProcessModel>) => r.body as JobProcessModel)
    );
  }

  /**
   * Path part for operation getActivities
   */
  static readonly GetActivitiesPath = '/api/moryx/processes/running/{id}/activities';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getActivities()` instead.
   *
   * This method doesn't expect any request body.
   */
  getActivities$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<ProcessActivityModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetActivitiesPath, 'get');
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
        return r as StrictHttpResponse<Array<ProcessActivityModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getActivities$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getActivities(params: {
    id: number;
    context?: HttpContext
  }
): Observable<Array<ProcessActivityModel>> {

    return this.getActivities$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ProcessActivityModel>>) => r.body as Array<ProcessActivityModel>)
    );
  }

  /**
   * Path part for operation getTargets
   */
  static readonly GetTargetsPath = '/api/moryx/processes/running/{id}/targets';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTargets()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<ActivityResourceModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetTargetsPath, 'get');
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
        return r as StrictHttpResponse<Array<ActivityResourceModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTargets$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets(params: {
    id: number;
    context?: HttpContext
  }
): Observable<Array<ActivityResourceModel>> {

    return this.getTargets$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ActivityResourceModel>>) => r.body as Array<ActivityResourceModel>)
    );
  }

  /**
   * Path part for operation getTargets_1
   */
  static readonly GetTargets_1Path = '/api/moryx/processes/running/{id}/targets/{activityId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTargets_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets_1$Response(params: {
    id: number;
    activityId: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<ActivityResourceModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.GetTargets_1Path, 'get');
    if (params) {
      rb.path('id', params.id, {});
      rb.path('activityId', params.activityId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ActivityResourceModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTargets_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets_1(params: {
    id: number;
    activityId: number;
    context?: HttpContext
  }
): Observable<Array<ActivityResourceModel>> {

    return this.getTargets_1$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ActivityResourceModel>>) => r.body as Array<ActivityResourceModel>)
    );
  }

  /**
   * Path part for operation processUpdatesStream
   */
  static readonly ProcessUpdatesStreamPath = '/api/moryx/processes/stream/processes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `processUpdatesStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  processUpdatesStream$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.ProcessUpdatesStreamPath, 'get');
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
   * To access the full response (for headers, for example), `processUpdatesStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  processUpdatesStream(params?: {
    context?: HttpContext
  }
): Observable<void> {

    return this.processUpdatesStream$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation activitiesUpdatesStream
   */
  static readonly ActivitiesUpdatesStreamPath = '/api/moryx/processes/stream/activities';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `activitiesUpdatesStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  activitiesUpdatesStream$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProcessEngineService.ActivitiesUpdatesStreamPath, 'get');
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
   * To access the full response (for headers, for example), `activitiesUpdatesStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  activitiesUpdatesStream(params?: {
    context?: HttpContext
  }
): Observable<void> {

    return this.activitiesUpdatesStream$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
