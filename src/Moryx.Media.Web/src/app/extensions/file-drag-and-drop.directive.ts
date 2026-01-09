import {
  Directive,
  HostBinding,
  HostListener,
  Output,
  EventEmitter,
} from '@angular/core';

@Directive({ selector: '[appFileDragAndDrop]' })
export class FileDragAndDropDirective {
  constructor() {}

  @Output() fileDropped = new EventEmitter();
  @HostBinding('class.fileover') fileover?: boolean;

  //Drag over listener
  @HostListener('dragover', ['$event']) onDragOver(event: any) {
    event.preventDefault();
    event.stopPropagation();
    this.fileover = true;
  }

  //Drag leave listener
  @HostListener('dragleave', ['$event']) public onDragLeave(event: any) {
    event.preventDefault();
    event.stopPropagation();
    this.fileover = false;
  }

  //Drop listener
  @HostListener('drop', ['$event']) public onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.fileover = false;
    let files = event.dataTransfer?.files;
    if (files) {
      this.fileDropped.emit({ target: { files } });
    }
  }
}
