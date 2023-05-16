import {Injectable} from "@angular/core";
import {MatSnackBar} from "@angular/material/snack-bar";

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private snackBar: MatSnackBar) {}

  public showInfoMessage(message: string): void {
    this.snackBar.open(message, null, {
      horizontalPosition: 'end',
      verticalPosition: 'top',
      duration: 3000,
      panelClass: 'mat-snack-bar-info'
    });
  }

  public showErrorMessage(message: string): void {
    this.snackBar.open(message, 'OK', {
      horizontalPosition: 'end',
      verticalPosition: 'top',
      duration: 3000,
      panelClass: 'mat-snack-bar-error'
    });
  }
}
