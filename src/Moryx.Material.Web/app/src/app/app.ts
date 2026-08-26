/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, OnDestroy, OnInit, resource, Signal, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
    LanguageService,
    SnackbarService
} from '@moryx/ngx-web-framework/services';
import { RouterOutlet, RouterLink, RouterLinkActive, ActivatedRoute, Router, EventType, NavigationEnd } from '@angular/router';
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { filter, firstValueFrom, lastValueFrom, SubscriptionLike } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { HttpErrorResponse } from '@angular/common/http';
import { DialogAddMaterialContainerComponent } from './dialogs/dialog-add-material-container/dialog-add-material-container.component';
import { MaterialFlowService } from './services/material-flow.service';
import { MaterialManagementService, ResourceModificationService } from './api/services';
import { MaterialContainerModel, OrderReferenceModel } from './api/models';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrls: ['./app.scss'],
    imports: [
        MatIconModule,
        MatButtonModule,
        MatInputModule,
        MatFormFieldModule,
        MatSelectModule,
        RouterOutlet,
        RouterLink,
        MatTabsModule
    ]
})
export class App implements OnInit, OnDestroy {
    private route = inject(Router);
    private routeEvent = toSignal(this.route.events.pipe(filter(x => x && x.type === EventType.NavigationEnd)));
    private dialog = inject(MatDialog);
    private materialFlow = inject(MaterialFlowService);
    private materialApi = inject(MaterialManagementService);
    private containersSource = toSignal(this.materialApi.getAll());
    private containerResource = resource({
        loader: () : Promise<MaterialContainerModel[]> => firstValueFrom(this.materialApi.getAll())
    });
    private resourceApi = inject(ResourceModificationService);
    private snackbarService = inject(SnackbarService);
    views = Views;
    private subscriptions: SubscriptionLike[] = [];

    protected view = computed(() => {
        const event = this.routeEvent();
        const url = event?.urlAfterRedirects ?? "";
        return url.includes(Views.history) ? Views.history :
            url.includes(Views.summary) ? Views.summary :
                Views.cards;
    });
    protected selectedOrders = signal<string[]>([]);
    protected selectedProducts = signal<string[]>([]);
    protected ordersSource = signal<OrderReferenceModel[]>([]);
    protected hasOrderIntegration = signal<boolean>(false);

    constructor() {
        effect(() => {
            const filters = [];
            if (this.selectedOrders().length) {
                filters.push(...this.selectedOrders());
            }
            if (this.selectedProducts().length) {
                filters.push(...this.selectedProducts());
            }
            this.materialFlow.executeFilter(filters);
        })
    }

    ngOnInit(): void {
        const sub = this.materialApi.hasOrderIntegration().subscribe(response => {
            if (response) {
                this.hasOrderIntegration.set(true);
                this.fetchLinkedOrders();
            }
        })
        this.subscriptions.push(sub);
    }

    fetchLinkedOrders() {
        const orderSub = this.materialApi.getOrderReferences().subscribe(orders => {
            this.ordersSource.set(orders);
        });
        this.subscriptions.push(orderSub);
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    products() {
        return this.containersSource()?.map(x => x.material) ?? [];
    }

    onAdd() {
        const dialogRef = this.dialog.open(DialogAddMaterialContainerComponent, {
            height: '560px',
            width: '560px'
        });

        dialogRef.afterClosed().subscribe(async (result: any | undefined) => {
            if (!result) {
                return;
            }

            const constructed = await lastValueFrom(this.resourceApi
                .constructWithParameters({
                    type: result.name,
                    method: result.method?.name,
                    body: result.method?.parameters,
                }))
                .catch(async (e: HttpErrorResponse) => await this.snackbarService.handleError(e));
            if (!constructed) {
                return;
            }
            this.snackbarService.showSuccess("Material Container Created!");
            if (this.hasOrderIntegration()) {
                this.fetchLinkedOrders();
            }
        }
        )
    }
}

export enum Views {
    cards = "cards",
    summary = "summary",
    history = "history"
}
