using API.Modules.Game.Items;

namespace API.Modules.Game.Inventories;

public class HealingScroll : Scroll
{
    public HealingScroll() : base(ItemKeys.HEALING_SCROLL, ItemType.HealingScroll)
    {
    }
}

public class DamageScroll : Scroll
{
    public DamageScroll() : base(ItemKeys.ATTACK_SCROLL, ItemType.DamageScroll)
    {
    }
}
