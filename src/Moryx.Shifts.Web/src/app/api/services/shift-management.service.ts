/* tslint:disable */
/* eslint-disable */
import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';

import { createShift } from '../fn/shift-management/create-shift';
import { CreateShift$Params } from '../fn/shift-management/create-shift';
import { createShiftAssignement } from '../fn/shift-management/create-shift-assignement';
import { CreateShiftAssignement$Params } from '../fn/shift-management/create-shift-assignement';
import { createShiftType } from '../fn/shift-management/create-shift-type';
import { CreateShiftType$Params } from '../fn/shift-management/create-shift-type';
import { deleteShift } from '../fn/shift-management/delete-shift';
import { DeleteShift$Params } from '../fn/shift-management/delete-shift';
import { deleteShiftAssignement } from '../fn/shift-management/delete-shift-assignement';
import { DeleteShiftAssignement$Params } from '../fn/shift-management/delete-shift-assignement';
import { deleteShiftType } from '../fn/shift-management/delete-shift-type';
import { DeleteShiftType$Params } from '../fn/shift-management/delete-shift-type';
import { getShiftAssignements } from '../fn/shift-management/get-shift-assignements';
import { getShiftAssignements_1 } from '../fn/shift-management/get-shift-assignements-1';
import { GetShiftAssignements_1$Params } from '../fn/shift-management/get-shift-assignements-1';
import { GetShiftAssignements$Params } from '../fn/shift-management/get-shift-assignements';
import { getShifts } from '../fn/shift-management/get-shifts';
import { getShifts_1 } from '../fn/shift-management/get-shifts-1';
import { GetShifts_1$Params } from '../fn/shift-management/get-shifts-1';
import { GetShifts$Params } from '../fn/shift-management/get-shifts';
import { getShiftTypes } from '../fn/shift-management/get-shift-types';
import { GetShiftTypes$Params } from '../fn/shift-management/get-shift-types';
import { ShiftAssignementModel } from '../models/shift-assignement-model';
import { ShiftModel } from '../models/shift-model';
import { ShiftTypeModel } from '../models/shift-type-model';
import { updateShift } from '../fn/shift-management/update-shift';
import { UpdateShift$Params } from '../fn/shift-management/update-shift';
import { updateShiftAssignement } from '../fn/shift-management/update-shift-assignement';
import { UpdateShiftAssignement$Params } from '../fn/shift-management/update-shift-assignement';
import { updateShiftType } from '../fn/shift-management/update-shift-type';
import { UpdateShiftType$Params } from '../fn/shift-management/update-shift-type';

@Injectable({ providedIn: 'root' })
export class ShiftManagementService extends BaseService {
  constructor(config: ApiConfiguration, http: HttpClient) {
    super(config, http);
  }

  /** Path part for operation `getShifts()` */
  static readonly GetShiftsPath = '/api/moryx/shifts';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getShifts()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShifts$Response(params?: GetShifts$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftModel>>> {
    return getShifts(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getShifts$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShifts(params?: GetShifts$Params, context?: HttpContext): Observable<Array<ShiftModel>> {
    return this.getShifts$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<ShiftModel>>): Array<ShiftModel> => r.body)
    );
  }

