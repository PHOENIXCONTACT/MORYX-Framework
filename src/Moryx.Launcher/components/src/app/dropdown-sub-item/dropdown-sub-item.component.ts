import { Component, ElementRef, EventEmitter, Input, Output } from '@angular/core';
import { DropdownItemComponent } from "../dropdown-item/dropdown-item.component";
import { DropdownContainerComponent } from "../dropdown-container/dropdown-container.component";
import { DropdownMenuComponent } from "../dropdown-menu/dropdown-menu.component";

@Component({
  selector: 'app-dropdown-sub-item',
  standalone: true,
  imports: [DropdownItemComponent, DropdownContainerComponent, DropdownMenuComponent],
  templateUrl: './dropdown-sub-item.component.html',
  styleUrl: './dropdown-sub-item.component.css'
})
export class DropdownSubItemComponent {
  @Input() closeOnClick : boolean = false;
  @Output() onClick = new EventEmitter<boolean>();
  @Input() class: string = "";
  
  
  handleClick(){
    this.onClick.emit(this.closeOnClick);
  }
}
