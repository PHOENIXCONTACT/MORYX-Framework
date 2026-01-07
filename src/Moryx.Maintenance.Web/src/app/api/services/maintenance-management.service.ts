/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { acknowledge } from '../fn/maintenance-management/acknowledge';
import { Acknowledge$Params } from '../fn/maintenance-management/acknowledge';
import { add } from '../fn/maintenance-management/add';
import { Add$Params } from '../fn/maintenance-management/add';
import { delete$ } from '../fn/maintenance-management/delete';
import { Delete$Params } from '../fn/maintenance-management/delete';
import { get } from '../fn/maintenance-management/get';
import { Get$Params } from '../fn/maintenance-management/get';
import { getAll_2 } from '../fn/maintenance-management/get-all-2';
import { GetAll_2$Params } from '../fn/maintenance-management/get-all-2';
import { MaintenanceOrderDto as MoryxMaintenanceEndpointsDtosMaintenanceOrderDto } from '../models/Moryx/Maintenance/Endpoints/Dtos/maintenance-order-dto';
import { MaintenanceOrdersDto as MoryxMaintenanceEndpointsDtosMaintenanceOrdersDto } from '../models/Moryx/Maintenance/Endpoints/Dtos/maintenance-orders-dto';
import { prototype } from '../fn/maintenance-management/prototype';
import { Prototype$Params } from '../fn/maintenance-management/prototype';
import { start } from '../fn/maintenance-management/start';
import { Start$Params } from '../fn/maintenance-management/start';
import { stream } from '../fn/maintenance-management/stream';
import { Stream$Params } from '../fn/maintenance-management/stream';
import { update } from '../fn/maintenance-management/update';
import { Update$Params } from '../fn/maintenance-management/update';
import { Entry } from '@moryx/ngx-web-framework';

@Injectable({ providedIn: 'root' })
export class MaintenanceManagementService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `getAll_2()` */
  static readonly GetAll_2Path = '/api/moryx/maintenances';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getAll_2()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll_2$Response(params?: GetAll_2$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxMaintenanceEndpointsDtosMaintenanceOrdersDto>> {
    return getAll_2(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getAll_2$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getAll_2(params?: GetAll_2$Params, context?: HttpContext): Observable<MoryxMaintenanceEndpointsDtosMaintenanceOrdersDto> {
    return this.getAll_2$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxMaintenanceEndpointsDtosMaintenanceOrdersDto>): MoryxMaintenanceEndpointsDtosMaintenanceOrdersDto => r.body)
    );
  }

  /** Path part for operation `get()` */
  static readonly GetPath = '/api/moryx/maintenances/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `get()` instead.
   *
   * This method doesn't expect any request body.
   */
  get$Response(params: Get$Params, context?: HttpContext): Observable<StrictHttpResponse<Entry>> {
    return get(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `get$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  get(params: Get$Params, context?: HttpContext): Observable<Entry> {
    return this.get$Response(params, context).pipe(
      map((r: StrictHttpResponse<Entry>): Entry => r.body)
    );
  }

  /** Path part for operation `update()` */
  static readonly UpdatePath = '/api/moryx/maintenances/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  update$Response(params: Update$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return update(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `update$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  update(params: Update$Params, context?: HttpContext): Observable<void> {
    return this.update$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `delete()` */
  static readonly DeletePath = '/api/moryx/maintenances/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `delete()` instead.
   *
   * This method doesn't expect any request body.
   */
  delete$Response(params: Delete$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return delete$(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `delete$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  delete(params: Delete$Params, context?: HttpContext): Observable<void> {
    return this.delete$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `prototype()` */
  static readonly PrototypePath = '/api/moryx/maintenances/prototype';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `prototype()` instead.
   *
   * This method doesn't expect any request body.
   */
  prototype$Response(params?: Prototype$Params, context?: HttpContext): Observable<StrictHttpResponse<Entry>> {
    return prototype(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `prototype$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  prototype(params?: Prototype$Params, context?: HttpContext): Observable<Entry> {
    return this.prototype$Response(params, context).pipe(
      map((r: StrictHttpResponse<Entry>): Entry => r.body)
    );
  }

  /** Path part for operation `add()` */
  static readonly AddPath = '/api/moryx/maintenances/new';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `add()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  add$Response(params?: Add$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return add(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `add$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  add(params?: Add$Params, context?: HttpContext): Observable<void> {
    return this.add$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `acknowledge()` */
  static readonly AcknowledgePath = '/api/moryx/maintenances/{id}/acknowledge';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `acknowledge()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  acknowledge$Response(params: Acknowledge$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return acknowledge(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `acknowledge$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  acknowledge(params: Acknowledge$Params, context?: HttpContext): Observable<void> {
    return this.acknowledge$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `start()` */
  static readonly StartPath = '/api/moryx/maintenances/{id}/start';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `start()` instead.
   *
   * This method doesn't expect any request body.
   */
  start$Response(params: Start$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return start(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `start$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  start(params: Start$Params, context?: HttpContext): Observable<void> {
    return this.start$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `stream()` */
  static readonly StreamPath = '/api/moryx/maintenances/stream';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `stream()` instead.
   *
   * This method doesn't expect any request body.
   */
  stream$Response(params?: Stream$Params, context?: HttpContext): Observable<StrictHttpResponse<MoryxMaintenanceEndpointsDtosMaintenanceOrderDto>> {
    return stream(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `stream$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  stream(params?: Stream$Params, context?: HttpContext): Observable<MoryxMaintenanceEndpointsDtosMaintenanceOrderDto> {
    return this.stream$Response(params, context).pipe(
      map((r: StrictHttpResponse<MoryxMaintenanceEndpointsDtosMaintenanceOrderDto>): MoryxMaintenanceEndpointsDtosMaintenanceOrderDto => r.body)
    );
  }

}
