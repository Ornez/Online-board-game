using API.Modules.Game.Items;

namespace API.Modules.Game.Inventories;

public class ShortSword : Weapon
{
    public ShortSword() : base(ItemKeys.SWORD, 1)
    {
    }
}

public class Spear : Weapon
{
    public Spear() : base(ItemKeys.SPEAR, 2)
    {
    }
}

public class BattleAxe : Weapon
{
    public BattleAxe() : base(ItemKeys.AXE, 3)
    {
    }
}