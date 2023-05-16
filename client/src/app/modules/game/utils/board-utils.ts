import {PossibleRouteField} from "@game/models/possible-route-field";
import {Player} from "@game/models/player";
import {Position} from "@game/models/position";

export class BoardUtils {

  public static generatePossibleFields(possiblePositions: Position[]): PossibleRouteField[] {
    const emptyRouteFields: PossibleRouteField[] = [];
    possiblePositions.forEach( (position: Position) => emptyRouteFields.push({position: position }))
    return emptyRouteFields;
  }

  public static sortPlayers(players: Player[]): Player[] {
    return players.sort((a, b)=> {
      return a.queueOrder < b.queueOrder ? -1 : 1;
    })
  }
}
