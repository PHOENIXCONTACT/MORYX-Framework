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

import { ContentDescriptorModel } from '../models/content-descriptor-model';
import { VariantDescriptor } from '../models/variant-descriptor';

@Injectable({
  providedIn: 'root',
})
export class MediaServerService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getAll
   */
  static readonly GetAllPath = '/api/moryx/media';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll$Response(params?: {
  }): Observable<StrictHttpResponse<Array<ContentDescriptorModel>>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.GetAllPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ContentDescriptorModel>>;
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
  }): Observable<Array<ContentDescriptorModel>> {

    return this.getAll$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ContentDescriptorModel>>) => r.body as Array<ContentDescriptorModel>)
    );
  }

  /**
   * Path part for operation get
   */
  static readonly GetPath = '/api/moryx/media/{guid}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `get()` instead.
   *
   * This method doesn't expect any request body.
   */
  get$Response(params: {
    guid: string;
  }): Observable<StrictHttpResponse<ContentDescriptorModel>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.GetPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ContentDescriptorModel>;
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
  }): Observable<ContentDescriptorModel> {

    return this.get$Response(params).pipe(
      map((r: StrictHttpResponse<ContentDescriptorModel>) => r.body as ContentDescriptorModel)
    );
  }

  /**
   * Path part for operation removeContent
   */
  static readonly RemoveContentPath = '/api/moryx/media/{guid}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `removeContent()` instead.
   *
   * This method doesn't expect any request body.
   */
  removeContent$Response(params: {
    guid: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.RemoveContentPath, 'delete');
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
   * To access the full response (for headers, for example), `removeContent$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  removeContent(params: {
    guid: string;
  }): Observable<void> {

    return this.removeContent$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getVariant
   */
  static readonly GetVariantPath = '/api/moryx/media/{guid}/{variantName}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getVariant()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVariant$Response(params: {
    guid: string;
    variantName: string;
  }): Observable<StrictHttpResponse<VariantDescriptor>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.GetVariantPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
      rb.path('variantName', params.variantName, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<VariantDescriptor>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getVariant$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVariant(params: {
    guid: string;
    variantName: string;
  }): Observable<VariantDescriptor> {

    return this.getVariant$Response(params).pipe(
      map((r: StrictHttpResponse<VariantDescriptor>) => r.body as VariantDescriptor)
    );
  }

  /**
   * Path part for operation removeVariant
   */
  static readonly RemoveVariantPath = '/api/moryx/media/{guid}/{variantName}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `removeVariant()` instead.
   *
   * This method doesn't expect any request body.
   */
  removeVariant$Response(params: {
    guid: string;
    variantName: string;
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.RemoveVariantPath, 'delete');
    if (params) {
      rb.path('guid', params.guid, {});
      rb.path('variantName', params.variantName, {});
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
   * To access the full response (for headers, for example), `removeVariant$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  removeVariant(params: {
    guid: string;
    variantName: string;
  }): Observable<void> {

    return this.removeVariant$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getVariantStream
   */
  static readonly GetVariantStreamPath = '/api/moryx/media/{guid}/{variantName}/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getVariantStream$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVariantStream$Plain$Response(params: {
    guid: string;
    variantName: string;
    preview?: boolean;
  }): Observable<StrictHttpResponse<Blob>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.GetVariantStreamPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
      rb.path('variantName', params.variantName, {});
      rb.query('preview', params.preview, {});
    }

    return this.http.request(rb.build({
      responseType: 'blob',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Blob>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getVariantStream$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVariantStream$Plain(params: {
    guid: string;
    variantName: string;
    preview?: boolean;
  }): Observable<Blob> {

    return this.getVariantStream$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<Blob>) => r.body as Blob)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getVariantStream$Json()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVariantStream$Json$Response(params: {
    guid: string;
    variantName: string;
    preview?: boolean;
  }): Observable<StrictHttpResponse<Blob>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.GetVariantStreamPath, 'get');
    if (params) {
      rb.path('guid', params.guid, {});
      rb.path('variantName', params.variantName, {});
      rb.query('preview', params.preview, {});
    }

    return this.http.request(rb.build({
      responseType: 'blob',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Blob>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getVariantStream$Json$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getVariantStream$Json(params: {
    guid: string;
    variantName: string;
    preview?: boolean;
  }): Observable<Blob> {

    return this.getVariantStream$Json$Response(params).pipe(
      map((r: StrictHttpResponse<Blob>) => r.body as Blob)
    );
  }

  /**
   * Path part for operation addMaster
   */
  static readonly AddMasterPath = '/api/moryx/media/master';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `addMaster()` instead.
   *
   * This method sends `multipart/form-data` and handles request body of type `multipart/form-data`.
   */
  addMaster$Response(params?: {
    body?: {
'formFile'?: Blob;
}
  }): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.AddMasterPath, 'post');
    if (params) {
      rb.body(params.body, 'multipart/form-data');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `addMaster$Response()` instead.
   *
   * This method sends `multipart/form-data` and handles request body of type `multipart/form-data`.
   */
  addMaster(params?: {
    body?: {
'formFile'?: Blob;
}
  }): Observable<string> {

    return this.addMaster$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

  /**
   * Path part for operation addVariant
   */
  static readonly AddVariantPath = '/api/moryx/media/{contentId}/{variantName}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `addVariant()` instead.
   *
   * This method sends `multipart/form-data` and handles request body of type `multipart/form-data`.
   */
  addVariant$Response(params: {
    contentId: string;
    variantName: string;
    body?: {
'formFile'?: Blob;
}
  }): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, MediaServerService.AddVariantPath, 'post');
    if (params) {
      rb.path('contentId', params.contentId, {});
      rb.path('variantName', params.variantName, {});
      rb.body(params.body, 'multipart/form-data');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<string>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `addVariant$Response()` instead.
   *
   * This method sends `multipart/form-data` and handles request body of type `multipart/form-data`.
   */
  addVariant(params: {
    contentId: string;
    variantName: string;
    body?: {
'formFile'?: Blob;
}
  }): Observable<string> {

    return this.addVariant$Response(params).pipe(
      map((r: StrictHttpResponse<string>) => r.body as string)
    );
  }

}
