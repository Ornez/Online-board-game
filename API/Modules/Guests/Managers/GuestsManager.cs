using API.Modules.Guests.Data;
using API.Utils;

namespace API.Modules.Guests.Managers;
public class GuestsManager : Registry<GuestData>, IGuestsManager
{
    private IdGenerator idGenerator = new();

    private List<string> registeredGuests = new();

    public GuestData CreateNewGuest() 
    {
        int id = idGenerator.GenerateNextId();
        GuestData newGuest = new(id, $"Guest{id}");
        AddItem(newGuest, id);
        return newGuest;
    }

    public void RegisterGuest(string token)
    {
        registeredGuests.Add(token);
    }
    
    public bool IsGuestRegistered(string token)
    {
        return registeredGuests.Contains(token);
    }
}
