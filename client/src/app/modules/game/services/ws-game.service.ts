import {Injectable} from "@angular/core";
import {HttpTransportType, HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {from, Observable, Subject} from "rxjs";
import {RouteField} from "@game/models/route-field";
import {Player} from "@game/models/player";
import {AuthService} from "@auth/services/auth.service";
import {UserData} from "@game/models/user-data";
import {BoardUtils} from "@game/utils/board-utils";
import {Position} from "@game/models/position";
import {PlayerMovement} from "@game/models/player-movement";
import {FightCalculation} from "@game/models/fight-calculation";
import {Fight} from "@game/models/fight";
import {FightResult} from "@game/enums/fight-result.enum";
import {map} from "rxjs/operators";
import {GameResults} from "@game/models/game-results";

@Injectable()
export class WsGameService {

  private gameHubConnection: HubConnection;

  constructor(private auth: AuthService) {}

  public initGameConnection(): Observable<void> {
    this.gameHubConnection = new HubConnectionBuilder()
      .withUrl('/game', {
        accessTokenFactory: () => this.auth.getJwtToken(),
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    return from(this.gameHubConnection.start());
  }

  public getRouteField(): Observable<any> {
    return from(this.gameHubConnection.invoke('GetRouteField'));
  }

  public setRouteField(position: Position, rotation: number): Observable<PlayerMovement> {
    return from(this.gameHubConnection.invoke('SetRouteField', {position, rotation}));
  }

  public getPositionsToMove(): Observable<Position[]> {
    return from(this.gameHubConnection.invoke('GetPositionsToMove'));
  }

  public movePlayer(position: Position): Observable<PlayerMovement> {
    return from(this.gameHubConnection.invoke('Move', position));
  }

  public rollTheDice(): Observable<number[]> {
    return from(this.gameHubConnection.invoke('RollTheDice'));
  }

  public calculateFight(damageScrollsUsed: number): Observable<FightCalculation> {
    return from(this.gameHubConnection.invoke('CalculateFight', {damageScrollsUsed}));
  }

  public fight(damageScrollsUsed: number): Observable< FightResult> {
    return from(this.gameHubConnection.invoke('StartFight', {damageScrollsUsed}))
      .pipe(map((result: {fightResult: FightResult}) => result.fightResult));
  }

  public pickUpItem(): Observable<any> {
    return from(this.gameHubConnection.invoke('PickUpItem'));
  }

  public openChest(): Observable<any> {
    return from(this.gameHubConnection.invoke('OpenChest'));
  }

  public useHealingScroll(): Observable<any> {
    return from(this.gameHubConnection.invoke('UseHealingScroll'));
  }

  public roundOver(): Observable<any> {
    return from(this.gameHubConnection.invoke('RoundOver'));
  }

  public listenToPlayersCharactersChosen(): Observable<Player[]> {
    const players$: Subject<Player[]> = new Subject<Player[]>();
    this.gameHubConnection.on('players_selected', (players: Player[]) => players$.next(BoardUtils.sortPlayers(players)));
    return players$;
  }

  public listenToPlayerUpdated(): Observable<Player> {
    const player$: Subject<Player> = new Subject<Player>();
    this.gameHubConnection.on('player_updated', (player: Player) => player$.next(player));
    return player$;
  }

  public listenToRouteFieldAdded(): Observable<RouteField> {
    const newRouteField$: Subject<RouteField> = new Subject<RouteField>();
    this.gameHubConnection.on('route_field_placed', (routeField: RouteField) => newRouteField$.next(routeField));
    return newRouteField$;
  }

  public listenToRouteFieldUpdated(): Observable<RouteField> {
    const updatedRouteField$: Subject<RouteField> = new Subject<RouteField>();
    this.gameHubConnection.on('route_field_updated', (routeField: RouteField) => updatedRouteField$.next(routeField));
    return updatedRouteField$;
  }

  public listenToPlayerRoundStarted(): Observable<UserData> {
    const currentRoundPlayer$: Subject<UserData> = new Subject<UserData>();
    this.gameHubConnection.on('round_started', (player: UserData) => currentRoundPlayer$.next(player));
    return currentRoundPlayer$;
  }

  public listenToPlayerMoved(): Observable<PlayerMovement> {
    const playerMovement$: Subject<PlayerMovement> = new Subject<PlayerMovement>();
    this.gameHubConnection.on('player_moved', (playerMovement: PlayerMovement) => playerMovement$.next(playerMovement));
    return playerMovement$;
  }

  public listenToFightStarted(): Observable<Fight> {
    const fight$: Subject<Fight> = new Subject<Fight>();
    this.gameHubConnection.on('fight_started', (fight: Fight) => fight$.next(fight));
    return fight$;
  }

  public listenToDiceRolled(): Observable<number[]> {
    const diceRolls$: Subject<number[]> = new Subject<number[]>();
    this.gameHubConnection.on('dice_rolled', (diceRolls: number[]) => diceRolls$.next(diceRolls));
    return diceRolls$;
  }

  public listenToFightCalculated(): Observable<FightCalculation> {
    const fightCalculation$: Subject<FightCalculation> = new Subject<FightCalculation>();
    this.gameHubConnection.on('fight_calculated', (calculation: FightCalculation) => fightCalculation$.next(calculation));
    return fightCalculation$;
  }

  public listenToFightOver(): Observable<FightResult> {
    const fightResult$: Subject<FightResult> = new Subject<FightResult>();
    this.gameHubConnection.on('fight_over', (result: {fightResult: FightResult}) => fightResult$.next(result.fightResult));
    return fightResult$;
  }

  public listenToGameOver(): Observable<GameResults> {
    const gameResults$: Subject<GameResults> = new Subject<GameResults>();
    this.gameHubConnection.on('game_ended', (results: GameResults) => gameResults$.next(results));
    return gameResults$;
  }

  public closeGameConnection(): void {
    this.gameHubConnection.stop();
  }
}
