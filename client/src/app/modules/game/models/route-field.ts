import {FieldType} from "@game/enums/field-type.enum";
import {Position} from "@game/models/position";
import {ItemType} from "@game/enums/item-type.enum";
import {Enemy} from "@game/models/enemy";

export interface RouteField {
  fieldType: FieldType;
  position: Position;
  rotation: number;
  enemy?: Enemy;
  itemName?: ItemType;
  chest?: boolean
}
