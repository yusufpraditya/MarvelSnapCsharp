namespace MarvelSnap;

public class CharacterCard : Card
{
	private Dictionary<int, List<Buff>> _buffs = new();
	
	public int BaseEnergyCost { get; set; }
	public int BasePower { get; set; }
	public bool HasAbility { get; set; }
	
	protected CharacterCard(CharacterId id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base((int)id, name, description)
	{
		BaseEnergyCost = baseEnergyCost;
		BasePower = basePower;
		HasAbility = hasAbility;
	}
	
	public int GetCurrentPower() 
	{

	}
}
