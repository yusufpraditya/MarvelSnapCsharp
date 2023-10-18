namespace MarvelSnap;

public abstract class LocationCard : Card
{
	public LocationCard(LocationType id, string name, string description) : base((int)id, name, description)
	{
		
	}

	public override LocationCard ShallowCopy()
	{
		LocationCard other = (LocationCard) MemberwiseClone();
		return other;
	}
}
