import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-navigation-button',
  standalone: true,
  imports: [],
  templateUrl: './navigation-button.component.html',
  styleUrl: './navigation-button.component.css'
})
export class NavigationButtonComponent {
  public static POSITION_ATTRIBUTE = "position";
 @Input() position: NavigationButtonPosition = "default";
}

export type NavigationButtonPosition = "fixed" | "default";