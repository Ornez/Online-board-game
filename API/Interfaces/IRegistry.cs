namespace API.Interfaces
{
    public interface IRegistry<TItem>
    {
        void AddItem(TItem item, int itemId);
        void RemoveItem(TItem item);
        void RemoveById(int id);
        int GetIdByItem(TItem item);
        TItem GetItemById(int id);
        bool ContainsItem(TItem item);
        bool ContainsId(int id);
    }
}