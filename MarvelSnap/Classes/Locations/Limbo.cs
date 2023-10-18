namespace MarvelSnap;

public class Limbo : LocationCard
{
	public Limbo(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		controller.NotifyCardRevealed(null, this);
		controller.MaxTurn = 7;
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
