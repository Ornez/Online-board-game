import {Character} from "@game/enums/character.enum";
import {Equipment} from "@game/models/equipment";
import {UserData} from "@game/models/user-data";
import {Position} from "@game/models/position";

export interface Player {
  userData: UserData;
  characterName: Character;
  position: Position;
  movementPoints: number;
  healthPoints: number;
  treasures: number;
  equipment: Equipment;
  queueOrder: number;
}
