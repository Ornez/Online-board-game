namespace API.Modules.Game.Inventories;

public class Item
{
    public Item()
    {
    }
    
    public Item(string name, ItemType itemType)
    {
        Name = name;
        ItemType = itemType;
    }

    public string Name { get; set; } = "";
    public ItemType ItemType { get; set; }
}