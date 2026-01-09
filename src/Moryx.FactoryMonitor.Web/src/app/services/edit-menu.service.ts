import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { EditMenuState } from './EditMenutState';

@Injectable({
  providedIn: 'root'
})
export class EditMenuService {

  private _activeState = new BehaviorSubject<EditMenuState>(EditMenuState.Closed);

  public activeState$ = this._activeState.asObservable();
  constructor() { }


  public setActiveState(state : EditMenuState) {
    this._activeState.next(state);
  }
}
