import {ItemType} from "@game/enums/item-type.enum";
import {Enemy as EnemyEnum} from "@game/enums/enemy.enum";

export interface Enemy {
  itemReward: ItemType;
  name: EnemyEnum;
  strength: number;
}
