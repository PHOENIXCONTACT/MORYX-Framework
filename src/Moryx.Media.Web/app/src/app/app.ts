/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { MediaService } from './services/media-service/media.service';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.html',
    styleUrls: ['./app.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [RouterOutlet]
})
export class App implements OnInit {
  private mediaService = inject(MediaService);

  ngOnInit(): void {
    this.mediaService.loadContents();
  }
}

