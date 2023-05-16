import {Component} from "@angular/core";
import {FieldRotationService} from "@game/services/field-rotation.service";

@Component({
  selector: 'field-rotation-dialog',
  templateUrl: './field-rotation-dialog.component.html',
  styleUrls: ['./field-rotation-dialog.component.scss']
})
export class FieldRotationDialogComponent {

  constructor(public fieldRotation: FieldRotationService) {}

  public onRotateToLeft(): void {
    this.fieldRotation.rotateToLeft();
  }

  public onRotateToRight(): void {
    this.fieldRotation.rotateToRight();
  }
}
