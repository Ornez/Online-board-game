import {Weapon} from "@game/enums/weapon.enum";
import {Scroll} from "@game/enums/scroll.enum";

export interface Equipment {
  weapon1: {
    name: Weapon;
    damage: number;
  }
  weapon2: {
    name: Weapon;
    damage: number;
  }
  scroll1: {
    name: Scroll
  };
  scroll2: {
    name: Scroll
  };
  scroll3: {
    name: Scroll
  };
  key: boolean;
}
