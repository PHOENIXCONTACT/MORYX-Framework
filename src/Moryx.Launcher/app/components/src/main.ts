/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { bootstrapApplication, createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { DropdownMenu } from './app/dropdown-menu/dropdown-menu';
import { DropdownItem } from './app/dropdown-item/dropdown-item';
import { DropdownContainer } from './app/dropdown-container/dropdown-container';
import { Constants } from './app/constants';
import { DropdownSubItem } from './app/dropdown-sub-item/dropdown-sub-item';
import { LanguageSelector } from './app/language-selector/language-selector';
import { PageLayout } from './app/page-layout/page-layout';
import { Notifications } from './app/notifications/notifications';
import { SearchBox } from './app/search-box/search-box';
import { NotificationBadge } from './app/notification-badge/notification-badge';
import { SignInButton } from './app/auth-button/auth-button';
import { NavigationButton } from './app/navigation-button/navigation-button';

(async () => {
  const app = await createApplication({
    providers: []
  });


  const dropdownItem = createCustomElement(DropdownItem,{
    injector: app.injector
  });
  const dropdownSubItem = createCustomElement(DropdownSubItem,{
    injector: app.injector
  });
  const dropdownContainer = createCustomElement(DropdownContainer,{
    injector: app.injector
  });

  const dropdownMenu = createCustomElement(DropdownMenu,{
    injector: app.injector
  });
  const languageSelector = createCustomElement(LanguageSelector,{
    injector: app.injector
  });
  const pageLayout = createCustomElement(PageLayout,{
    injector: app.injector
  });
  const searchbox = createCustomElement(SearchBox,{
    injector: app.injector
  });
  const notifications = createCustomElement(Notifications,{
    injector: app.injector
  });
  const notificationBadge = createCustomElement(NotificationBadge,{
    injector: app.injector
  });
  const authButtons = createCustomElement(SignInButton,{
    injector: app.injector
  });
  const navButton = createCustomElement(NavigationButton,{
    injector: app.injector
  });

  customElements.define(Constants.WebComponentNames.DropdownMenu,dropdownMenu);
  customElements.define(Constants.WebComponentNames.DropdownItem,dropdownItem);
  customElements.define(Constants.WebComponentNames.DropdownContainer,dropdownContainer);
  customElements.define(Constants.WebComponentNames.DropdownSubItem,dropdownSubItem);
  customElements.define(Constants.WebComponentNames.LanguageSelector,languageSelector);
  customElements.define(Constants.WebComponentNames.PageLayout,pageLayout);
  customElements.define(Constants.WebComponentNames.Searchbox,searchbox);
  customElements.define(Constants.WebComponentNames.Notifications,notifications);
  customElements.define(Constants.WebComponentNames.NotificationBadge,notificationBadge);
  customElements.define(Constants.WebComponentNames.SignIn,authButtons);
  customElements.define(Constants.WebComponentNames.NavigationButton,navButton);

})();
