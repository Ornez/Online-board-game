using API.DTOs;
using API.Interfaces;

namespace API.Registries
{
    public class PlayersRegistry : IPlayersRegistry
    {
        private readonly Dictionary<string, MemberDto> playersToConnectionIds = new();
        private readonly Dictionary<int, MemberDto> playersToUserIds = new();

        public PlayersRegistry() 
        {
            MockPlayersData();
        }

        private void MockPlayersData() 
        {
            MemberDto member1 = new() 
            {
                Id = 100,
                Email = "email100@gmail.com",
                Username = "User100",
                Language = new LanguageDto() 
                {
                    Name = "Polska",
                    Code = "pl"
                },
                Settings = new SettingsDto(5, 4),
                Statistics = new StatisticsDto()
                {
                    GamesPlayed = 5,
                    GamesWon = 3,
                    DefeatedEnemies = 12,
                    OpenedChests = 6
                }
            };
            playersToConnectionIds.Add("100", member1);
            playersToUserIds.Add(100, member1);

            MemberDto member2 = new() 
            {
                Id = 101,
                Email = "email101@gmail.com",
                Username = "User101",
                Language = new LanguageDto() 
                {
                    Name = "Polska",
                    Code = "pl"
                },
                Settings = new SettingsDto(5, 4),
                Statistics = new StatisticsDto()
                {
                    GamesPlayed = 5,
                    GamesWon = 3,
                    DefeatedEnemies = 12,
                    OpenedChests = 6
                }
            };
            playersToConnectionIds.Add("101", member2);
            playersToUserIds.Add(101, member2);

            MemberDto member3 = new() 
            {
                Id = 102,
                Email = "email102@gmail.com",
                Username = "User102",
                Language = new LanguageDto() 
                {
                    Name = "Polska",
                    Code = "pl"
                },
                Settings = new SettingsDto(5, 4),
                Statistics = new StatisticsDto()
                {
                    GamesPlayed = 5,
                    GamesWon = 3,
                    DefeatedEnemies = 12,
                    OpenedChests = 6
                }
            };
            playersToConnectionIds.Add("102", member3);
            playersToUserIds.Add(102, member3);

            MemberDto member4 = new() 
            {
                Id = 103,
                Email = "email103@gmail.com",
                Username = "User103",
                Language = new LanguageDto() 
                {
                    Name = "Polska",
                    Code = "pl"
                },
                Settings = new SettingsDto(5, 4),
                Statistics = new StatisticsDto()
                {
                    GamesPlayed = 5,
                    GamesWon = 3,
                    DefeatedEnemies = 12,
                    OpenedChests = 6
                }
            };
            playersToConnectionIds.Add("103", member4);
            playersToUserIds.Add(103, member4);

            MemberDto member5 = new() 
            {
                Id = 104,
                Email = "email104@gmail.com",
                Username = "User104",
                Language = new LanguageDto() 
                {
                    Name = "Polska",
                    Code = "pl"
                },
                Settings = new SettingsDto(5, 4),
                Statistics = new StatisticsDto()
                {
                    GamesPlayed = 5,
                    GamesWon = 3,
                    DefeatedEnemies = 12,
                    OpenedChests = 6
                }
            };
            playersToConnectionIds.Add("104", member5);
            playersToUserIds.Add(104, member5);
        }

        public void AddPlayer(string connectionId, MemberDto player) 
        {
            playersToConnectionIds.TryAdd(connectionId, player);
            playersToUserIds.TryAdd(player.Id, player);
        }

        public void RemovePlayer(string connectionId) 
        {
            if (playersToConnectionIds.TryGetValue(connectionId, out MemberDto member)) 
            {
                int playerId = member.Id;
                playersToConnectionIds.Remove(connectionId);
                playersToUserIds.Remove(playerId);
            }
        }

        public MemberDto GetPlayer(string connectionId) 
        {
            if (playersToConnectionIds.TryGetValue(connectionId, out MemberDto member))
            {
                return member;
            }
            return null;
        }

        public MemberDto GetPlayer(int userId) 
        {
            return playersToUserIds[userId];
        }
    }
}