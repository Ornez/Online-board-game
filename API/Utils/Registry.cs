using API.Interfaces;

namespace API.Utils;
    
public class Registry<TItem> : IRegistry<TItem>
{
    private readonly List<TItem> items = new List<TItem>();
    private readonly List<int> itemIds = new List<int>();

    public void AddItem(TItem item, int itemId) 
    {
        items.Add(item);
        itemIds.Add(itemId);
    }

    public void RemoveItem(TItem item) 
    {
        int itemIndex = items.IndexOf(item);
        if (itemIndex > 0 && itemIndex < items.Count) 
        {
            itemIds.RemoveAt(itemIndex);
            items.RemoveAt(itemIndex);
            return;
        }
        throw new ArgumentException($"Item: {item.ToString()} not found in registry.");
    }

    public void RemoveById(int id)
    {
        int index = itemIds.IndexOf(id);
        if (index > 0 && index < items.Count) 
        {
            itemIds.RemoveAt(index);
            items.RemoveAt(index);
            return;
        }
        throw new ArgumentException($"Id: {id} not found in registry.");
    }

    public int GetIdByItem(TItem item)
    {
        return itemIds[items.IndexOf(item)];
    }

    public TItem GetItemById(int id)
    {
        return items[itemIds.IndexOf(id)];
    }

    public bool ContainsItem(TItem item)
    {
        return items.Contains(item);
    }

    public bool ContainsId(int id)
    {
        return itemIds.Contains(id);
    }
}
