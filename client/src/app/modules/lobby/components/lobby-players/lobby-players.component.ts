import {Component, OnInit} from '@angular/core';
import {LobbyStoreService} from "@lobby/store/lobby-store.service";
import {LobbyPlayer} from "@lobby/models/lobby-player";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {filter, takeUntil} from "rxjs";
import {ObjectUtils} from "@shared/utils/object-utils";
import {AuthService} from "@auth/services/auth.service";

@Component({
  selector: 'lobby-players',
  templateUrl: './lobby-players.component.html',
  styleUrls: ['./lobby-players.component.scss'],
})
export class LobbyPlayersComponent extends BaseComponent implements OnInit{

  private ownerUsername: string;

  constructor(public lobbyStore: LobbyStoreService,
              public auth: AuthService) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
  }

  public onToggleReadiness(lobbyPlayer: LobbyPlayer): void {
    this.lobbyStore.changeReadiness(!lobbyPlayer.isReady);
  }

  public onKickPlayer(lobbyPlayer: LobbyPlayer): void {
    this.lobbyStore.kickPlayer(lobbyPlayer);
  }

  public isKickButtonVisible(lobbyPlayer: LobbyPlayer): boolean {
    const isOwner = this.ownerUsername === this.auth.getUsername();
    const isNotActualPlayer = lobbyPlayer.username !== this.auth.getUsername();
    return isOwner && isNotActualPlayer;
  }

  private initDataHandle(): void {
    this.lobbyStore.lobbyOwnerUsername$
      .pipe(
        filter(ownerUsername => !ObjectUtils.isNil(ownerUsername)),
        takeUntil(this.destroyed$))
      .subscribe( (ownerUsername: string) => this.ownerUsername = ownerUsername);
  }
}
