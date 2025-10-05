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

import { Entry } from '@moryx/ngx-web-framework';
import { ProductCustomization } from '../models/product-customization';
import { ProductInstanceModel } from '../models/product-instance-model';
import { ProductModel } from '../models/product-model';
import { ProductQuery } from '../models/product-query';
import { RecipeModel } from '../models/recipe-model';

@Injectable({
  providedIn: 'root',
})
export class ProductManagementService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation getProductCustomization
   */
  static readonly GetProductCustomizationPath = '/api/moryx/products/configuration';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getProductCustomization()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProductCustomization$Response(params?: {
    context?: HttpContext
  }
): Observable<StrictHttpResponse<ProductCustomization>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetProductCustomizationPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ProductCustomization>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getProductCustomization$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getProductCustomization(params?: {
    context?: HttpContext
  }
): Observable<ProductCustomization> {

    return this.getProductCustomization$Response(params).pipe(
      map((r: StrictHttpResponse<ProductCustomization>) => r.body as ProductCustomization)
    );
  }

  /**
   * Path part for operation import
   */
  static readonly ImportPath = '/api/moryx/products/importers/{importerName}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `import()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  import$Response(params: {
    importerName: string;
    context?: HttpContext
    body?: Entry
  }
): Observable<StrictHttpResponse<Array<ProductModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.ImportPath, 'post');
    if (params) {
      rb.path('importerName', params.importerName, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ProductModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `import$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  import(params: {
    importerName: string;
    context?: HttpContext
    body?: Entry
  }
): Observable<Array<ProductModel>> {

    return this.import$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ProductModel>>) => r.body as Array<ProductModel>)
    );
  }

  /**
   * Path part for operation getTypeByIdentity
   */
  static readonly GetTypeByIdentityPath = '/api/moryx/products/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTypeByIdentity()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypeByIdentity$Response(params?: {
    identity?: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<ProductModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetTypeByIdentityPath, 'get');
    if (params) {
      rb.query('identity', params.identity, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ProductModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTypeByIdentity$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypeByIdentity(params?: {
    identity?: string;
    context?: HttpContext
  }
): Observable<Array<ProductModel>> {

    return this.getTypeByIdentity$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ProductModel>>) => r.body as Array<ProductModel>)
    );
  }

  /**
   * Path part for operation saveType
   */
  static readonly SaveTypePath = '/api/moryx/products/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `saveType()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveType$Response(params?: {
    context?: HttpContext
    body?: ProductModel
  }
): Observable<StrictHttpResponse<number>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.SaveTypePath, 'post');
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
        return (r as HttpResponse<any>).clone({ body: parseFloat(String((r as HttpResponse<any>).body)) }) as StrictHttpResponse<number>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `saveType$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveType(params?: {
    context?: HttpContext
    body?: ProductModel
  }
): Observable<number> {

    return this.saveType$Response(params).pipe(
      map((r: StrictHttpResponse<number>) => r.body as number)
    );
  }

  /**
   * Path part for operation getTypes
   */
  static readonly GetTypesPath = '/api/moryx/products/types/query';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTypes()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  getTypes$Response(params?: {
    context?: HttpContext
    body?: ProductQuery
  }
): Observable<StrictHttpResponse<Array<ProductModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetTypesPath, 'post');
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
        return r as StrictHttpResponse<Array<ProductModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTypes$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  getTypes(params?: {
    context?: HttpContext
    body?: ProductQuery
  }
): Observable<Array<ProductModel>> {

    return this.getTypes$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ProductModel>>) => r.body as Array<ProductModel>)
    );
  }

  /**
   * Path part for operation getTypeById
   */
  static readonly GetTypeByIdPath = '/api/moryx/products/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getTypeById()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypeById$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<ProductModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetTypeByIdPath, 'get');
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
        return r as StrictHttpResponse<ProductModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getTypeById$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getTypeById(params: {
    id: number;
    context?: HttpContext
  }
): Observable<ProductModel> {

    return this.getTypeById$Response(params).pipe(
      map((r: StrictHttpResponse<ProductModel>) => r.body as ProductModel)
    );
  }

  /**
   * Path part for operation updateType
   */
  static readonly UpdateTypePath = '/api/moryx/products/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateType()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateType$Response(params: {
    id: number;
    context?: HttpContext
    body?: ProductModel
  }
): Observable<StrictHttpResponse<number>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.UpdateTypePath, 'put');
    if (params) {
      rb.path('id', params.id, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: parseFloat(String((r as HttpResponse<any>).body)) }) as StrictHttpResponse<number>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `updateType$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateType(params: {
    id: number;
    context?: HttpContext
    body?: ProductModel
  }
): Observable<number> {

    return this.updateType$Response(params).pipe(
      map((r: StrictHttpResponse<number>) => r.body as number)
    );
  }

  /**
   * Path part for operation duplicate
   */
  static readonly DuplicatePath = '/api/moryx/products/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `duplicate()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  duplicate$Response(params: {
    id: number;
    context?: HttpContext
    body?: string
  }
): Observable<StrictHttpResponse<ProductModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.DuplicatePath, 'post');
    if (params) {
      rb.path('id', params.id, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ProductModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `duplicate$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  duplicate(params: {
    id: number;
    context?: HttpContext
    body?: string
  }
): Observable<ProductModel> {

    return this.duplicate$Response(params).pipe(
      map((r: StrictHttpResponse<ProductModel>) => r.body as ProductModel)
    );
  }

  /**
   * Path part for operation deleteType
   */
  static readonly DeleteTypePath = '/api/moryx/products/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteType()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteType$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<boolean>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.DeleteTypePath, 'delete');
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
        return (r as HttpResponse<any>).clone({ body: String((r as HttpResponse<any>).body) === 'true' }) as StrictHttpResponse<boolean>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `deleteType$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteType(params: {
    id: number;
    context?: HttpContext
  }
): Observable<boolean> {

    return this.deleteType$Response(params).pipe(
      map((r: StrictHttpResponse<boolean>) => r.body as boolean)
    );
  }

  /**
   * Path part for operation getRecipes
   */
  static readonly GetRecipesPath = '/api/moryx/products/types/{id}/recipes/{classification}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRecipes()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRecipes$Response(params: {
    id: number;
    classification: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<RecipeModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetRecipesPath, 'get');
    if (params) {
      rb.path('id', params.id, {});
      rb.path('classification', params.classification, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<RecipeModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getRecipes$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRecipes(params: {
    id: number;
    classification: number;
    context?: HttpContext
  }
): Observable<Array<RecipeModel>> {

    return this.getRecipes$Response(params).pipe(
      map((r: StrictHttpResponse<Array<RecipeModel>>) => r.body as Array<RecipeModel>)
    );
  }

  /**
   * Path part for operation getInstance
   */
  static readonly GetInstancePath = '/api/moryx/products/instances/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getInstance()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInstance$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<ProductInstanceModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetInstancePath, 'get');
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
        return r as StrictHttpResponse<ProductInstanceModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getInstance$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInstance(params: {
    id: number;
    context?: HttpContext
  }
): Observable<ProductInstanceModel> {

    return this.getInstance$Response(params).pipe(
      map((r: StrictHttpResponse<ProductInstanceModel>) => r.body as ProductInstanceModel)
    );
  }

  /**
   * Path part for operation getInstances
   */
  static readonly GetInstancesPath = '/api/moryx/products/instances';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getInstances()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInstances$Response(params?: {
    ids?: Array<number>;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<Array<ProductInstanceModel>>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetInstancesPath, 'get');
    if (params) {
      rb.query('ids', params.ids, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<ProductInstanceModel>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getInstances$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getInstances(params?: {
    ids?: Array<number>;
    context?: HttpContext
  }
): Observable<Array<ProductInstanceModel>> {

    return this.getInstances$Response(params).pipe(
      map((r: StrictHttpResponse<Array<ProductInstanceModel>>) => r.body as Array<ProductInstanceModel>)
    );
  }

  /**
   * Path part for operation saveInstance
   */
  static readonly SaveInstancePath = '/api/moryx/products/instances';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `saveInstance()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveInstance$Response(params?: {
    context?: HttpContext
    body?: ProductInstanceModel
  }
): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.SaveInstancePath, 'put');
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
   * To access the full response (for headers, for example), `saveInstance$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveInstance(params?: {
    context?: HttpContext
    body?: ProductInstanceModel
  }
): Observable<void> {

    return this.saveInstance$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation createInstance
   */
  static readonly CreateInstancePath = '/api/moryx/products/instances';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `createInstance()` instead.
   *
   * This method doesn't expect any request body.
   */
  createInstance$Response(params?: {
    identifier?: string;
    revision?: number;
    save?: boolean;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<ProductInstanceModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.CreateInstancePath, 'post');
    if (params) {
      rb.query('identifier', params.identifier, {});
      rb.query('revision', params.revision, {});
      rb.query('save', params.save, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<ProductInstanceModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `createInstance$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  createInstance(params?: {
    identifier?: string;
    revision?: number;
    save?: boolean;
    context?: HttpContext
  }
): Observable<ProductInstanceModel> {

    return this.createInstance$Response(params).pipe(
      map((r: StrictHttpResponse<ProductInstanceModel>) => r.body as ProductInstanceModel)
    );
  }

  /**
   * Path part for operation getRecipe
   */
  static readonly GetRecipePath = '/api/moryx/products/recipes/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRecipe()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRecipe$Response(params: {
    id: number;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<RecipeModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.GetRecipePath, 'get');
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
        return r as StrictHttpResponse<RecipeModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getRecipe$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRecipe(params: {
    id: number;
    context?: HttpContext
  }
): Observable<RecipeModel> {

    return this.getRecipe$Response(params).pipe(
      map((r: StrictHttpResponse<RecipeModel>) => r.body as RecipeModel)
    );
  }

  /**
   * Path part for operation updateRecipe
   */
  static readonly UpdateRecipePath = '/api/moryx/products/recipes/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateRecipe()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateRecipe$Response(params: {
    id: number;
    context?: HttpContext
    body?: RecipeModel
  }
): Observable<StrictHttpResponse<number>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.UpdateRecipePath, 'put');
    if (params) {
      rb.path('id', params.id, {});
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: parseFloat(String((r as HttpResponse<any>).body)) }) as StrictHttpResponse<number>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `updateRecipe$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateRecipe(params: {
    id: number;
    context?: HttpContext
    body?: RecipeModel
  }
): Observable<number> {

    return this.updateRecipe$Response(params).pipe(
      map((r: StrictHttpResponse<number>) => r.body as number)
    );
  }

  /**
   * Path part for operation saveRecipe
   */
  static readonly SaveRecipePath = '/api/moryx/products/recipes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `saveRecipe()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveRecipe$Response(params?: {
    context?: HttpContext
    body?: RecipeModel
  }
): Observable<StrictHttpResponse<number>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.SaveRecipePath, 'post');
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
        return (r as HttpResponse<any>).clone({ body: parseFloat(String((r as HttpResponse<any>).body)) }) as StrictHttpResponse<number>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `saveRecipe$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  saveRecipe(params?: {
    context?: HttpContext
    body?: RecipeModel
  }
): Observable<number> {

    return this.saveRecipe$Response(params).pipe(
      map((r: StrictHttpResponse<number>) => r.body as number)
    );
  }

  /**
   * Path part for operation createRecipe
   */
  static readonly CreateRecipePath = '/api/moryx/products/recipe/construct/{recipeType}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `createRecipe()` instead.
   *
   * This method doesn't expect any request body.
   */
  createRecipe$Response(params: {
    recipeType: string;
    context?: HttpContext
  }
): Observable<StrictHttpResponse<RecipeModel>> {

    const rb = new RequestBuilder(this.rootUrl, ProductManagementService.CreateRecipePath, 'get');
    if (params) {
      rb.path('recipeType', params.recipeType, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
      context: params?.context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<RecipeModel>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `createRecipe$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  createRecipe(params: {
    recipeType: string;
    context?: HttpContext
  }
): Observable<RecipeModel> {

    return this.createRecipe$Response(params).pipe(
      map((r: StrictHttpResponse<RecipeModel>) => r.body as RecipeModel)
    );
  }

}
