import {FieldType} from "@game/enums/field-type.enum";
import {Position} from "@game/models/position";

export interface RouteFieldToPlace {
  fieldType: FieldType;
  positionsToPlace: {
    0: Position[];
    90: Position[];
    180: Position[];
    270: Position[];
  }
}
