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

import { NodeConnector } from '../models/node-connector';
import { OpenSessionRequest } from '../models/open-session-request';
import { WorkplanNodeModel } from '../models/workplan-node-model';
import { WorkplanSessionModel } from '../models/workplan-session-model';
import { WorkplanStepRecipe } from '../models/workplan-step-recipe';

@Injectable({
  providedIn: 'root',
})
export class WorkplanEditingService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation availableSteps
   */
  static readonly AvailableStepsPath = '/api/moryx/workplans/steps';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `availableSteps()` instead.
   *
   * This method doesn't expect any request body.
   */
  availableSteps$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<WorkplanStepRecipe>>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.AvailableStepsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<WorkplanStepRecipe>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `availableSteps$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  availableSteps(params?: {
    context?: HttpContext
  }
): Observable<Array<WorkplanStepRecipe>> {

    return this.availableSteps$Response(params).pipe(
      map((r: StrictHttpResponse<Array<WorkplanStepRecipe>>) => r.body as Array<WorkplanStepRecipe>)
    );
  }

  /**
   * Path part for operation editWorkplan
   */
  static readonly EditWorkplanPath = '/api/moryx/workplans/sessions';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `editWorkplan()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  editWorkplan$Response(params?: {
    context?: HttpContext
    body?: OpenSessionRequest
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.EditWorkplanPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `editWorkplan$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  editWorkplan(params?: {
    context?: HttpContext
    body?: OpenSessionRequest
  }
): Observable<WorkplanSessionModel> {

    return this.editWorkplan$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation openSession
   */
  static readonly OpenSessionPath = '/api/moryx/workplans/sessions/{sessionId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `openSession()` instead.
   *
   * This method doesn't expect any request body.
   */
  openSession$Response(params: {
    sessionId: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.OpenSessionPath, 'get');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `openSession$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  openSession(params: {
    sessionId: string;
    context?: HttpContext
  }
): Observable<WorkplanSessionModel> {

    return this.openSession$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation updateSession
   */
  static readonly UpdateSessionPath = '/api/moryx/workplans/sessions/{sessionId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateSession()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateSession$Response(params: {
    sessionId: string;
    context?: HttpContext
    body?: WorkplanSessionModel
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.UpdateSessionPath, 'put');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `updateSession$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateSession(params: {
    sessionId: string;
    context?: HttpContext
    body?: WorkplanSessionModel
  }
): Observable<WorkplanSessionModel> {

    return this.updateSession$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation closeSession
   */
  static readonly CloseSessionPath = '/api/moryx/workplans/sessions/{sessionId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `closeSession()` instead.
   *
   * This method doesn't expect any request body.
   */
  closeSession$Response(params: {
    sessionId: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.CloseSessionPath, 'delete');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
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
   * To access the full response (for headers, for example), `closeSession$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  closeSession(params: {
    sessionId: string;
    context?: HttpContext
  }
): Observable<void> {

    return this.closeSession$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation autoLayout
   */
  static readonly AutoLayoutPath = '/api/moryx/workplans/sessions/{sessionId}/autolayout';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `autoLayout()` instead.
   *
   * This method doesn't expect any request body.
   */
  autoLayout$Response(params: {
    sessionId: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.AutoLayoutPath, 'get');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `autoLayout$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  autoLayout(params: {
    sessionId: string;
    context?: HttpContext
  }
): Observable<WorkplanSessionModel> {

    return this.autoLayout$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation saveSession
   */
  static readonly SaveSessionPath = '/api/moryx/workplans/sessions/{sessionId}/save';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `saveSession()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveSession$Response(params: {
    sessionId: string;
    context?: HttpContext
    body?: WorkplanSessionModel
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.SaveSessionPath, 'post');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `saveSession$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveSession(params: {
    sessionId: string;
    context?: HttpContext
    body?: WorkplanSessionModel
  }
): Observable<WorkplanSessionModel> {

    return this.saveSession$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation addStep
   */
  static readonly AddStepPath = '/api/moryx/workplans/sessions/{sessionId}/nodes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `addStep()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addStep$Response(params: {
    sessionId: string;
    context?: HttpContext
    body?: WorkplanStepRecipe
  }
): Observable<StrictHttpResponse<WorkplanNodeModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.AddStepPath, 'post');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanNodeModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `addStep$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addStep(params: {
    sessionId: string;
    context?: HttpContext
    body?: WorkplanStepRecipe
  }
): Observable<WorkplanNodeModel> {

    return this.addStep$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanNodeModel>) => r.body as WorkplanNodeModel)
    );
  }

  /**
   * Path part for operation updateStep
   */
  static readonly UpdateStepPath = '/api/moryx/workplans/sessions/{sessionId}/nodes/{nodeId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateStep()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateStep$Response(params: {
    sessionId: string;
    nodeId: number;
    context?: HttpContext
    body?: WorkplanNodeModel
  }
): Observable<StrictHttpResponse<WorkplanNodeModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.UpdateStepPath, 'put');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.path('nodeId', params.nodeId, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanNodeModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `updateStep$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateStep(params: {
    sessionId: string;
    nodeId: number;
    context?: HttpContext
    body?: WorkplanNodeModel
  }
): Observable<WorkplanNodeModel> {

    return this.updateStep$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanNodeModel>) => r.body as WorkplanNodeModel)
    );
  }

  /**
   * Path part for operation removeNode
   */
  static readonly RemoveNodePath = '/api/moryx/workplans/sessions/{sessionId}/nodes/{nodeId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `removeNode()` instead.
   *
   * This method doesn't expect any request body.
   */
  removeNode$Response(params: {
    sessionId: string;
    nodeId: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.RemoveNodePath, 'delete');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.path('nodeId', params.nodeId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `removeNode$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  removeNode(params: {
    sessionId: string;
    nodeId: number;
    context?: HttpContext
  }
): Observable<WorkplanSessionModel> {

    return this.removeNode$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation connectStep
   */
  static readonly ConnectStepPath = '/api/moryx/workplans/sessions/{sessionId}/nodes/{targetNodeId}/{targetIndex}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `connectStep()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  connectStep$Response(params: {
    sessionId: string;
    targetNodeId: number;
    targetIndex: number;
    context?: HttpContext
    body?: NodeConnector
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.ConnectStepPath, 'post');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.path('targetNodeId', params.targetNodeId, {});
      rb.path('targetIndex', params.targetIndex, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `connectStep$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  connectStep(params: {
    sessionId: string;
    targetNodeId: number;
    targetIndex: number;
    context?: HttpContext
    body?: NodeConnector
  }
): Observable<WorkplanSessionModel> {

    return this.connectStep$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

  /**
   * Path part for operation disconnectStep
   */
  static readonly DisconnectStepPath = '/api/moryx/workplans/sessions/{sessionId}/nodes/{targetNodeId}/{targetIndex}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `disconnectStep()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  disconnectStep$Response(params: {
    sessionId: string;
    targetNodeId: number;
    targetIndex: number;
    context?: HttpContext
    body?: NodeConnector
  }
): Observable<StrictHttpResponse<WorkplanSessionModel>> {

    const rb = new RequestBuilder(this.rootUrl, WorkplanEditingService.DisconnectStepPath, 'delete');
    if (params) {
      rb.path('sessionId', params.sessionId, {});
      rb.path('targetNodeId', params.targetNodeId, {});
      rb.path('targetIndex', params.targetIndex, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<WorkplanSessionModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `disconnectStep$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  disconnectStep(params: {
    sessionId: string;
    targetNodeId: number;
    targetIndex: number;
    context?: HttpContext
    body?: NodeConnector
  }
): Observable<WorkplanSessionModel> {

    return this.disconnectStep$Response(params).pipe(
      map((r: StrictHttpResponse<WorkplanSessionModel>) => r.body as WorkplanSessionModel)
    );
  }

}
