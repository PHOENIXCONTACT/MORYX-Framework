export interface WebModuleItem extends ModuleItem {
  eventStream? : string
}
export interface ExternalModuleItem extends ModuleItem {
  url? : string
}

export interface ModuleItem {
  route : string,
  sortIndex : number,
  title : string,
  icon : string,
  description : string,
  category : ModuleCategory,
}

export enum ModuleCategory {
  User = 'User',
  Settings = 'Settings',
  Diagnostics = 'Diagnostics',
  Help = 'Help'
}


export interface CultureModel {
  name : string,
  displayName : string
}
