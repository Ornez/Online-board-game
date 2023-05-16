using API.Modules.Game.Keys;
using API.Modules.Game.Items;

namespace API.Modules.Game.Inventories;

public class Key : Item
{
    public Key() : base(ItemKeys.KEY, ItemType.Key)
    {
    }
}