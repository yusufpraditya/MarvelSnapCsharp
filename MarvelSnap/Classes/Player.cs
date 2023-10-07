namespace MarvelSnap;

public class Player
{
	public int Id { get; set; }
	public string? Name { get; set; }
	
	public Player(int id) 
	{
		Id = id;
	}
	
	public Player(int id, string name) 
	{
		Id = id;
		Name = name;
	}

	public override int GetHashCode()
	{
		return Id;
	}

    public override bool Equals(object? obj)
    {
        return obj is Player player && player.Id == Id;
    }
}
