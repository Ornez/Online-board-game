import {UserData} from "@game/models/user-data";

export interface FightCalculation {
  userData: UserData;
  playerStrength: number;
  enemyStrength: number;
  damageScrollsUsed: number;
}
