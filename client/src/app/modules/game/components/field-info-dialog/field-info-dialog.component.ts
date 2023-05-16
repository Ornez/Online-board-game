import {Component, Inject} from "@angular/core";
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {Enemy} from "@game/models/enemy";
import {Player} from "@game/models/player";
import {ItemType} from "@game/enums/item-type.enum";

@Component({
  selector: 'field-info-dialog',
  templateUrl: './field-info-dialog.component.html',
  styleUrls: ['./field-info-dialog.component.scss']
})
export class FieldInfoDialogComponent {

  constructor(@Inject(MAT_DIALOG_DATA) public data: {enemy: Enemy, players: Player[], itemName: ItemType, chest: boolean}) { }
}
