public class Enemy
{
    public Enemy() { }

    public Enemy(int id, string name, int count, int order)
    {
        this.id = id;
        this.name = name;
        this.count = count;
        this.order = order;
    }

    public int id;
    public string name;
    public int count;
    public int order;
}