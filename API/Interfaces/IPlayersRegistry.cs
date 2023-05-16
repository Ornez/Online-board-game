using API.DTOs;

namespace API.Interfaces
{
    public interface IPlayersRegistry
    {
        void AddPlayer(string connectionId, MemberDto player);
        void RemovePlayer(string connectionId);
        MemberDto GetPlayer(string connectionId);
        MemberDto GetPlayer(int userId);
    }
}