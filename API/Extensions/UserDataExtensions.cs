using API.Data.Game;

namespace API.Extensions;
public static class UserDataExtensions
{
    public static bool IsSameUserAs(this UserData userData, UserData otherUserData) 
    {
        return (userData.Id == otherUserData.Id && userData.Role == otherUserData.Role);
    }
}
