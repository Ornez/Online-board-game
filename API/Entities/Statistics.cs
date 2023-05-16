namespace API.Entities;
public class Statistics
{
    public int Id { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int DefeatedEnemies { get; set; }
    public int OpenedChests { get; set; }
    public AppUser AppUser { get; set; }
}
