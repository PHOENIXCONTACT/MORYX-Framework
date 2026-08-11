import { ElementRef } from "@angular/core";
import { CellLocationModel } from "@api/models";
import { CdkDragEnd } from '@angular/cdk/drag-drop';

export function createUpdatedLocation(event: CdkDragEnd, itemElement: ElementRef<HTMLElement>,
  containerElement: ElementRef<HTMLElement>, id: number | undefined) {

  const cellY = itemElement.nativeElement.offsetTop! + event.distance.y;
  const cellX = itemElement.nativeElement.offsetLeft! + event.distance.x;
  const containerHeight = containerElement.nativeElement.offsetHeight;
  const containerWidth = containerElement.nativeElement.offsetWidth;

  return <CellLocationModel>{
    id: id,
    positionX: clamp(cellX / containerWidth),
    positionY: clamp(cellY / containerHeight)
  };
}

function clamp(x: number) {
  return Math.max(0, Math.min(x, 1));
}
