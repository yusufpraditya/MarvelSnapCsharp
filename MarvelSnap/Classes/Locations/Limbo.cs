using System.Text.Json;

namespace MarvelSnap;

public class Limbo : LocationCard
{
	public Limbo(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public Limbo()
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (!IsRevealed)
		{
			IsRevealed = true;
			controller.NotifyCardRevealed(null, this);
			controller.MaxTurn = 7;
		}
	}

	public override Limbo? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Limbo? card = JsonSerializer.Deserialize<Limbo>(json);
		return card;
	}

	public override void Ongoing(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}
}
