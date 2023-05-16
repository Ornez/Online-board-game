namespace API.Modules.Game.Inventories;

public class Weapon : Item
{
    public Weapon(string name, int damage) : base(name, ItemType.Weapon)
    {
        Damage = damage;
    }

    public int Damage { get; set; }
}