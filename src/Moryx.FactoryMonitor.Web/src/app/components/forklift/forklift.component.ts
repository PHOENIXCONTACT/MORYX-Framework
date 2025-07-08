import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIcon, MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-forklift',
    templateUrl: './forklift.component.html',
    styleUrls: ['./forklift.component.scss'],
    imports: [CommonModule, MatIconModule],
    standalone: true
})
export class ForkliftComponent {

}
