import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

/**
 * Component used for resetting the application state on cancellation of the product editing. 
 * It reads the 'to' query parameter for the target route to navigate to. 
 * Resetting happens through the route resolvers, the route still functions as the single point of truth.
 * 
 * This solution for cancelling the editing is rather heavy weighted, as it requires a full route navigation;
 * Other considered solutions included
 * - Resetting the state by reloading the product in the edit service
 *    => RouteResolver would not be the single point for loading products anymore
 *    => Cancellation while editing newly added recipes and parts left the url in an inconsistent state
 * - Manually correcting route state by navigating to parent routes and correcting route parameters
 *    => Requires more diffuse interference with the route state
 *    => Requires checking product structure at multiple places
 *    => Still executes halve of the navigations
 * - Configuring the provided router to reload on every navigation
 *    => Causes override of the UI state when switching tabs in a selected product breaking edit mode behavior
 */
@Component({
  selector: 'app-cancellation',
  imports: [],
  templateUrl: './cancellation.component.html',
  styleUrl: './cancellation.component.scss',
})
export class CancellationComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  
  ngOnInit(): void {
    const raw = this.route.snapshot.queryParamMap.get('to'); 
    const to = raw ? decodeURIComponent(raw) : '/'; 
    const tree = this.router.parseUrl(to); 
    this.router.navigateByUrl(tree, { replaceUrl: true });
  }
}
