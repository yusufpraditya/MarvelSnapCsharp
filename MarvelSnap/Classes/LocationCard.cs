namespace MarvelSnap;

public class LocationCard : Card
{
	public LocationType LocationType { get; set; }
	public LocationCard(LocationType type, int id, string name, string description) : base(id, name, description)
	{
		LocationType = type;
	}
}
