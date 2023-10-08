namespace MarvelSnap;

public class CharacterCard : Card
{
	private Dictionary<int, List<Buff>> _buffs = new();
	public int BaseEnergyCost { get; set; }
	public int BasePower { get; set; }
	public bool HasAbility { get; set; }
	public int CardTurn { get; set; }
	public ArenaType Location { get; set; }
	
	public CharacterCard(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base((int)id, name, description)
	{
		BaseEnergyCost = baseEnergyCost;
		BasePower = basePower;
		HasAbility = hasAbility;
	}
	
	public CharacterCard(int id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description)
	{
		BaseEnergyCost = baseEnergyCost;
		BasePower = basePower;
		HasAbility = hasAbility;
	}
	
	public int GetCurrentPower(int ownerId) 
	{
		if (_buffs[ownerId].Count > 0) 
		{
			_buffs[ownerId].Sort();
			int currentPower = 0;
			foreach (var buff in _buffs[ownerId]) 
			{
				currentPower += buff.Apply(BasePower);
			}
			return currentPower;
		}
		else return BasePower;
	}
	
	public void AddBuff(int ownerId, Buff buff) 
	{
		bool status = _buffs.TryAdd(ownerId, new List<Buff>() {buff});
		if (!status) _buffs[ownerId].Add(buff);
	}
	
	public void RemoveBuff(int ownerId, int buffId)
	{
		foreach (var kvp in _buffs) 
		{
			if (kvp.Key == ownerId) 
			{
				foreach (var buff in kvp.Value) 
				{
					if (buff.Id == buffId) _buffs[kvp.Key].Remove(buff);
				}
			}
		}
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
	}

	public override void Ongoing(Player player, MarvelSnapGame controller)
	{
	}

	public override void OnDestroyed(Player player, MarvelSnapGame controller)
	{
	}

	public override void OnMoved(Player player, MarvelSnapGame controller)
	{
	}
}