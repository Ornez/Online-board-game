import {Component, OnInit} from '@angular/core';
import {LobbyStoreService} from "@lobby/store/lobby-store.service";
import {ChatStoreService} from "@chat/store/chat-store.service";
import {ActivatedRoute, Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {LobbyListDialogComponent} from "@lobby-list/components/lobby-list-dialog/lobby-list-dialog.component";
import {filter, takeUntil} from "rxjs";
import {ObjectUtils} from "@shared/utils/object-utils";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {AuthService} from "@auth/services/auth.service";
import {LobbyPlayer} from "@lobby/models/lobby-player";

@Component({
  selector: 'lobby-page',
  templateUrl: './lobby-page.component.html',
  styleUrls: ['./lobby-page.component.scss']
})
export class LobbyPageComponent extends BaseComponent implements OnInit {
  private ownerUsername: string;
  public isAllPlayersReady: boolean = false;

  constructor(public lobbyStore: LobbyStoreService,
              private chatStore: ChatStoreService,
              private auth: AuthService,
              private route: ActivatedRoute,
              private router: Router,
              private dialog: MatDialog) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
    this.lobbyStore.connectLobbyHub();
    this.chatStore.connectChatHub();
  }

  public onLeaveLobby(): void {
    this.lobbyStore.leaveLobby();
  }

  public onCopyLobbyCode(): void {
    const textarea = document.createElement('textarea');
    textarea.style.opacity = '0';
    textarea.style.height = '0';
    textarea.style.width = '0';
    textarea.value = this.lobbyStore.getLobbyId().toString();
    document.body.appendChild(textarea);
    textarea.focus();
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
  }

  public onStartGame(): void {
    this.lobbyStore.startTheGame();
  }

  public isStartGameButtonVisible(): boolean {
    return this.ownerUsername === this.auth.getUsername();
  }

  private checkIsAllPlayersReady(players: LobbyPlayer[]): void {
    const notReadyPlayer = players.find( player => player.isReady === false);
    this.isAllPlayersReady = notReadyPlayer === undefined;
  }

  private initDataHandle(): void {
    this.lobbyStore.lobbyPlayers$
      .pipe(
        filter((players: LobbyPlayer[]) => !ObjectUtils.isNil(players)),
        takeUntil(this.destroyed$))
      .subscribe( (players: LobbyPlayer[]) => this.checkIsAllPlayersReady(players));

    this.lobbyStore.lobbyOwnerUsername$
      .pipe(
        filter(ownerUsername => !ObjectUtils.isNil(ownerUsername)),
        takeUntil(this.destroyed$))
      .subscribe( (ownerUsername: string) => this.ownerUsername = ownerUsername);

    this.lobbyStore.playerKickedSuccess$
      .pipe(
        filter(Boolean),
        takeUntil(this.destroyed$))
      .subscribe( () => this.onLeaveLobby());

    this.lobbyStore.playerLeftSuccess$
      .pipe(
        filter(Boolean),
        takeUntil(this.destroyed$))
      .subscribe( () => {
        this.chatStore.disconnectChatHub();
        this.router.navigate(['']);
        this.dialog.open(LobbyListDialogComponent, { autoFocus: false, width: '800px' });
      });
  }

  public ngOnDestroy() {
    this.lobbyStore.disconnectLobbyHub();
    super.ngOnDestroy();
  }
}
