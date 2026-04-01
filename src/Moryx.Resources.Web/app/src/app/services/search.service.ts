import { effect, inject, Injectable, untracked } from '@angular/core';
import { CacheResourceService } from './cache-resource.service';
import { ResourceModel } from '../api/models';
import { SearchBarService, SearchRequest, SearchSuggestion } from '@moryx/ngx-web-framework/services';
import { EditResourceService } from './edit-resource.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class SearchService {
  private cacheService = inject(CacheResourceService);
  private editService = inject(EditResourceService);
  private searchBarService = inject(SearchBarService);
  private router = inject(Router);

  private blockedByEditing = toSignal(this.editService.edit$);
  private resources = toSignal(this.cacheService.flatResources);

  constructor() {
    effect(() => {
      const blocked = this.blockedByEditing();
      untracked(() => {
        if (!blocked) {
          this.hookupSubscription();  
          return;
        }

        this.searchBarService.clearSuggestions();
        this.searchBarService.unsubscribe();
      });
    });
  }

  private hookupSubscription() {
    this.searchBarService.subscribe({
      next: (newRequest: SearchRequest) => {
        this.onSearch(newRequest);
      }
    });
  }
  
  private onSearch(result: SearchRequest) {
    let possibleResults = this.getMatchingResources(result.term);
    if (!possibleResults.length) {
      this.searchBarService.provideSuggestions([]);
    }

    if (result.submitted) {
      this.processSearch(possibleResults);
    } else {
      this.updateSuggestions(possibleResults);
    }
  }

  /**
   * Returns resources matching the @param searchTerm in their id, name, type or 
   * description case-insensitive. The results are sorted by name.
   */
  private getMatchingResources(searchTerm: string): ResourceModel[] {
    const resources = this.resources();
    if (!resources) 
      return [];

    const possibleResults = resources.filter(r => this.matchById(searchTerm, r) 
      || this.matchByName(searchTerm, r) 
      || this.matchByType(searchTerm, r) 
      || this.matchByDescription(searchTerm, r));
    
    return possibleResults.sort((a, b) => a.name?.localeCompare(b.name ?? '') ?? 0);
  }

  private matchById(searchTerm: string, r: ResourceModel): unknown {
    return r.id?.toString().includes(searchTerm.toLowerCase());
  }
    
  private matchByName(searchTerm :string, r: ResourceModel): unknown {
    return r.name?.toLowerCase()?.includes(searchTerm.toLowerCase());
  }
  
  private matchByType(searchTerm: string, r: ResourceModel): unknown {
    return r.type?.toLowerCase()?.includes(searchTerm.toLowerCase());
  }

  private matchByDescription(searchTerm: string, r: ResourceModel): unknown {
    return r.description?.toLowerCase()?.includes(searchTerm.toLowerCase());
  }

  private processSearch(possibleResults: ResourceModel[]) {
    this.searchBarService.clearSuggestions();
    if (possibleResults.length === 1 && possibleResults[0].id) {
      this.selectResource(possibleResults[0].id);
    }
    this.hookupSubscription();
  }

  private selectResource(id: number) {
    if (this.blockedByEditing() || this.editService.activeResource()?.id === id) return;
    this.router.navigate([`/details/${id}`]);
  }

  private updateSuggestions(possibleResults: ResourceModel[]) {
    const searchSuggestions = [] as SearchSuggestion[];
    for (let resource of possibleResults) {
      const urlBase = 'Resources/details/';
      const url = urlBase + resource.id;
      searchSuggestions.push({ text: this.createSuggestionText(resource), url: url });
    }

    this.searchBarService.provideSuggestions(searchSuggestions);
  }

  private createSuggestionText(resource: ResourceModel): string {
    return `#${resource.id} - ${resource.name}`;
  }
}
