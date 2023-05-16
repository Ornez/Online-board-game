using API.Modules.Guests.Data;

namespace API.Modules.Guests.Managers;
public interface IGuestsManager
{
    GuestData CreateNewGuest();
    void RegisterGuest(string token);
    bool IsGuestRegistered(string token);
}
