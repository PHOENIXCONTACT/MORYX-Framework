import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { CardComponent } from "../card/card.component";
import { MaterialContainer } from 'src/app/models/material-container';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';
import { MaterialFlowService } from 'src/app/services/material-flow.service';

@Component({
  selector: 'app-cards',
  imports: [CardComponent],
  templateUrl: './cards.component.html',
  styleUrl: './cards.component.scss',
})
export class CardsComponent implements OnInit {
  private materialFlow = inject(MaterialFlowService);
  filterEvents = toSignal(this.materialFlow.$filter);

  containers = signal<MaterialContainer[]>([]);

  constructor() {
    effect(() => {
      const filters = this.filterEvents() ?? [];
      if (filters.length > 0) {
        this.containers.set(this.materialFlow.containers.filter(e => filters.some(f => e.resource.includes(f) || e.type.includes(f))));
      } else {
        this.containers.set(this.materialFlow.containers);
      }
    })
  }

  ngOnInit(): void {

  }
}

