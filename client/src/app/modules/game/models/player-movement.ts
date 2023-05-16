import {Position} from "@game/models/position";
import {UserData} from "@game/models/user-data";

export interface PlayerMovement {
  path: Position[];
  userData: UserData;
  movementPoints: number;
}
