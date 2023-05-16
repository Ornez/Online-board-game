using API.Data;
using API.Data.Game;

namespace API.Modules.Game.Inventories.Systems;

public interface IInventorySystem
{
    Task PickUpItem(UserData userData);
    Task<ReturnMessage> OpenChest(UserData userData);
    Task RemoveHealingScroll(UserData userData);
}