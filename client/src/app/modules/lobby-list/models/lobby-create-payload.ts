export interface LobbyCreatePayload {
  lobbyName: string,
  password?: string,
  maxPlayersNumber: number,
  isPrivate: boolean
}
