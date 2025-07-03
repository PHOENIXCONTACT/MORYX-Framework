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

import { InstructionModel } from '../models/instruction-model';
import { InstructionResponseModel } from '../models/instruction-response-model';

@Injectable({
  providedIn: 'root',
})
export class WorkerSupportService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getAll_5
   */
  static readonly GetAllPath = '/api/moryx/instructions/{identifier}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll$Response(params: {
    identifier: string;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<InstructionModel>>> {

    const rb = new RequestBuilder(this.rootUrl, WorkerSupportService.GetAllPath, 'get');
    if (params) {
      rb.path('identifier', params.identifier, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<InstructionModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getAll$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll(params: {
    identifier: string;
  },
  context?: HttpContext

): Observable<Array<InstructionModel>> {

    return this.getAll$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<InstructionModel>>) => r.body as Array<InstructionModel>)
    );
  }

  /**
   * Path part for operation addInstruction
   */
  static readonly AddInstructionPath = '/api/moryx/instructions/{identifier}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `addInstruction()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addInstruction$Response(params: {
    identifier: string;
    body?: InstructionModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkerSupportService.AddInstructionPath, 'post');
    if (params) {
      rb.path('identifier', params.identifier, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `addInstruction$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  addInstruction(params: {
    identifier: string;
    body?: InstructionModel
  },
  context?: HttpContext

): Observable<void> {

    return this.addInstruction$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation clearInstruction
   */
  static readonly ClearInstructionPath = '/api/moryx/instructions/{identifier}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `clearInstruction()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  clearInstruction$Response(params: {
    identifier: string;
    body?: InstructionModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkerSupportService.ClearInstructionPath, 'delete');
    if (params) {
      rb.path('identifier', params.identifier, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `clearInstruction$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  clearInstruction(params: {
    identifier: string;
    body?: InstructionModel
  },
  context?: HttpContext

): Observable<void> {

    return this.clearInstruction$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation completeInstruction
   */
  static readonly CompleteInstructionPath = '/api/moryx/instructions/{identifier}/response';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `completeInstruction()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  completeInstruction$Response(params: {
    identifier: string;
    body?: InstructionResponseModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkerSupportService.CompleteInstructionPath, 'put');
    if (params) {
      rb.path('identifier', params.identifier, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `completeInstruction$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  completeInstruction(params: {
    identifier: string;
    body?: InstructionResponseModel
  },
  context?: HttpContext

): Observable<void> {

    return this.completeInstruction$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getInstructors
   */
  static readonly GetInstructorsPath = '/api/moryx/instructions/instructors';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getInstructors()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInstructors$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<string>>> {

    const rb = new RequestBuilder(this.rootUrl, WorkerSupportService.GetInstructorsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<string>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getInstructors$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInstructors(params?: {
  },
  context?: HttpContext

): Observable<Array<string>> {

    return this.getInstructors$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<string>>) => r.body as Array<string>)
    );
  }

  /**
   * Path part for operation instructionStream
   */
  static readonly InstructionStreamPath = '/api/moryx/instructions/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `instructionStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  instructionStream$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, WorkerSupportService.InstructionStreamPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `instructionStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  instructionStream(params?: {
  },
  context?: HttpContext

): Observable<void> {

    return this.instructionStream$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

}
