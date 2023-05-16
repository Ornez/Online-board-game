import {Component, Inject} from "@angular/core";
import {Player} from "@game/models/player";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {GameStoreService} from "@game/store/game-store.service";
import {RoundStoreService} from "@game/store/round-store.service";
import {AuthService} from "@auth/services/auth.service";

@Component({
  selector: 'player-details-dialog',
  templateUrl: './player-details-dialog.component.html',
  styleUrls: ['./player-details-dialog.component.scss']
})
export class PlayerDetailsDialogComponent {

  constructor(@Inject(MAT_DIALOG_DATA) public player: Player,
              private dialogRef: MatDialogRef<PlayerDetailsDialogComponent>,
              private auth: AuthService,
              private roundStore: RoundStoreService,
              private gameStore: GameStoreService) {}

  public onUseHealingScroll(): void {
    const isPlayerRound: boolean = this.roundStore.isCurrentPlayersRound();
    const isPlayerEquipment: boolean = this.player.userData.username === this.auth.getUsername();

    if (isPlayerRound && isPlayerEquipment) {
      this.dialogRef.close();
      this.gameStore.useHealingScroll();
    }
  }
}
