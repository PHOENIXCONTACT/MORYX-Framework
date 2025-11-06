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

import { CellLocationModel as MoryxFactoryMonitorEndpointsModelCellLocationModel } from '../models/Moryx/FactoryMonitor/Endpoints/Model/cell-location-model';
import { CellPropertySettings as MoryxFactoryMonitorEndpointsModelCellPropertySettings } from '../models/Moryx/FactoryMonitor/Endpoints/Model/cell-property-settings';
import { CellSettingsModel as MoryxFactoryMonitorEndpointsModelCellSettingsModel } from '../models/Moryx/FactoryMonitor/Endpoints/Model/cell-settings-model';
import { FactoryStateModel as MoryxFactoryMonitorEndpointsModelFactoryStateModel } from '../models/Moryx/FactoryMonitor/Endpoints/Model/factory-state-model';
import { TransportRouteModel as MoryxFactoryMonitorEndpointsModelTransportRouteModel } from '../models/Moryx/FactoryMonitor/Endpoints/Model/transport-route-model';
import { FactoryModel as MoryxFactoryMonitorEndpointsModelsFactoryModel } from '../models/Moryx/FactoryMonitor/Endpoints/Models/factory-model';
import { OrderChangedModel as MoryxFactoryMonitorEndpointsModelsOrderChangedModel } from '../models/Moryx/FactoryMonitor/Endpoints/Models/order-changed-model';
import { VisualizableItemModel as MoryxFactoryMonitorEndpointsModelsVisualizableItemModel } from '../models/Moryx/FactoryMonitor/Endpoints/Models/visualizable-item-model';

