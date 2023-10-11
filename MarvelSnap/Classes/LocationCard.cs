using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarvelSnap;

public class LocationCard : Card
{
	public LocationCard(LocationType id, string name, string description) : base((int)id, name, description)
	{
		
	}
	
	[JsonConstructor]
	public LocationCard() 
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

	public override LocationCard ShallowCopy()
	{
		LocationCard other = (LocationCard) MemberwiseClone();
		return other;
	}

	public override LocationCard? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		LocationCard? card = JsonSerializer.Deserialize<LocationCard>(json);
		return card;
	}
}
