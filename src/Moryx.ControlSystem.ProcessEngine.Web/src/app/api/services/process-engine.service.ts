/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { activitiesUpdatesStream } from '../fn/process-engine/activities-updates-stream';
import { ActivitiesUpdatesStream$Params } from '../fn/process-engine/activities-updates-stream';
import { getActivities } from '../fn/process-engine/get-activities';
import { GetActivities$Params } from '../fn/process-engine/get-activities';
import { getProcess } from '../fn/process-engine/get-process';
import { GetProcess$Params } from '../fn/process-engine/get-process';
import { getProcesses } from '../fn/process-engine/get-processes';
import { GetProcesses$Params } from '../fn/process-engine/get-processes';
import { getProcessHolderGroups } from '../fn/process-engine/get-process-holder-groups';
import { GetProcessHolderGroups$Params } from '../fn/process-engine/get-process-holder-groups';
import { getRunningProcesses } from '../fn/process-engine/get-running-processes';
import { GetRunningProcesses$Params } from '../fn/process-engine/get-running-processes';
import { getRunningProcessesOfJob } from '../fn/process-engine/get-running-processes-of-job';
import { GetRunningProcessesOfJob$Params } from '../fn/process-engine/get-running-processes-of-job';
import { getTargets } from '../fn/process-engine/get-targets';
import { getTargets_1 } from '../fn/process-engine/get-targets-1';
import { GetTargets_1$Params } from '../fn/process-engine/get-targets-1';
import { GetTargets$Params } from '../fn/process-engine/get-targets';
import { groupStream } from '../fn/process-engine/group-stream';
import { GroupStream$Params } from '../fn/process-engine/group-stream';
import { ActivityResourceModel as MoryxControlSystemProcessesEndpointsActivityResourceModel } from '../models/Moryx/ControlSystem/Processes/Endpoints/activity-resource-model';
import { JobProcessModel as MoryxControlSystemProcessesEndpointsJobProcessModel } from '../models/Moryx/ControlSystem/Processes/Endpoints/job-process-model';
import { ProcessActivityModel as MoryxControlSystemProcessesEndpointsProcessActivityModel } from '../models/Moryx/ControlSystem/Processes/Endpoints/process-activity-model';
import { ProcessHolderGroupModel as MoryxControlSystemProcessesEndpointsProcessHolderGroupModel } from '../models/Moryx/ControlSystem/Processes/Endpoints/process-holder-group-model';
import { processUpdatesStream } from '../fn/process-engine/process-updates-stream';
import { ProcessUpdatesStream$Params } from '../fn/process-engine/process-updates-stream';
import { resetGroup } from '../fn/process-engine/reset-group';
import { ResetGroup$Params } from '../fn/process-engine/reset-group';
import { resetPosition } from '../fn/process-engine/reset-position';
import { ResetPosition$Params } from '../fn/process-engine/reset-position';
import { ApiResponse } from '../models/Moryx/ControlSystem/Processes/Endpoints/api-response-model';

