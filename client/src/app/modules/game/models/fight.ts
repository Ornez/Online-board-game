import {UserData} from "@game/models/user-data";
import {Enemy} from "@game/enums/enemy.enum";

export interface Fight {
  userData: UserData;
  enemyName: Enemy;
  enemyDamage: number;
}
