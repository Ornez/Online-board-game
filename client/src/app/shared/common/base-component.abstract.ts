import {Directive, OnDestroy} from '@angular/core';
import {Subject} from 'rxjs';

@Directive()
export abstract class BaseComponent implements OnDestroy {

  protected destroyed$ = new Subject<boolean>();

  protected constructor() {}

  public ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }
}
