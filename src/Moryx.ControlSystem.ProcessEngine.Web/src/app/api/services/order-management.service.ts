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

import { AdviceContext } from '../models/advice-context';
import { AdviceModel } from '../models/advice-model';
import { BeginContext } from '../models/begin-context';
import { BeginModel } from '../models/begin-model';
import { DocumentModel } from '../models/document-model';
import { OperationCreationContextModel } from '../models/operation-creation-context-model';
import { OperationLogMessageModel } from '../models/operation-log-message-model';
import { OperationModel } from '../models/operation-model';
import { OperationRecipeModel } from '../models/operation-recipe-model';
import { ProductPartModel } from '../models/product-part-model';
import { ReportContext } from '../models/report-context';
import { ReportModel } from '../models/report-model';

@Injectable({
  providedIn: 'root',
})
export class OrderManagementService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getOperations
   */
  static readonly GetOperationsPath = '/api/moryx/orders';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getOperations()` instead.
   *
   * This method doesn't expect any request body.
   */
  getOperations$Response(params?: {
    orderNumber?: string;
    operationNumber?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<OperationModel>>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetOperationsPath, 'get');
    if (params) {
      rb.query('orderNumber', params.orderNumber, {});
      rb.query('operationNumber', params.operationNumber, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<OperationModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getOperations$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getOperations(params?: {
    orderNumber?: string;
    operationNumber?: string;
    context?: HttpContext
  }
): Observable<Array<OperationModel>> {

    return this.getOperations$Response(params).pipe(
      map((r: StrictHttpResponse<Array<OperationModel>>) => r.body as Array<OperationModel>)
    );
  }

  /**
   * Path part for operation addOperation
   */
  static readonly AddOperationPath = '/api/moryx/orders';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `addOperation()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addOperation$Response(params?: {
    sourceId?: string;
    context?: HttpContext
    body?: OperationCreationContextModel
  }
): Observable<StrictHttpResponse<OperationModel>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.AddOperationPath, 'post');
    if (params) {
      rb.query('sourceId', params.sourceId, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<OperationModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `addOperation$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addOperation(params?: {
    sourceId?: string;
    context?: HttpContext
    body?: OperationCreationContextModel
  }
): Observable<OperationModel> {

    return this.addOperation$Response(params).pipe(
      map((r: StrictHttpResponse<OperationModel>) => r.body as OperationModel)
    );
  }

  /**
   * Path part for operation getOperation
   */
  static readonly GetOperationPath = '/api/moryx/orders/{guid}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getOperation()` instead.
   *
   * This method doesn't expect any request body.
   */
  getOperation$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<OperationModel>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetOperationPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<OperationModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getOperation$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getOperation(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<OperationModel> {

    return this.getOperation$Response(params).pipe(
      map((r: StrictHttpResponse<OperationModel>) => r.body as OperationModel)
    );
  }

  /**
   * Path part for operation getDocuments
   */
  static readonly GetDocumentsPath = '/api/moryx/orders/{guid}/documents';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getDocuments()` instead.
   *
   * This method doesn't expect any request body.
   */
  getDocuments$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<DocumentModel>>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetDocumentsPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<DocumentModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getDocuments$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getDocuments(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<Array<DocumentModel>> {

    return this.getDocuments$Response(params).pipe(
      map((r: StrictHttpResponse<Array<DocumentModel>>) => r.body as Array<DocumentModel>)
    );
  }

  /**
   * Path part for operation getDocumentStream
   */
  static readonly GetDocumentStreamPath = '/api/moryx/orders/{guid}/document/{identifier}/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getDocumentStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  getDocumentStream$Response(params: {
    guid: string;
    identifier: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Blob>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetDocumentStreamPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
      rb.path('identifier', params.identifier, {});
    }

    return this.http.request(rb.build({
      responseType: 'blob',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Blob>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getDocumentStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getDocumentStream(params: {
    guid: string;
    identifier: string;
    context?: HttpContext
  }
): Observable<Blob> {

    return this.getDocumentStream$Response(params).pipe(
      map((r: StrictHttpResponse<Blob>) => r.body as Blob)
    );
  }

  /**
   * Path part for operation getProductParts
   */
  static readonly GetProductPartsPath = '/api/moryx/orders/{guid}/productparts';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProductParts()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProductParts$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<ProductPartModel>>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetProductPartsPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ProductPartModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getProductParts$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProductParts(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<Array<ProductPartModel>> {

    return this.getProductParts$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ProductPartModel>>) => r.body as Array<ProductPartModel>)
    );
  }

  /**
   * Path part for operation getBeginContext
   */
  static readonly GetBeginContextPath = '/api/moryx/orders/{guid}/begin';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getBeginContext()` instead.
   *
   * This method doesn't expect any request body.
   */
  getBeginContext$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<BeginContext>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetBeginContextPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<BeginContext>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getBeginContext$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getBeginContext(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<BeginContext> {

    return this.getBeginContext$Response(params).pipe(
      map((r: StrictHttpResponse<BeginContext>) => r.body as BeginContext)
    );
  }

  /**
   * Path part for operation beginOperation
   */
  static readonly BeginOperationPath = '/api/moryx/orders/{guid}/begin';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `beginOperation()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  beginOperation$Response(params: {
    guid: string;
    context?: HttpContext
    body?: BeginModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.BeginOperationPath, 'post');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `beginOperation$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  beginOperation(params: {
    guid: string;
    context?: HttpContext
    body?: BeginModel
  }
): Observable<void> {

    return this.beginOperation$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getReportContext
   */
  static readonly GetReportContextPath = '/api/moryx/orders/{guid}/report';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getReportContext()` instead.
   *
   * This method doesn't expect any request body.
   */
  getReportContext$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<ReportContext>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetReportContextPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ReportContext>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getReportContext$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getReportContext(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<ReportContext> {

    return this.getReportContext$Response(params).pipe(
      map((r: StrictHttpResponse<ReportContext>) => r.body as ReportContext)
    );
  }

  /**
   * Path part for operation reportOperation
   */
  static readonly ReportOperationPath = '/api/moryx/orders/{guid}/report';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `reportOperation()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  reportOperation$Response(params: {
    guid: string;
    context?: HttpContext
    body?: ReportModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.ReportOperationPath, 'post');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `reportOperation$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  reportOperation(params: {
    guid: string;
    context?: HttpContext
    body?: ReportModel
  }
): Observable<void> {

    return this.reportOperation$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getInterruptContext
   */
  static readonly GetInterruptContextPath = '/api/moryx/orders/{guid}/interrupt';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getInterruptContext()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInterruptContext$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<ReportContext>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetInterruptContextPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ReportContext>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getInterruptContext$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInterruptContext(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<ReportContext> {

    return this.getInterruptContext$Response(params).pipe(
      map((r: StrictHttpResponse<ReportContext>) => r.body as ReportContext)
    );
  }

  /**
   * Path part for operation interruptOperation
   */
  static readonly InterruptOperationPath = '/api/moryx/orders/{guid}/interrupt';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `interruptOperation()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  interruptOperation$Response(params: {
    guid: string;
    context?: HttpContext
    body?: ReportModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.InterruptOperationPath, 'post');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `interruptOperation$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  interruptOperation(params: {
    guid: string;
    context?: HttpContext
    body?: ReportModel
  }
): Observable<void> {

    return this.interruptOperation$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getAdviceContext
   */
  static readonly GetAdviceContextPath = '/api/moryx/orders/{guid}/advice';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAdviceContext()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAdviceContext$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<AdviceContext>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetAdviceContextPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<AdviceContext>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAdviceContext$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAdviceContext(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<AdviceContext> {

    return this.getAdviceContext$Response(params).pipe(
      map((r: StrictHttpResponse<AdviceContext>) => r.body as AdviceContext)
    );
  }

  /**
   * Path part for operation adviceOperation
   */
  static readonly AdviceOperationPath = '/api/moryx/orders/{guid}/advice';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `adviceOperation()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  adviceOperation$Response(params: {
    guid: string;
    context?: HttpContext
    body?: AdviceModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.AdviceOperationPath, 'post');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `adviceOperation$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  adviceOperation(params: {
    guid: string;
    context?: HttpContext
    body?: AdviceModel
  }
): Observable<void> {

    return this.adviceOperation$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getLogs
   */
  static readonly GetLogsPath = '/api/moryx/orders/{guid}/logs';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getLogs()` instead.
   *
   * This method doesn't expect any request body.
   */
  getLogs$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<OperationLogMessageModel>>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetLogsPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<OperationLogMessageModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getLogs$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getLogs(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<Array<OperationLogMessageModel>> {

    return this.getLogs$Response(params).pipe(
      map((r: StrictHttpResponse<Array<OperationLogMessageModel>>) => r.body as Array<OperationLogMessageModel>)
    );
  }

  /**
   * Path part for operation getAssignableRecipes
   */
  static readonly GetAssignableRecipesPath = '/api/moryx/orders/recipes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAssignableRecipes()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAssignableRecipes$Response(params?: {
    identifier?: string;
    revision?: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<OperationRecipeModel>>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.GetAssignableRecipesPath, 'get');
    if (params) {
      rb.query('identifier', params.identifier, {});
      rb.query('revision', params.revision, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<OperationRecipeModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getAssignableRecipes$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAssignableRecipes(params?: {
    identifier?: string;
    revision?: number;
    context?: HttpContext
  }
): Observable<Array<OperationRecipeModel>> {

    return this.getAssignableRecipes$Response(params).pipe(
      map((r: StrictHttpResponse<Array<OperationRecipeModel>>) => r.body as Array<OperationRecipeModel>)
    );
  }

  /**
   * Path part for operation abortOperation
   */
  static readonly AbortOperationPath = '/api/moryx/orders/{guid}/abort';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `abortOperation()` instead.
   *
   * This method doesn't expect any request body.
   */
  abortOperation$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.AbortOperationPath, 'post');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `abortOperation$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  abortOperation(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<void> {

    return this.abortOperation$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation setOperationSortOrder
   */
  static readonly SetOperationSortOrderPath = '/api/moryx/orders/{guid}/position';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `setOperationSortOrder()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  setOperationSortOrder$Response(params: {
    guid: string;
    context?: HttpContext
    body?: number
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.SetOperationSortOrderPath, 'put');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `setOperationSortOrder$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  setOperationSortOrder(params: {
    guid: string;
    context?: HttpContext
    body?: number
  }
): Observable<void> {

    return this.setOperationSortOrder$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation reload
   */
  static readonly ReloadPath = '/api/moryx/orders/{guid}/reload';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `reload()` instead.
   *
   * This method doesn't expect any request body.
   */
  reload$Response(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, OrderManagementService.ReloadPath, 'put');
    if (params) {
      rb.path('guid', params.guid, {});
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
   * To access the full response (for headers, for example), `reload$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  reload(params: {
    guid: string;
    context?: HttpContext
  }
): Observable<void> {

    return this.reload$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
