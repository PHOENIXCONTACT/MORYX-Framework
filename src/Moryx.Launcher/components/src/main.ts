/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { bootstrapApplication, createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { DropdownMenuComponent } from './app/dropdown-menu/dropdown-menu.component';
import { DropdownItemComponent } from './app/dropdown-item/dropdown-item.component';
import { DropdownContainerComponent } from './app/dropdown-container/dropdown-container.component';
import { Constants } from './app/constants';
import { DropdownSubItemComponent } from './app/dropdown-sub-item/dropdown-sub-item.component';
import { LanguageSelectorComponent } from './app/language-selector/language-selector.component';
import { PageLayoutComponent } from './app/page-layout/page-layout.component';
import { NotificationsComponent } from './app/notifications/notifications.component';
import { SearchBoxComponent } from './app/search-box/search-box.component';
import { NotificationBadgeComponent } from './app/notification-badge/notification-badge.component';
import { SignInButtonComponent } from './app/auth-button/auth-button.component';
import { NavigationButtonComponent } from './app/navigation-button/navigation-button.component';

(async () => {
  const app = await createApplication({
    providers: []
  });


  const dropdownItem = createCustomElement(DropdownItemComponent,{
    injector: app.injector
  });
  const dropdownSubItem = createCustomElement(DropdownSubItemComponent,{
    injector: app.injector
  });
  const dropdownContainer = createCustomElement(DropdownContainerComponent,{
    injector: app.injector
  });

  const dropdownMenu = createCustomElement(DropdownMenuComponent,{
    injector: app.injector
  });
  const LanguageSelector = createCustomElement(LanguageSelectorComponent,{
    injector: app.injector
  });
  const pageLayout = createCustomElement(PageLayoutComponent,{
    injector: app.injector
  });
  const searchbox = createCustomElement(SearchBoxComponent,{
    injector: app.injector
  });
  const notifications = createCustomElement(NotificationsComponent,{
    injector: app.injector
  });
  const notificationBadge = createCustomElement(NotificationBadgeComponent,{
    injector: app.injector
  });
  const authButtons = createCustomElement(SignInButtonComponent,{
    injector: app.injector
  });
  const navButton = createCustomElement(NavigationButtonComponent,{
    injector: app.injector
  });

  customElements.define(Constants.WebComponentNames.DropdownMenu,dropdownMenu);
  customElements.define(Constants.WebComponentNames.DropdownItem,dropdownItem);
  customElements.define(Constants.WebComponentNames.DropdownContainer,dropdownContainer);
  customElements.define(Constants.WebComponentNames.DropdownSubItem,dropdownSubItem);
  customElements.define(Constants.WebComponentNames.LanguageSelector,LanguageSelector);
  customElements.define(Constants.WebComponentNames.PageLayout,pageLayout);
  customElements.define(Constants.WebComponentNames.Searchbox,searchbox);
  customElements.define(Constants.WebComponentNames.Notifications,notifications);
  customElements.define(Constants.WebComponentNames.NotificationBadge,notificationBadge);
  customElements.define(Constants.WebComponentNames.SignIn,authButtons);
  customElements.define(Constants.WebComponentNames.NavigationButton,navButton);

})();
