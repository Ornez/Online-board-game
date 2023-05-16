import {UserData} from "@game/models/user-data";
import {Character} from "@game/enums/character.enum";

export interface GameResults {
  firstPlace: {
    userData: UserData,
    characterName: Character
  }[];
  secondPlace: {
    userData: UserData,
    characterName: Character
  }[];
  thirdPlace: {
    userData: UserData,
    characterName: Character
  }[];
}
