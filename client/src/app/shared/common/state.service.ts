import { BehaviorSubject, Observable } from 'rxjs';
import { distinctUntilChanged, map } from 'rxjs/operators';

export interface BaseState<T = any> {
  loading?: boolean;
  success?: boolean;
  data?: T;
}

export class StateService<T> {
  private state$: BehaviorSubject<T>;

  protected get state(): T {
    return this.state$.getValue();
  }

  constructor(initialState: T) {
    this.state$ = new BehaviorSubject<T>(initialState);
  }

  protected select<K>(mapFn: (state: T) => K): Observable<K> {
    return this.state$.asObservable().pipe(
      map((state: T) => mapFn(state)),
      distinctUntilChanged()
    );
  }

  protected setState(newState: Partial<T>, rewriteArray = false) {
    this.state$.next(this.mergeDeep(this.state, newState, rewriteArray));
  }

  private mergeDeep(currState: any, newState: any, rewriteArray = false) {
    const isObject = (obj: object) => obj && typeof obj === 'object';

    return [ currState, newState ].reduce((prev, obj) => {
      Object.keys(obj).forEach(key => {
        const pVal = prev[key];
        const oVal = obj[key];

        if (Array.isArray(pVal) && Array.isArray(oVal) && !rewriteArray) {
          const ids = new Set(pVal.map(d => d.id));
          prev[key] = [ ...oVal, ...pVal.filter(d => !ids.has(d.id)) ];
        } else if (Array.isArray(pVal) && Array.isArray(oVal) && rewriteArray) {
          prev[key] = oVal;
        } else if (isObject(pVal) && isObject(oVal)) {
          prev[key] = this.mergeDeep(pVal, oVal, rewriteArray);
        } else {
          prev[key] = oVal;
        }
      });

      return prev;
    }, {});
  }
}
