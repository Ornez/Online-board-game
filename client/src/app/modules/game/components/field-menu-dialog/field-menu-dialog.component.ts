import {Component, Inject} from "@angular/core";
import {FieldMenuAction} from "@game/enums/field-menu-action.enum";
import {MAT_DIALOG_DATA} from "@angular/material/dialog";

@Component({
  selector: 'field-menu-dialog-dialog',
  templateUrl: './field-menu-dialog.component.html',
  styleUrls: ['./field-menu-dialog.component.scss']
})
export class FieldMenuDialogComponent {

  FieldMenuAction = FieldMenuAction;

  constructor(@Inject(MAT_DIALOG_DATA) public fieldMenuItems: FieldMenuAction[]) {}
}