@Injectable({ providedIn: 'root' })
export class ProcessEngineService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `getRunningProcessesOfJob()` */
  static readonly GetRunningProcessesOfJobPath = '/api/moryx/processes/job';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRunningProcessesOfJob()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcessesOfJob$Response(params?: GetRunningProcessesOfJob$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>> {
    return getRunningProcessesOfJob(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getRunningProcessesOfJob$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcessesOfJob(params?: GetRunningProcessesOfJob$Params, context?: HttpContext): Observable<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>> {
    return this.getRunningProcessesOfJob$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>): Array<MoryxControlSystemProcessesEndpointsJobProcessModel> => r.body)
    );
  }

  /** Path part for operation `getRunningProcesses()` */
  static readonly GetRunningProcessesPath = '/api/moryx/processes/running';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRunningProcesses()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcesses$Response(params?: GetRunningProcesses$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>> {
    return getRunningProcesses(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getRunningProcesses$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRunningProcesses(params?: GetRunningProcesses$Params, context?: HttpContext): Observable<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>> {
    return this.getRunningProcesses$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>): Array<MoryxControlSystemProcessesEndpointsJobProcessModel> => r.body)
    );
  }

  /** Path part for operation `getProcesses()` */
  static readonly GetProcessesPath = '/api/moryx/processes/instance/{productInstanceId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProcesses()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcesses$Response(params: GetProcesses$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>> {
    return getProcesses(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getProcesses$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcesses(params: GetProcesses$Params, context?: HttpContext): Observable<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>> {
    return this.getProcesses$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsJobProcessModel>>): Array<MoryxControlSystemProcessesEndpointsJobProcessModel> => r.body)
    );
  }

  /** Path part for operation `getProcess()` */
  static readonly GetProcessPath = '/api/moryx/processes/running/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProcess()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcess$Response(params: GetProcess$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsJobProcessModel>> {
    return getProcess(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getProcess$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcess(params: GetProcess$Params, context?: HttpContext): Observable<MoryxControlSystemProcessesEndpointsJobProcessModel> {
    return this.getProcess$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxControlSystemProcessesEndpointsJobProcessModel>): MoryxControlSystemProcessesEndpointsJobProcessModel => r.body)
    );
  }

  /** Path part for operation `getActivities()` */
  static readonly GetActivitiesPath = '/api/moryx/processes/running/{id}/activities';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getActivities()` instead.
   *
   * This method doesn't expect any request body.
   */
  getActivities$Response(params: GetActivities$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsProcessActivityModel>>> {
    return getActivities(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getActivities$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getActivities(params: GetActivities$Params, context?: HttpContext): Observable<Array<MoryxControlSystemProcessesEndpointsProcessActivityModel>> {
    return this.getActivities$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsProcessActivityModel>>): Array<MoryxControlSystemProcessesEndpointsProcessActivityModel> => r.body)
    );
  }

  /** Path part for operation `getTargets()` */
  static readonly GetTargetsPath = '/api/moryx/processes/running/{id}/targets';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTargets()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets$Response(params: GetTargets$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>> {
    return getTargets(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getTargets$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets(params: GetTargets$Params, context?: HttpContext): Observable<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>> {
    return this.getTargets$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>): Array<MoryxControlSystemProcessesEndpointsActivityResourceModel> => r.body)
    );
  }

  /** Path part for operation `getTargets_1()` */
  static readonly GetTargets_1Path = '/api/moryx/processes/running/{id}/targets/{activityId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTargets_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets_1$Response(params: GetTargets_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>> {
    return getTargets_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getTargets_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTargets_1(params: GetTargets_1$Params, context?: HttpContext): Observable<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>> {
    return this.getTargets_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<MoryxControlSystemProcessesEndpointsActivityResourceModel>>): Array<MoryxControlSystemProcessesEndpointsActivityResourceModel> => r.body)
    );
  }

  /** Path part for operation `processUpdatesStream()` */
  static readonly ProcessUpdatesStreamPath = '/api/moryx/processes/stream/processes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `processUpdatesStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  processUpdatesStream$Response(params?: ProcessUpdatesStream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsJobProcessModel>> {
    return processUpdatesStream(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `processUpdatesStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  processUpdatesStream(params?: ProcessUpdatesStream$Params, context?: HttpContext): Observable<MoryxControlSystemProcessesEndpointsJobProcessModel> {
    return this.processUpdatesStream$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxControlSystemProcessesEndpointsJobProcessModel>): MoryxControlSystemProcessesEndpointsJobProcessModel => r.body)
    );
  }

  /** Path part for operation `activitiesUpdatesStream()` */
  static readonly ActivitiesUpdatesStreamPath = '/api/moryx/processes/stream/activities';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `activitiesUpdatesStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  activitiesUpdatesStream$Response(params?: ActivitiesUpdatesStream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessActivityModel>> {
    return activitiesUpdatesStream(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `activitiesUpdatesStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  activitiesUpdatesStream(params?: ActivitiesUpdatesStream$Params, context?: HttpContext): Observable<MoryxControlSystemProcessesEndpointsProcessActivityModel> {
    return this.activitiesUpdatesStream$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessActivityModel>): MoryxControlSystemProcessesEndpointsProcessActivityModel => r.body)
    );
  }

  /** Path part for operation `getProcessHolderGroups()` */
  static readonly GetProcessHolderGroupsPath = '/api/moryx/processes/holders/groups';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProcessHolderGroups()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcessHolderGroups$Response(params?: GetProcessHolderGroups$Params, context?: HttpContext): Observable<StrictHttpResponse<ApiResponse<Array<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>>>> {
    return getProcessHolderGroups(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getProcessHolderGroups$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProcessHolderGroups(params?: GetProcessHolderGroups$Params, context?: HttpContext): Observable<ApiResponse<Array<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>>> {
    return this.getProcessHolderGroups$Response(params, context).pipe(
      map((r: StrictHttpResponse<ApiResponse<Array<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>>>): ApiResponse<Array<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>> => r.body)
    );
  }

  /** Path part for operation `groupStream()` */
  static readonly GroupStreamPath = '/api/moryx/processes/holders/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `groupStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  groupStream$Response(params?: GroupStream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>> {
    return groupStream(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `groupStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  groupStream(params?: GroupStream$Params, context?: HttpContext): Observable<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel> {
    return this.groupStream$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxControlSystemProcessesEndpointsProcessHolderGroupModel>): MoryxControlSystemProcessesEndpointsProcessHolderGroupModel => r.body)
    );
  }

  /** Path part for operation `resetGroup()` */
  static readonly ResetGroupPath = '/api/moryx/processes/holders/groups/{id}/reset';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `resetGroup()` instead.
   *
   * This method doesn't expect any request body.
   */
  resetGroup$Response(params: ResetGroup$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return resetGroup(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `resetGroup$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  resetGroup(params: ResetGroup$Params, context?: HttpContext): Observable<void> {
    return this.resetGroup$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `resetPosition()` */
  static readonly ResetPositionPath = '/api/moryx/processes/holders/positions/{id}/reset';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `resetPosition()` instead.
   *
   * This method doesn't expect any request body.
   */
  resetPosition$Response(params: ResetPosition$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return resetPosition(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `resetPosition$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  resetPosition(params: ResetPosition$Params, context?: HttpContext): Observable<void> {
    return this.resetPosition$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
