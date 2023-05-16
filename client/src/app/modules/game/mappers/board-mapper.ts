import {RouteFieldToPlace} from "@game/models/route-field-to-place";

export class BoardMapper {

  public static mapToRouteFieldToPlace(value: any): RouteFieldToPlace {
    return {
      fieldType: value.fieldType,
      positionsToPlace: {
        0: value.positionsToPlace[0],
        90: value.positionsToPlace[1],
        180: value.positionsToPlace[2],
        270: value.positionsToPlace[3],
      }
    }
  }
}
