import {RouteField} from "@game/models/route-field";
import {Player} from "@game/models/player";

export interface Board {
  fields: RouteField[];
  players: Player[];
}
