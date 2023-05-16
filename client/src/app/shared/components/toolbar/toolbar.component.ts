import {Component, Input} from '@angular/core';
import {MatDialog} from "@angular/material/dialog";
import {SettingsDialogComponent} from "@core/components/settings-dialog/settings-dialog.component";
import {StatisticsDialogComponent} from "@core/components/statistics-dialog/statistics-dialog.component";
import {ProfileDialogComponent} from "@core/components/profile-dialog/profile-dialog.component";
import {Toolbar} from "@core/enums/toolbar.enum";
import {LoginDialogComponent} from "@auth/components/login-dialog/login-dialog.component";
import {ChatDialogComponent} from "@chat/components/chat-dialog/chat-dialog.component";
import {AuthService} from "@auth/services/auth.service";
import {ChatStoreService} from "@chat/store/chat-store.service";

@Component({
  selector: 'toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
})
export class ToolbarComponent {

  @Input() items: Toolbar[] = [];
  Toolbar = Toolbar;

  constructor(private dialog: MatDialog,
              public auth: AuthService,
              public chatStore: ChatStoreService) {}

  public onOpenProfileDialog(): void {
    this.auth.isLoggedIn()
      ? this.dialog.open(ProfileDialogComponent, { autoFocus: false, width: '400px' })
      : this.dialog.open(LoginDialogComponent, { autoFocus: false, data: () => this.onOpenProfileDialog()});
  }

  public onOpenSettingsDialog(): void {
    this.dialog.open(SettingsDialogComponent, { autoFocus: false, width: '400px' });
  }

  public onOpenChatDialog(): void {
    this.chatStore.clearUnreadMessages();
    this.dialog.open(ChatDialogComponent, {autoFocus: false, width: '400px'});
  }

  public onOpenStatisticsDialog(): void {
    this.auth.isLoggedIn()
      ? this.dialog.open(StatisticsDialogComponent, { autoFocus: false, width: '400px' })
      : this.dialog.open(LoginDialogComponent, { autoFocus: false, data: () => this.onOpenStatisticsDialog()});
  }
}
