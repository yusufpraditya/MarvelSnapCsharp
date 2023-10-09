namespace MarvelSnap;

public class LocationCard : Card
{
	public LocationCard(LocationType id, string name, string description) : base((int)id, name, description)
	{
	}

	public override void OnReveal(Player? player, MarvelSnapGame controller)
	{
	}

	public override void Ongoing(Player? player, MarvelSnapGame controller)
	{
	}

	public override void OnDestroyed(Player? player, MarvelSnapGame controller)
	{
	}

	public override void OnMoved(Player? player, MarvelSnapGame controller)
	{
	}

	public override LocationCard Copy()
	{
		LocationCard other = (LocationCard) MemberwiseClone();
		return other;
	}
}
