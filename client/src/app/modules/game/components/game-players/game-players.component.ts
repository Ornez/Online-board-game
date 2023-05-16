import {Component} from "@angular/core";
import {GameStoreService} from "@game/store/game-store.service";
import {Player} from "@game/models/player";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {PlayerDetailsDialogComponent} from "@game/components/player-details-dialog/player-details-dialog.component";
import {OverlayPositionBuilder} from "@angular/cdk/overlay";
import {RoundStoreService} from "@game/store/round-store.service";

@Component({
  selector: 'game-players',
  templateUrl: './game-players.component.html',
  styleUrls: ['./game-players.component.scss']
})
export class GamePlayersComponent {

  private dialogRef: MatDialogRef<PlayerDetailsDialogComponent>;

  constructor(public gameStore: GameStoreService,
              public roundStore: RoundStoreService,
              private overlayPositionBuilder: OverlayPositionBuilder,
              private dialog: MatDialog) {}

  public onOpenPlayerDialog(player: Player, elementRef: any): void {
    this.dialogRef ? this.dialogRef.close() : undefined;

    const triggerRect = elementRef.getBoundingClientRect();

    this.dialogRef = this.dialog.open(PlayerDetailsDialogComponent, {autoFocus: false, data: player, hasBackdrop: true, backdropClass: 'mat-dialog--no-background'});

    this.dialogRef.updatePosition({
      left: triggerRect.x + triggerRect.width + 10 + 'px',
      top: triggerRect.y + triggerRect.height / 2 - 200 + 'px'
    });
  }
}
