import { CommonModule } from '@angular/common';
import { AfterContentInit, AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { localLanguage } from '../utils';

@Component({
  selector: 'app-language-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './language-selector.component.html',
  styleUrl: './language-selector.component.css'
})
export class LanguageSelectorComponent {
  @Input() language: string = "de-DE";
  @Input() class: string = "";

  currentLanguage(){
    return localLanguage();
  }

  isChecked(){
    return this.currentLanguage() === this.language;
  }

  onClick(){
    let CookieDate = new Date;
    CookieDate.setFullYear(CookieDate.getFullYear() + 1);
    const cookieString = `c=${this.language}|uic=${this.language}`;
    const encodedCookie = escape(cookieString);
    document.cookie =
        '.AspNetCore.Culture=' + encodedCookie +
        '; expires=' + CookieDate.toUTCString() +
        '; path=/;';
    location.reload();
  }
}
