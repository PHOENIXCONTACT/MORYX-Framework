import {ModuleCategory} from './module-category';

export interface ModuleItem {
  route: string,
  sortIndex: number,
  title: string,
  icon: string,
  description: string,
  category: ModuleCategory,
}
