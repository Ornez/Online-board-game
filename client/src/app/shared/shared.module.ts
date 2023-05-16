import {NgModule} from '@angular/core';
import {TranslateModule} from "@ngx-translate/core";
import {RouterModule} from "@angular/router";
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MatDialogModule} from '@angular/material/dialog';
import {MatSelectModule} from '@angular/material/select';
import {MatInputModule} from "@angular/material/input";
import {MatIconModule} from "@angular/material/icon";
import {MatButtonModule} from "@angular/material/button";
import {MatTableModule} from "@angular/material/table";
import {MatSliderModule} from '@angular/material/slider';
import {NavigationButtonComponent} from "@shared/components/navigation-button/navigation-button.component";
import {LoaderComponent} from "@shared/components/loader/loader.component";
import {MatCardModule} from "@angular/material/card";
import {MatCheckboxModule} from '@angular/material/checkbox';
import {ToolbarComponent} from "@shared/components/toolbar/toolbar.component";
import {MatSnackBarModule} from "@angular/material/snack-bar";
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import {ScrollingModule} from '@angular/cdk/scrolling';
import {MatBadgeModule} from '@angular/material/badge';

const materials = [
  MatDialogModule,
  MatSelectModule,
  MatInputModule,
  MatIconModule,
  MatButtonModule,
  MatTableModule,
  MatSliderModule,
  MatCardModule,
  MatCheckboxModule,
  MatSnackBarModule,
  MatSlideToggleModule,
  MatBadgeModule
]

@NgModule({
  declarations: [
    NavigationButtonComponent,
    LoaderComponent,
    ToolbarComponent
  ],
  imports: [
    CommonModule,
    materials
  ],
  exports: [
    TranslateModule,
    RouterModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NavigationButtonComponent,
    LoaderComponent,
    ToolbarComponent,
    ScrollingModule,
    materials
  ],
  providers: [],
})
export class SharedModule { }
