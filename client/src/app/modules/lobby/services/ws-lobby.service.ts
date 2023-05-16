import {Injectable} from "@angular/core";
import {from, Observable, Subject} from "rxjs";
import {HttpTransportType, HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {WebSocketResponse} from "@lobby-list/models/web-socket-response";
import {LobbyLeavePayload} from "@lobby/models/lobby-leave-payload";
import {LobbyPlayer} from "@lobby/models/lobby-player";
import {AuthService} from "@auth/services/auth.service";

@Injectable({
  providedIn: "root"
})
export class WsLobbyService {

  lobbyPlayers$: Subject<LobbyPlayer[]> = new Subject<LobbyPlayer[]>();
  gameStarted$: Subject<boolean> = new Subject<boolean>();

  lobbyHubConnection: HubConnection;

  constructor(private auth: AuthService) {
  }

  public initLobbyConnection(): Observable<void> {
    this.lobbyHubConnection = new HubConnectionBuilder()
      .withUrl('ws-backend/lobby', {
        accessTokenFactory: () => this.auth.getJwtToken(),
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    return from(this.lobbyHubConnection.start());
  }

  public getPlayers(): Observable<LobbyPlayer[]> {
    return from(this.lobbyHubConnection.invoke('GetPlayers'));
  }

  public listenToPlayerJoined(): Observable<LobbyPlayer> {
    const joinedPlayer$: Subject<LobbyPlayer> = new Subject<LobbyPlayer>();
    this.lobbyHubConnection.on('player_joined', (joinedPlayer: LobbyPlayer) => joinedPlayer$.next(joinedPlayer))
    return joinedPlayer$;
  }

  public listenToPlayerChangedReadiness(): Observable<LobbyPlayer> {
    const updatedPlayer$: Subject<LobbyPlayer> = new Subject<LobbyPlayer>();
    this.lobbyHubConnection.on('player_changed_readiness', (updatedPlayer: LobbyPlayer) => updatedPlayer$.next(updatedPlayer))
    return updatedPlayer$;
  }

  public listenToPlayerLeft(): Observable<LobbyPlayer> {
    const leftPlayer$: Subject<LobbyPlayer> = new Subject<LobbyPlayer>();
    this.lobbyHubConnection.on('player_left', (leftPlayer: LobbyPlayer) => leftPlayer$.next(leftPlayer))
    return leftPlayer$;
  }

  public listenToPlayerWasKicked(): Observable<LobbyPlayer> {
    const kickedPlayer$: Subject<LobbyPlayer> = new Subject<LobbyPlayer>();
    this.lobbyHubConnection.on('player_kicked', (kickedPlayer: LobbyPlayer) => kickedPlayer$.next(kickedPlayer))
    return kickedPlayer$;
  }

  public listenToGameStarted(): Observable<boolean> {
    this.lobbyHubConnection.on('game_started', () => this.gameStarted$.next(true));
    return this.gameStarted$;
  }

  public leaveLobby(lobbyLeavePayload: LobbyLeavePayload): Observable<WebSocketResponse> {
    return from(this.lobbyHubConnection.invoke('LeaveLobby', lobbyLeavePayload));
  }

  public changeReadiness(isReady: boolean): Observable<WebSocketResponse> {
    return from(this.lobbyHubConnection.invoke('SetReady', {isReady: isReady}));
  }

  public kickPlayer(player: LobbyPlayer): Observable<WebSocketResponse> {
    return from(this.lobbyHubConnection.invoke('KickPlayer', player));
  }

  public startTheGame(): Observable<WebSocketResponse> {
    return from(this.lobbyHubConnection.invoke('StartTheGame'));
  }

  public closeLobbyConnection(): void {
    this.lobbyHubConnection.stop();
  }
}
