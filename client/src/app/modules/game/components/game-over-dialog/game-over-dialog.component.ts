import {Component, Inject} from "@angular/core";
import {Router} from "@angular/router";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {GameResults} from "@game/models/game-results";

@Component({
  selector: 'field-menu-dialog-dialog',
  templateUrl: './game-over-dialog.component.html',
  styleUrls: ['./game-over-dialog.component.scss']
})
export class GameOverDialogComponent {

  constructor(private router: Router,
              private dialogRef: MatDialogRef<GameOverDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public results: GameResults) {}

  public onExit(): void {
    this.dialogRef.close();
    this.router.navigate(['']);
  }
}
