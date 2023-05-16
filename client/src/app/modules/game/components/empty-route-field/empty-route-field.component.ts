import {Component, Input} from "@angular/core";
import {FieldType} from "@game/enums/field-type.enum";

@Component({
  selector: 'empty-route-field',
  templateUrl: './empty-route-field.component.html',
  styleUrls: ['./empty-route-field.component.scss']
})
export class EmptyRouteFieldComponent {

  @Input() fieldType: FieldType;
  @Input() fieldRotation: number;
  constructor() {}
}
