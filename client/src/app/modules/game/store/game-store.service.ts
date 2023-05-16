import {Injectable} from "@angular/core";
import {BehaviorSubject, Subject, take, takeUntil} from "rxjs";
import {RouteField} from "@game/models/route-field";
import {WsGameService} from "@game/services/ws-game.service";
import {Player} from "@game/models/player";
import {UserData} from "@game/models/user-data";
import {FieldRotationService} from "@game/services/field-rotation.service";
import {RouteFieldToPlace} from "@game/models/route-field-to-place";
import {BoardMapper} from "@game/mappers/board-mapper";
import {map} from "rxjs/operators";
import {Position} from "@game/models/position";
import {RoundStoreService} from "@game/store/round-store.service";
import {PlayerMovement} from "@game/models/player-movement";
import {FightCalculation} from "@game/models/fight-calculation";
import {Fight} from "@game/models/fight";
import {FightResult} from "@game/enums/fight-result.enum";
import {FieldType} from "@game/enums/field-type.enum";
import {GameResults} from "@game/models/game-results";

@Injectable()
export class GameStoreService {

  readonly gameSubscription$: Subject<boolean> = new Subject<boolean>();

  readonly routeFields$: BehaviorSubject<RouteField[]> =  new BehaviorSubject<RouteField[]>([]);
  readonly players$: BehaviorSubject<Player[]> =  new BehaviorSubject<Player[]>([]);
  readonly playerMoved$: BehaviorSubject<PlayerMovement> =  new BehaviorSubject<PlayerMovement>(undefined);
  readonly gameOver$: Subject<GameResults> =  new Subject<GameResults>();

  constructor(private ws: WsGameService,
              private fieldRotation: FieldRotationService,
              private roundStore: RoundStoreService) {}

  public connectGameHub(): void {
    this.ws.initGameConnection().pipe(takeUntil(this.gameSubscription$)).subscribe( () => {
      this.ws
        .listenToPlayersCharactersChosen()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((players: Player[]) => this.players$.next(players));

      this.ws
        .listenToPlayerUpdated()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((updatedPlayer: Player) => {
          const player: Player = this.players$.value.find( (player: Player) => player.userData.username === updatedPlayer.userData.username);
          player.position = updatedPlayer.position;
          player.movementPoints = updatedPlayer.movementPoints;
          player.healthPoints = updatedPlayer.healthPoints;
          player.treasures = updatedPlayer.treasures;
          player.equipment = updatedPlayer.equipment;
          this.players$.next( this.players$.value);
        });

      this.ws
        .listenToRouteFieldAdded()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((routeField: RouteField) => {
          this.routeFields$.next([...this.routeFields$.value, routeField]);
        });

      this.ws
        .listenToRouteFieldUpdated()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((updatedField: RouteField) => {
          const field: RouteField = this.routeFields$.value.find( (routeField: RouteField) => routeField.position.x === updatedField.position.x && routeField.position.y === updatedField.position.y);
          field.chest = updatedField.chest;
          field.itemName = updatedField.itemName;
          field.enemy = updatedField.enemy;
          this.routeFields$.next( this.routeFields$.value);
        });

      this.ws
        .listenToPlayerRoundStarted()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((userData: UserData) => {
          this.roundStore.start(userData);
          this.roundStore.isCurrentPlayersRound() ? this.getPositionsToMove() : undefined;
        });

      this.ws
        .listenToPlayerMoved()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((playerMovement: PlayerMovement) => this.playerMoved$.next(playerMovement));

      this.ws
        .listenToFightStarted()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((fight: Fight) => {
          this.roundStore.openFightDialog(fight);
          this.calculateFight(0);
        });

      this.ws
        .listenToDiceRolled()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((result: number[]) => this.roundStore.diceRolledResult(result));

      this.ws
        .listenToFightCalculated()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((fightCalculation: FightCalculation) => {
          this.roundStore.setPlayerStrength(fightCalculation.playerStrength);
          this.roundStore.setNumberOfUsedScrolls(fightCalculation.damageScrollsUsed);
        });

      this.ws
        .listenToFightOver()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((fightResult: FightResult) => this.roundStore.setFightResult(fightResult));

      this.ws
        .listenToGameOver()
        .pipe(takeUntil(this.gameSubscription$))
        .subscribe((results: GameResults) => this.gameOver$.next(results));
    })
  }

  public getRouteField(): void {
    this.ws.getRouteField()
      .pipe(
        take(1),
        map(value => BoardMapper.mapToRouteFieldToPlace(value)))
      .subscribe((routeField: RouteFieldToPlace) => {
        this.fieldRotation.setPossibleFields(routeField);
        this.roundStore.hideDrawFieldButton();
        this.roundStore.openFieldRotationDialog();
      });
  }

  public setRouteField(position: Position, rotation: number): void {
    this.ws.setRouteField(position, rotation)
      .pipe(take(1))
      .subscribe((playerMovement: PlayerMovement) => {
        this.fieldRotation.clearPossibleFields();
        this.roundStore.closeFieldRotationDialog();
        this.roundStore.showDrawFieldButton();
        this.playerMoved$.next(playerMovement);
        this.roundStore.setMovementPoints(playerMovement.movementPoints);
        this.getPositionsToMove();
      });
  }

  public getPositionsToMove(): void {
    this.ws.getPositionsToMove()
      .pipe(take(1))
      .subscribe((positions: Position[]) => this.roundStore.setPositionsToMove(positions));
  }

  public movePlayer(position: Position): void {
    this.ws.movePlayer(position)
      .pipe(take(1))
      .subscribe((playerMovement: PlayerMovement) => {
        this.playerMoved$.next(playerMovement);
        this.roundStore.setMovementPoints(playerMovement.movementPoints);
        this.getPositionsToMove();
      });
  }

  public rollTheDice(numberOfDamageScrolls: number): void {
    this.ws.rollTheDice()
      .pipe(take(1))
      .subscribe((result: number[]) => {
        this.roundStore.diceRolledResult(result);
        this.calculateFight(numberOfDamageScrolls);
      });
  }

  public calculateFight(numberOfDamageScrolls: number): void {
    this.ws.calculateFight(numberOfDamageScrolls)
      .pipe(take(1))
      .subscribe((fightCalculation: FightCalculation) => this.roundStore.setPlayerStrength(fightCalculation.playerStrength));
  }

  public fight(numberOfDamageScrolls: number): void {
    this.ws.fight(numberOfDamageScrolls)
      .pipe(take(1))
      .subscribe((fightResult: FightResult) => this.roundStore.setFightResult(fightResult));
  }

  public pickUpItem(): void {
    this.ws.pickUpItem()
      .pipe(take(1))
      .subscribe();
  }

  public openChest(): void {
    this.ws.openChest()
      .pipe(take(1))
      .subscribe();
  }

  public useHealingScroll(): void {
    this.ws.useHealingScroll()
      .pipe(take(1))
      .subscribe(() => this.getPositionsToMove());
  }

  public roundOver(): void {
    this.ws.roundOver()
      .pipe(take(1))
      .subscribe( () => {
        this.roundStore.closeFieldRotationDialog();
        this.fieldRotation.setPossibleFields({fieldType: FieldType.EMPTY_FIELD, positionsToPlace: {0: [], 90: [], 180: [], 270: []}});
      });
  }

  public disconnectGameHub(): void {
    this.ws.closeGameConnection();
    this.gameSubscription$.next(true);
    this.gameSubscription$.complete();

    this.players$.next([]);
    this.routeFields$.next([]);
    this.playerMoved$.next(undefined);
  }
}


