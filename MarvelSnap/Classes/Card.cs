namespace MarvelSnap;

public abstract class Card
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	
	protected Card(int id, string name, string description) 
	{
		Id = id;
		Name = name;
		Description = description;
	}
}
