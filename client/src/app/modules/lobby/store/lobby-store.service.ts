import {Injectable} from "@angular/core";
import {BaseState, StateService} from "@shared/common/state.service";
import {filter, Observable, Subject, take, takeUntil} from "rxjs";
import {WsLobbyService} from "@lobby/services/ws-lobby.service";
import {LobbyPlayer} from "@lobby/models/lobby-player";
import {AuthService} from "@auth/services/auth.service";
import {Router} from "@angular/router";

interface LobbyState {
  lobbyPlayers: BaseState<LobbyPlayer[]>;
  changeReadiness: BaseState<boolean>;
  lobbyId: number,
  playerKicked: boolean;
  playerLeft: boolean;
  ownerUsername: string;
}

const initialState: LobbyState = {
  lobbyPlayers: {data: []},
  changeReadiness: {},
  lobbyId: undefined,
  playerKicked: false,
  playerLeft: undefined,
  ownerUsername: undefined
}

@Injectable({
  providedIn: 'root'
})
export class LobbyStoreService extends StateService<LobbyState> {

  lobbySubscription$: Subject<boolean> = new Subject<boolean>();

  readonly lobbyPlayers$: Observable<LobbyPlayer[]> = this.select(state => state.lobbyPlayers.data);
  readonly lobbyOwnerUsername$: Observable<string> = this.select(state => state.ownerUsername);

  readonly playerKickedSuccess$: Observable<boolean> = this.select(() => this.state.playerKicked)
  readonly playerLeftSuccess$: Observable<boolean> = this.select(() => this.state.playerLeft)

  constructor(private ws: WsLobbyService,
              private auth: AuthService,
              private router: Router) {
    super(initialState);
  }

  public connectLobbyHub(): void {
    this.ws.initLobbyConnection().pipe(takeUntil(this.lobbySubscription$)).subscribe( () => {
      this.ws
        .getPlayers()
        .pipe(takeUntil(this.lobbySubscription$))
        .subscribe((players: LobbyPlayer[]) => {
          this.setState({lobbyPlayers: {data: players}} as Partial<LobbyState>);
          this.findOwner();
        });

      this.ws
        .listenToPlayerJoined()
        .pipe(takeUntil(this.lobbySubscription$))
        .subscribe((joinedPlayer: LobbyPlayer) => {
          this.state.lobbyPlayers.data.push(joinedPlayer)
          this.setState({lobbyPlayers: {data: this.state.lobbyPlayers.data}} as Partial<LobbyState>);
        });

      this.ws
        .listenToPlayerChangedReadiness()
        .pipe(takeUntil(this.lobbySubscription$))
        .subscribe((updatedPlayer: LobbyPlayer) => {
          const index = this.state.lobbyPlayers.data.findIndex(player => player.username === updatedPlayer.username);
          this.state.lobbyPlayers.data[index] = updatedPlayer;
          this.setState({lobbyPlayers: {data: this.state.lobbyPlayers.data}} as Partial<LobbyState>);
        });

      this.ws
        .listenToPlayerLeft()
        .pipe(takeUntil(this.lobbySubscription$))
        .subscribe((leftPlayer: LobbyPlayer) => {
          const index = this.state.lobbyPlayers.data.findIndex(player => player.username === leftPlayer.username);
          this.state.lobbyPlayers.data.splice(index, 1);
          this.setState({lobbyPlayers: {data: this.state.lobbyPlayers.data}} as Partial<LobbyState>);
        });

      this.ws
        .listenToPlayerWasKicked()
        .pipe(takeUntil(this.lobbySubscription$))
        .subscribe((kickedPlayer: LobbyPlayer) => {
          const index = this.state.lobbyPlayers.data.findIndex(player => player.username === kickedPlayer.username);
          this.state.lobbyPlayers.data.splice(index, 1);
          this.setState({lobbyPlayers: {data: this.state.lobbyPlayers.data}} as Partial<LobbyState>);
          this.setState({playerKicked: kickedPlayer.username === this.auth.getUsername()} as Partial<LobbyState>);
          this.setState({playerKicked: false} as Partial<LobbyState>);
        });

      this.ws
        .listenToGameStarted()
        .pipe(takeUntil(this.lobbySubscription$))
        .subscribe(() => {
          this.disconnectLobbyHub();
          this.router.navigate(['', 'game'])
        })
    })
  }

  public changeReadiness(isReady: boolean): void {
    this.ws.changeReadiness(isReady)
      .pipe(
        take(1),
        filter(webSocketResponse => webSocketResponse.success === true))
      .subscribe( () => {
        const player = this.state.lobbyPlayers.data.find(player => player.username === this.auth.getUsername());
        player.isReady = !player.isReady;
        this.setState({lobbyPlayers: {data: this.state.lobbyPlayers.data}} as Partial<LobbyState>);
      });
  }

  public kickPlayer(playerToKick: LobbyPlayer): void {
    this.ws.kickPlayer(playerToKick)
      .pipe(take(1))
      .subscribe(() => {})
  }

  public startTheGame(): void {
    this.ws.startTheGame();
  }

  public leaveLobby(): void {
    this.ws.leaveLobby({lobbyId: this.state.lobbyId})
      .pipe(
        take(1),
        filter(webSocketResponse => webSocketResponse.success === true))
      .subscribe(() => {
        this.setState({playerLeft: true} as Partial<LobbyState>)
        this.setState({playerLeft: undefined} as Partial<LobbyState>)
      })
  }

  private findOwner(): void {
    const owner = this.state.lobbyPlayers.data.find( (player: LobbyPlayer) => player.isOwner === true);
    owner ? this.setState({ownerUsername: owner.username} as Partial<LobbyState>) : undefined;
  }

  public disconnectLobbyHub(): void {
    this.lobbySubscription$.next(true);
    this.lobbySubscription$.complete();
    this.ws.closeLobbyConnection();
    this.setState({lobbyPlayers: {data: undefined}} as Partial<LobbyState>);
  }

  public setLobbyId(lobbyId: number): void {
    this.setState({lobbyId: lobbyId} as Partial<LobbyState>);
  }

  public getLobbyId(): number {
    return this.state.lobbyId;
  }
}
