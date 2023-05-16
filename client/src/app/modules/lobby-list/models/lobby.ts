import {LobbyPlayer} from "@lobby-list/models/lobby-player";

export interface Lobby {
  id: number;
  lobbyName: string;
  players: LobbyPlayer[],
  maxPlayersNumber: number;
  isPrivate: boolean;
}