  /** Path part for operation `updateShift()` */
  static readonly UpdateShiftPath = '/api/moryx/shifts';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateShift()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateShift$Response(params?: UpdateShift$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return updateShift(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `updateShift$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateShift(params?: UpdateShift$Params, context?: HttpContext): Observable<void> {
    return this.updateShift$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `createShift()` */
  static readonly CreateShiftPath = '/api/moryx/shifts';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `createShift()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  createShift$Response(params?: CreateShift$Params, context?: HttpContext): Observable<StrictHttpResponse<ShiftModel>> {
    return createShift(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `createShift$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  createShift(params?: CreateShift$Params, context?: HttpContext): Observable<ShiftModel> {
    return this.createShift$Response(params, context).pipe(
      map((r: StrictHttpResponse<ShiftModel>): ShiftModel => r.body)
    );
  }

  /** Path part for operation `getShifts_1()` */
  static readonly GetShifts_1Path = '/api/moryx/shifts/filter';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getShifts_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShifts_1$Response(params?: GetShifts_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftModel>>> {
    return getShifts_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getShifts_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShifts_1(params?: GetShifts_1$Params, context?: HttpContext): Observable<Array<ShiftModel>> {
    return this.getShifts_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<ShiftModel>>): Array<ShiftModel> => r.body)
    );
  }

  /** Path part for operation `deleteShift()` */
  static readonly DeleteShiftPath = '/api/moryx/shifts/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteShift()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteShift$Response(params: DeleteShift$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return deleteShift(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `deleteShift$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteShift(params: DeleteShift$Params, context?: HttpContext): Observable<void> {
    return this.deleteShift$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `getShiftTypes()` */
  static readonly GetShiftTypesPath = '/api/moryx/shifts/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getShiftTypes()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShiftTypes$Response(params?: GetShiftTypes$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftTypeModel>>> {
    return getShiftTypes(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getShiftTypes$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShiftTypes(params?: GetShiftTypes$Params, context?: HttpContext): Observable<Array<ShiftTypeModel>> {
    return this.getShiftTypes$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<ShiftTypeModel>>): Array<ShiftTypeModel> => r.body)
    );
  }

  /** Path part for operation `updateShiftType()` */
  static readonly UpdateShiftTypePath = '/api/moryx/shifts/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateShiftType()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateShiftType$Response(params?: UpdateShiftType$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return updateShiftType(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `updateShiftType$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateShiftType(params?: UpdateShiftType$Params, context?: HttpContext): Observable<void> {
    return this.updateShiftType$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `createShiftType()` */
  static readonly CreateShiftTypePath = '/api/moryx/shifts/types';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `createShiftType()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  createShiftType$Response(params?: CreateShiftType$Params, context?: HttpContext): Observable<StrictHttpResponse<ShiftTypeModel>> {
    return createShiftType(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `createShiftType$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  createShiftType(params?: CreateShiftType$Params, context?: HttpContext): Observable<ShiftTypeModel> {
    return this.createShiftType$Response(params, context).pipe(
      map((r: StrictHttpResponse<ShiftTypeModel>): ShiftTypeModel => r.body)
    );
  }

  /** Path part for operation `deleteShiftType()` */
  static readonly DeleteShiftTypePath = '/api/moryx/shifts/types/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteShiftType()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteShiftType$Response(params: DeleteShiftType$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return deleteShiftType(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `deleteShiftType$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteShiftType(params: DeleteShiftType$Params, context?: HttpContext): Observable<void> {
    return this.deleteShiftType$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `getShiftAssignements()` */
  static readonly GetShiftAssignementsPath = '/api/moryx/shifts/assignements';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getShiftAssignements()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShiftAssignements$Response(params?: GetShiftAssignements$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftAssignementModel>>> {
    return getShiftAssignements(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getShiftAssignements$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShiftAssignements(params?: GetShiftAssignements$Params, context?: HttpContext): Observable<Array<ShiftAssignementModel>> {
    return this.getShiftAssignements$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<ShiftAssignementModel>>): Array<ShiftAssignementModel> => r.body)
    );
  }

  /** Path part for operation `updateShiftAssignement()` */
  static readonly UpdateShiftAssignementPath = '/api/moryx/shifts/assignements';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `updateShiftAssignement()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateShiftAssignement$Response(params?: UpdateShiftAssignement$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return updateShiftAssignement(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `updateShiftAssignement$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  updateShiftAssignement(params?: UpdateShiftAssignement$Params, context?: HttpContext): Observable<void> {
    return this.updateShiftAssignement$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

  /** Path part for operation `createShiftAssignement()` */
  static readonly CreateShiftAssignementPath = '/api/moryx/shifts/assignements';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `createShiftAssignement()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  createShiftAssignement$Response(params?: CreateShiftAssignement$Params, context?: HttpContext): Observable<StrictHttpResponse<ShiftAssignementModel>> {
    return createShiftAssignement(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `createShiftAssignement$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  createShiftAssignement(params?: CreateShiftAssignement$Params, context?: HttpContext): Observable<ShiftAssignementModel> {
    return this.createShiftAssignement$Response(params, context).pipe(
      map((r: StrictHttpResponse<ShiftAssignementModel>): ShiftAssignementModel => r.body)
    );
  }

  /** Path part for operation `getShiftAssignements_1()` */
  static readonly GetShiftAssignements_1Path = '/api/moryx/shifts/assignements/filter';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getShiftAssignements_1()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShiftAssignements_1$Response(params?: GetShiftAssignements_1$Params, context?: HttpContext): Observable<StrictHttpResponse<Array<ShiftAssignementModel>>> {
    return getShiftAssignements_1(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `getShiftAssignements_1$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getShiftAssignements_1(params?: GetShiftAssignements_1$Params, context?: HttpContext): Observable<Array<ShiftAssignementModel>> {
    return this.getShiftAssignements_1$Response(params, context).pipe(
      map((r: StrictHttpResponse<Array<ShiftAssignementModel>>): Array<ShiftAssignementModel> => r.body)
    );
  }

  /** Path part for operation `deleteShiftAssignement()` */
  static readonly DeleteShiftAssignementPath = '/api/moryx/shifts/assignements/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `deleteShiftAssignement()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteShiftAssignement$Response(params: DeleteShiftAssignement$Params, context?: HttpContext): Observable<StrictHttpResponse<void>> {
    return deleteShiftAssignement(this.http, this.rootUrl, params, context);
  }

  /**
   * This method provides access only to the response body.
   * To access the full response (for headers, for example), `deleteShiftAssignement$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  deleteShiftAssignement(params: DeleteShiftAssignement$Params, context?: HttpContext): Observable<void> {
    return this.deleteShiftAssignement$Response(params, context).pipe(
      map((r: StrictHttpResponse<void>): void => r.body)
    );
  }

}
