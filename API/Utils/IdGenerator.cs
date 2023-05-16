namespace API.Utils;

public class IdGenerator
{
    private int lastGeneratedId = -1;
    private Queue<int> unusedIds = new();

    public IdGenerator(int startId = 0) 
    {
        lastGeneratedId = startId - 1;
    }

    public int GenerateNextId() 
    {
        if (unusedIds.Count > 0) 
        {
            return unusedIds.Dequeue();
        }

        lastGeneratedId++;
        return lastGeneratedId;
    }

    public void ReturnUnusedId(int id) 
    {
        unusedIds.Enqueue(id);
    }
}
