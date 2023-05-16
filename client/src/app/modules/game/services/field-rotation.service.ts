import {Injectable} from "@angular/core";
import {BehaviorSubject} from "rxjs";
import {FieldType} from "@game/enums/field-type.enum";
import {RouteFieldToPlace} from "@game/models/route-field-to-place";
import {PossibleRouteField} from "@game/models/possible-route-field";
import {BoardUtils} from "@game/utils/board-utils";

@Injectable()
export class FieldRotationService {
  routeFieldToPlace: RouteFieldToPlace;

  currentFieldType$: BehaviorSubject<FieldType> = new BehaviorSubject<FieldType>(FieldType.EMPTY_FIELD);
  currentFieldRotation$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  possibleFieldsWithCurrentFieldRotation$: BehaviorSubject<PossibleRouteField[]> = new BehaviorSubject<PossibleRouteField[]>([]);

  public rotateToLeft(): void {
    this.currentFieldRotation$.next((this.currentFieldRotation$.value - 90 + 360) % 360);
    this.possibleFieldsWithCurrentFieldRotation$.next(BoardUtils.generatePossibleFields(this.routeFieldToPlace.positionsToPlace[this.currentFieldRotation$.value]));
  }

  public rotateToRight(): void {
    this.currentFieldRotation$.next((this.currentFieldRotation$.value + 90) % 360);
    this.possibleFieldsWithCurrentFieldRotation$.next(BoardUtils.generatePossibleFields(this.routeFieldToPlace.positionsToPlace[this.currentFieldRotation$.value]));
  }

  public setPossibleFields(routeFieldToPlace: RouteFieldToPlace): void {
    this.routeFieldToPlace = routeFieldToPlace;
    this.currentFieldType$.next(routeFieldToPlace.fieldType);
    this.possibleFieldsWithCurrentFieldRotation$.next(BoardUtils.generatePossibleFields(this.routeFieldToPlace.positionsToPlace[this.currentFieldRotation$.value]));
  }

  public clearPossibleFields(): void {
    this.possibleFieldsWithCurrentFieldRotation$.next([]);
  }

  public getCurrentRotation(): number {
    return this.currentFieldRotation$.value;
  }
}
