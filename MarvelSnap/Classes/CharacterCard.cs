namespace MarvelSnap;

public class CharacterCard : Card
{
	private List<Buff> _buffs = new();
	public int BaseEnergyCost { get; set; }
	public int BasePower { get; set; }
	public bool HasAbility { get; set; }
	public CharacterType CharacterType { get; set; }
	public bool IsRevealed { get; set; }
	
	public CharacterCard(CharacterType type, int id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description)
	{
		BaseEnergyCost = baseEnergyCost;
		BasePower = basePower;
		HasAbility = hasAbility;
		CharacterType = type;
	}
	
	public int GetCurrentPower() 
	{
		if (_buffs.Count > 0) 
		{
			_buffs.Sort();
			int currentPower = 0;
			foreach (var buff in _buffs) 
			{
				currentPower += buff.Apply(BasePower);
			}
			return currentPower;
		}
		else return BasePower;
	}
	
	public void AddBuff(Buff buff) 
	{
		_buffs.Add(buff);
	}
	
	// public void RemoveBuff(int buffId) 
	// {
	// 	foreach (var buff in _buffs) 
	// 	{
	// 		if (buff.Id == buffId) _buffs.Remove(buff);
	// 	}
	// }
}