@Injectable({
  providedIn: 'root',
})
export class FactoryMonitorService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation initialFactoryState
   */
  static readonly InitialFactoryStatePath = '/api/moryx/factory-monitor/state';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `initialFactoryState$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  initialFactoryState$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelFactoryStateModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.InitialFactoryStatePath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelFactoryStateModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `initialFactoryState$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  initialFactoryState$Plain(params?: {
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelFactoryStateModel> {

    return this.initialFactoryState$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelFactoryStateModel>) => r.body as MoryxFactoryMonitorEndpointsModelFactoryStateModel)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `initialFactoryState()` instead.
   *
   * This method doesn't expect any request body.
   */
  initialFactoryState$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelFactoryStateModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.InitialFactoryStatePath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelFactoryStateModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `initialFactoryState$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  initialFactoryState(params?: {
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelFactoryStateModel> {

    return this.initialFactoryState$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelFactoryStateModel>) => r.body as MoryxFactoryMonitorEndpointsModelFactoryStateModel)
    );
  }

  /**
   * Path part for operation allCells
   */
  static readonly AllCellsPath = '/api/moryx/factory-monitor/cells';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `allCells$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  allCells$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.AllCellsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `allCells$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  allCells$Plain(params?: {
  },
  context?: HttpContext

): Observable<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>> {

    return this.allCells$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>) => r.body as Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `allCells()` instead.
   *
   * This method doesn't expect any request body.
   */
  allCells$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.AllCellsPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `allCells$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  allCells(params?: {
  },
  context?: HttpContext

): Observable<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>> {

    return this.allCells$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>) => r.body as Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>)
    );
  }

  /**
   * Path part for operation factoryContent
   */
  static readonly FactoryContentPath = '/api/moryx/factory-monitor/factory-content/{factoryId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `factoryContent$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryContent$Plain$Response(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.FactoryContentPath, 'get');
    if (params) {
      rb.path('factoryId', params.factoryId, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `factoryContent$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryContent$Plain(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>> {

    return this.factoryContent$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>) => r.body as Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `factoryContent()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryContent$Response(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.FactoryContentPath, 'get');
    if (params) {
      rb.path('factoryId', params.factoryId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `factoryContent$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryContent(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>> {

    return this.factoryContent$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>>) => r.body as Array<MoryxFactoryMonitorEndpointsModelsVisualizableItemModel>)
    );
  }

  /**
   * Path part for operation getNavigation
   */
  static readonly GetNavigationPath = '/api/moryx/factory-monitor/navigation/{factoryId}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getNavigation$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getNavigation$Plain$Response(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsFactoryModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.GetNavigationPath, 'get');
    if (params) {
      rb.path('factoryId', params.factoryId, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsFactoryModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getNavigation$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getNavigation$Plain(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelsFactoryModel> {

    return this.getNavigation$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsFactoryModel>) => r.body as MoryxFactoryMonitorEndpointsModelsFactoryModel)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getNavigation()` instead.
   *
   * This method doesn't expect any request body.
   */
  getNavigation$Response(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsFactoryModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.GetNavigationPath, 'get');
    if (params) {
      rb.path('factoryId', params.factoryId, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsFactoryModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getNavigation$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getNavigation(params: {
    factoryId: number;
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelsFactoryModel> {

    return this.getNavigation$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsFactoryModel>) => r.body as MoryxFactoryMonitorEndpointsModelsFactoryModel)
    );
  }

  /**
   * Path part for operation factoryStatesStream
   */
  static readonly FactoryStatesStreamPath = '/api/moryx/factory-monitor/state-stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `factoryStatesStream$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryStatesStream$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsOrderChangedModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.FactoryStatesStreamPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsOrderChangedModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `factoryStatesStream$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryStatesStream$Plain(params?: {
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelsOrderChangedModel> {

    return this.factoryStatesStream$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsOrderChangedModel>) => r.body as MoryxFactoryMonitorEndpointsModelsOrderChangedModel)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `factoryStatesStream()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryStatesStream$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsOrderChangedModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.FactoryStatesStreamPath, 'get');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsOrderChangedModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `factoryStatesStream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  factoryStatesStream(params?: {
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelsOrderChangedModel> {

    return this.factoryStatesStream$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelsOrderChangedModel>) => r.body as MoryxFactoryMonitorEndpointsModelsOrderChangedModel)
    );
  }

  /**
   * Path part for operation moveCell
   */
  static readonly MoveCellPath = '/api/moryx/factory-monitor/move-cell';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `moveCell$Plain()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  moveCell$Plain$Response(params?: {
    body?: MoryxFactoryMonitorEndpointsModelCellLocationModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelCellLocationModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.MoveCellPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelCellLocationModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `moveCell$Plain$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  moveCell$Plain(params?: {
    body?: MoryxFactoryMonitorEndpointsModelCellLocationModel
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelCellLocationModel> {

    return this.moveCell$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelCellLocationModel>) => r.body as MoryxFactoryMonitorEndpointsModelCellLocationModel)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `moveCell()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  moveCell$Response(params?: {
    body?: MoryxFactoryMonitorEndpointsModelCellLocationModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<MoryxFactoryMonitorEndpointsModelCellLocationModel>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.MoveCellPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<MoryxFactoryMonitorEndpointsModelCellLocationModel>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `moveCell$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  moveCell(params?: {
    body?: MoryxFactoryMonitorEndpointsModelCellLocationModel
  },
  context?: HttpContext

): Observable<MoryxFactoryMonitorEndpointsModelCellLocationModel> {

    return this.moveCell$Response(params,context).pipe(
      map((r: StrictHttpResponse<MoryxFactoryMonitorEndpointsModelCellLocationModel>) => r.body as MoryxFactoryMonitorEndpointsModelCellLocationModel)
    );
  }

  /**
   * Path part for operation changeBackground_1
   */
  static readonly ChangeBackground_1Path = '/api/moryx/factory-monitor/background';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `changeBackground_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  changeBackground_1$Response(params?: {
    resourceId?: number;
    url?: string;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.ChangeBackground_1Path, 'post');
    if (params) {
      rb.query('resourceId', params.resourceId, {});
      rb.query('url', params.url, {});
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
   * To access the full response (for headers, for example), `changeBackground_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  changeBackground_1(params?: {
    resourceId?: number;
    url?: string;
  },
  context?: HttpContext

): Observable<void> {

    return this.changeBackground_1$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getCellPropertiesSettings
   */
  static readonly GetCellPropertiesSettingsPath = '/api/moryx/factory-monitor/cell-properties/{identifier}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getCellPropertiesSettings$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getCellPropertiesSettings$Plain$Response(params: {
    identifier: string;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<{
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
}>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.GetCellPropertiesSettingsPath, 'get');
    if (params) {
      rb.path('identifier', params.identifier, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<{
        [key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
        }>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getCellPropertiesSettings$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getCellPropertiesSettings$Plain(params: {
    identifier: string;
  },
  context?: HttpContext

): Observable<{
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
}> {

    return this.getCellPropertiesSettings$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<{
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
}>) => r.body as {
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
})
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getCellPropertiesSettings()` instead.
   *
   * This method doesn't expect any request body.
   */
  getCellPropertiesSettings$Response(params: {
    identifier: string;
  },
  context?: HttpContext

): Observable<StrictHttpResponse<{
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
}>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.GetCellPropertiesSettingsPath, 'get');
    if (params) {
      rb.path('identifier', params.identifier, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<{
        [key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
        }>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getCellPropertiesSettings$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getCellPropertiesSettings(params: {
    identifier: string;
  },
  context?: HttpContext

): Observable<{
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
}> {

    return this.getCellPropertiesSettings$Response(params,context).pipe(
      map((r: StrictHttpResponse<{
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
}>) => r.body as {
[key: string]: MoryxFactoryMonitorEndpointsModelCellPropertySettings;
})
    );
  }

  /**
   * Path part for operation traceRoute
   */
  static readonly TraceRoutePath = '/api/moryx/factory-monitor/traceroute';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `traceRoute()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  traceRoute$Response(params?: {
    body?: MoryxFactoryMonitorEndpointsModelTransportRouteModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.TraceRoutePath, 'post');
    if (params) {
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
   * To access the full response (for headers, for example), `traceRoute$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  traceRoute(params?: {
    body?: MoryxFactoryMonitorEndpointsModelTransportRouteModel
  },
  context?: HttpContext

): Observable<void> {

    return this.traceRoute$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation cellSettings
   */
  static readonly CellSettingsPath = '/api/moryx/factory-monitor/cell-settings/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `cellSettings()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  cellSettings$Response(params: {
    id: number;
    body?: MoryxFactoryMonitorEndpointsModelCellSettingsModel
  },
  context?: HttpContext

): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.CellSettingsPath, 'put');
    if (params) {
      rb.path('id', params.id, {});
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
   * To access the full response (for headers, for example), `cellSettings$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  cellSettings(params: {
    id: number;
    body?: MoryxFactoryMonitorEndpointsModelCellSettingsModel
  },
  context?: HttpContext

): Observable<void> {

    return this.cellSettings$Response(params,context).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation getRoutes
   */
  static readonly GetRoutesPath = '/api/moryx/factory-monitor/routes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRoutes$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRoutes$Plain$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.GetRoutesPath, 'post');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getRoutes$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRoutes$Plain(params?: {
  },
  context?: HttpContext

): Observable<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>> {

    return this.getRoutes$Plain$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>>) => r.body as Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getRoutes()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRoutes$Response(params?: {
  },
  context?: HttpContext

): Observable<StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>>> {

    const rb = new RequestBuilder(this.rootUrl, FactoryMonitorService.GetRoutesPath, 'post');
    if (params) {
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json',
      context: context
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>>;
      })
    );
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getRoutes$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getRoutes(params?: {
  },
  context?: HttpContext

): Observable<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>> {

    return this.getRoutes$Response(params,context).pipe(
      map((r: StrictHttpResponse<Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>>) => r.body as Array<MoryxFactoryMonitorEndpointsModelTransportRouteModel>)
    );
  }

}
