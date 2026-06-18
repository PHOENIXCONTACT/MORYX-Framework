import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {HorizontalModuleNav} from '../horizontal-module-nav/horizontal-module-nav';

@Component({
  selector: 'app-operator-layout',
  imports: [HorizontalModuleNav],
  templateUrl: './operator-layout.html',
  styleUrl: './operator-layout.scss',
})
export class OperatorLayout {
}
