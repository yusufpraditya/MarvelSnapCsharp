namespace MarvelSnap;

public class IPlayer
{
	public int Id { get; set; }
	public string? Name { get; set; }

	public IPlayer(int id)
	{
		Id = id;
	}

	public IPlayer(int id, string name)
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
		return obj is IPlayer player && player.Id == Id;
	}
}
