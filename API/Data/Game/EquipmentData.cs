using API.Modules.Game.Inventories;

namespace API.Data.Game
{
    public class EquipmentData
    {
        public Weapon? Weapon1 { get; set; }// = new() { Name = "SWORD", Damage = 2 };
        public Weapon? Weapon2 { get; set; }// = new() { Name = "AXE", Damage = 3 };
        public Scroll? Scroll1 { get; set; }// = new() { Name = "ATTACK_SCROLL" };
        public Scroll? Scroll2 { get; set; }// = new() { Name = "HEALING_SCROLL" };
        public Scroll? Scroll3 { get; set; }
        public bool Key { get; set; }// = true;
    }
}