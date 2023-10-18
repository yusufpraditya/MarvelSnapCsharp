namespace MarvelSnap;

public abstract class Card
{
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public bool IsRevealed { get; set; }
	public bool IsWithdrawable { get; set; }

	public Card(int id, string name, string description)
	{
		Id = id;
		Name = name;
		Description = description;
	}

	public Card(Card other)
	{
		Id = other.Id;
		Name = other.Name;
		Description = other.Description;
	}

	public abstract void OnReveal(IPlayer player, MarvelSnapGame controller);
	public abstract void Ongoing(IPlayer player, MarvelSnapGame controller);
	public abstract void OnDestroyed(IPlayer player, MarvelSnapGame controller);
	public abstract void OnMoved(IPlayer player, MarvelSnapGame controller);
	public abstract Card ShallowCopy();
	public abstract Card DeepCopy();
}