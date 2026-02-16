/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Directive, output } from '@angular/core';

@Directive({
  selector: '[appFileDragAndDrop]',
  host: {
    '[class.fileover]': 'fileover',
    '(dragover)': 'onDragOver($event)',
    '(dragleave)': 'onDragLeave($event)',
    '(drop)': 'onDrop($event)'
  }
})
export class FileDragAndDropDirective {
  fileDropped = output<{ target: { files: FileList } }>();
  fileover = false;

  //Drag over listener
  onDragOver(event: any) {
    event.preventDefault();
    event.stopPropagation();
    this.fileover = true;
  }

  //Drag leave listener
  onDragLeave(event: any) {
    event.preventDefault();
    event.stopPropagation();
    this.fileover = false;
  }

  //Drop listener
  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.fileover = false;
    let files = event.dataTransfer?.files;
    if (files) {
      this.fileDropped.emit({target: {files}});
    }
  }
}

