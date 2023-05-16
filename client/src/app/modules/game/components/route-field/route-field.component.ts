import {Component, Input} from "@angular/core";
import {RouteField} from "@game/models/route-field";

@Component({
  selector: 'route-field',
  templateUrl: './route-field.component.html',
  styleUrls: ['./route-field.component.scss']
})
export class RouteFieldComponent {

  @Input() field: RouteField;
  @Input() canPlayerMoveToField: boolean = false;

  constructor() {}
}
