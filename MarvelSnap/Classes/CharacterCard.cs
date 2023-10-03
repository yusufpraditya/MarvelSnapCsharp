namespace MarvelSnap;

public class CharacterCard : Card
{
	private List<Buff> _buffs = new();
	
	public int BaseEnergyCost { get; set; }
	public int BasePower { get; set; }
	public bool HasAbility { get; set; }
	
	protected CharacterCard(CharacterType type, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base((int)type, name, description)
	{
		BaseEnergyCost = baseEnergyCost;
		BasePower = basePower;
		HasAbility = hasAbility;
	}
	
	public int GetCurrentPower() 
	{
		_buffs.Sort();
		int currentPower = BasePower;
		foreach (var buff in _buffs) 
		{
			currentPower += buff.Apply(currentPower);
		}
		return currentPower;
	}
	
	public void AddBuff(Buff buff) 
	{
		_buffs.Add(buff);
	}
	
	public void RemoveBuff(int buffId) 
	{
		foreach (var buff in _buffs) 
		{
			if (buff.Id == buffId) _buffs.Remove(buff);
		}
	}
}