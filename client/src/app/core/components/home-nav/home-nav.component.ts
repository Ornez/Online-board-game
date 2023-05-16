import { Component } from '@angular/core';
import { MatDialog } from "@angular/material/dialog";
import { SettingsDialogComponent } from "@core/components/settings-dialog/settings-dialog.component";
import { StatisticsDialogComponent } from "@core/components/statistics-dialog/statistics-dialog.component";
import { LoginDialogComponent } from "@auth/components/login-dialog/login-dialog.component";
import { AuthStoreService } from "@auth/store/auth-store.service";
import {LobbyListDialogComponent} from "@lobby-list/components/lobby-list-dialog/lobby-list-dialog.component";
import {FindLobbyDialogComponent} from "@lobby-list/components/find-lobby-dialog/find-lobby-dialog.component";
import {AuthService} from "@auth/services/auth.service";

@Component({
  selector: 'home-nav',
  templateUrl: './home-nav.component.html',
  styleUrls: ['./home-nav.component.scss'],
})
export class HomeNavComponent {

  constructor(private dialog: MatDialog,
              private authStore: AuthStoreService,
              private auth: AuthService) {}

  public onOpenSettingsDialog(): void {
    this.dialog.open(SettingsDialogComponent, { autoFocus: false, width: '400px' });
  }

  public onOpenStatisticsDialog(): void {
    this.auth.isLoggedIn()
      ? this.dialog.open(StatisticsDialogComponent, { autoFocus: false, width: '400px' })
      : this.dialog.open(LoginDialogComponent, { autoFocus: false, data: () => this.onOpenStatisticsDialog()});
  }

  public onOpenLobbyListDialog(): void {
    this.dialog.open(LobbyListDialogComponent, { autoFocus: false, width: '800px' });
  }

  public onOpenFindLobbyDialog(): void {
    this.dialog.open(FindLobbyDialogComponent, { autoFocus: false, width: '400px' });
  }

  public onExit(): void {
    this.authStore.logout();
  }
}
