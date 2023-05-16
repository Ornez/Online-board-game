import {Component, OnInit} from "@angular/core";
import {Player} from "@game/models/player";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {GameStoreService} from "@game/store/game-store.service";
import {filter, takeUntil} from "rxjs";
import {PlayerMovement} from "@game/models/player-movement";
import {Position} from "@game/models/position";
import {ObjectUtils} from "@shared/utils/object-utils";

@Component({
  selector: 'players-positions',
  templateUrl: './players-positions.component.html',
  styleUrls: ['./players-positions.component.scss']
})
export class PlayersPositionsComponent extends BaseComponent implements OnInit{

  players: Player[] = [];

  constructor(public gameStore: GameStoreService) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
  }

  private playerMovement(movement: PlayerMovement): void {
    const player = this.players.find((player: Player) => player.userData.username === movement.userData.username);
    movement.path.forEach((position: Position, index: number) => {
      setTimeout( () => player.position = position, index * 600 );
    })
  }

  private initDataHandle(): void {
    this.gameStore.players$
      .pipe(
        takeUntil(this.destroyed$),
        filter(players => !ObjectUtils.isNil(players)))
      .subscribe((players: Player[]) => this.players = players);

    this.gameStore.playerMoved$
      .pipe(
        takeUntil(this.destroyed$),
        filter(movement => !ObjectUtils.isNil(movement)))
      .subscribe((movement: PlayerMovement) => this.playerMovement(movement));
  }
}
