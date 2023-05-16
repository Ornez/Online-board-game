import {Component, OnInit} from '@angular/core';
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {Router} from "@angular/router";
import {filter, take, takeUntil} from "rxjs";
import {BaseComponent} from "@shared/common/base-component.abstract";
import {LobbyListStoreService} from "@lobby-list/store/lobby-list-store.service";
import {Lobby} from "@lobby-list/models/lobby";
import {LobbyAuthDialogComponent} from "@lobby-list/components/lobby-auth-dialog/lobby-auth-dialog.component";
import {CreateLobbyDialogComponent} from "@lobby-list/components/create-lobby-dialog/create-lobby-dialog.component";
import {LobbyStoreService} from "@lobby/store/lobby-store.service";

@Component({
  selector: 'lobby-list-dialog-dialog',
  templateUrl: './lobby-list-dialog.component.html',
  styleUrls: ['./lobby-list-dialog.component.scss']
})
export class LobbyListDialogComponent extends BaseComponent implements OnInit {

  constructor(public lobbyListStore: LobbyListStoreService,
              private lobbyStore: LobbyStoreService,
              private dialogRef: MatDialogRef<LobbyListDialogComponent>,
              private router: Router,
              private dialog: MatDialog) {
    super();
  }

  public ngOnInit(): void {
    this.initDataHandle();
  }

  public getOwnerUsername(lobby: Lobby): string {
    const owner = lobby.players.find(player => player.isOwner === true);
    return owner ? owner.username : '';
  }

  public onJoinTheLobby(lobby: Lobby): void {
    if (!lobby.isPrivate) {
      this.lobbyListStore.joinTheLobby({lobbyName: lobby.lobbyName});
      this.lobbyListStore.joinTheLobbySuccess$
        .pipe(
          filter(Boolean),
          take(1))
        .subscribe(() => {
          this.dialogRef.close(true);
          this.lobbyStore.setLobbyId(lobby.id);
          this.router.navigate(['', 'lobby']);
        });
      return;
    }

    this.dialog
      .open(LobbyAuthDialogComponent, { autoFocus: null, data:lobby, width: '400px' })
      .afterClosed()
      .pipe(
        takeUntil(this.destroyed$),
        filter(Boolean))
      .subscribe( () => {
        this.dialogRef.close();
        this.lobbyStore.setLobbyId(lobby.id);
        this.router.navigate(['', 'lobby']);
      })
  }

  public onCreateLobby(): void {
    this.dialog.open(CreateLobbyDialogComponent, { autoFocus: null, width: '400px' })
  }

  private initDataHandle(): void {
    this.lobbyListStore.createLobbySuccess$
      .pipe(
        filter(Boolean),
        takeUntil(this.destroyed$))
      .subscribe( () => this.dialogRef.close(true));
  }
}
