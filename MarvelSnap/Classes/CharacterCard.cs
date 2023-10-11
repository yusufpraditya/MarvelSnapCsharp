using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarvelSnap;

public class CharacterCard : Card
{
	private Dictionary<int, List<Buff>> _buffs = new();
	public int BaseEnergyCost { get; set; }
	public int BasePower { get; set; }
	public bool HasAbility { get; set; }
	public bool HasMoved { get; set; }
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
	
	[JsonConstructor]
	public CharacterCard() 
	{
		
	}
	
	public int GetCurrentPower(int ownerId) 
	{
		if (!_buffs.ContainsKey(ownerId)) 
		{
			return BasePower;
		}
		if (_buffs[ownerId].Count > 0) 
		{
			_buffs[ownerId].Sort();
			int currentPower = 0;
			foreach (var buff in _buffs[ownerId]) 
			{
				if (buff.Type == BuffType.Power) 
				{
					if (currentPower > 0) 
					{
						currentPower += buff.Apply(0);
					}
					else 
					{
						currentPower += buff.Apply(BasePower);
					}
				}
			}
			return currentPower;
		}
		else return BasePower;
	}
	
	public int GetCurrentEnergyCost(int ownerId) 
	{
		if (!_buffs.ContainsKey(ownerId)) 
		{
			return BaseEnergyCost;
		}
		if (_buffs[ownerId].Count > 0) 
		{
			int energyCost = 0;
			foreach (var buff in _buffs[ownerId]) 
			{
				if (buff.Type == BuffType.Energy) 
				{
					if (energyCost > 0) 
					{
						energyCost += buff.Apply(0);
					}
					else 
					{
						energyCost += buff.Apply(BaseEnergyCost);
					}
				}
			}
			return energyCost;
		}
		else return BaseEnergyCost;
	}
	
	public void AddBuff(int ownerId, Buff buff) 
	{
		bool status = _buffs.TryAdd(ownerId, new List<Buff>() { buff });
		if (!status) _buffs[ownerId].Add(buff);
	}
	
	public bool RemoveBuff(int ownerId, int buffId)
	{
		foreach (var buff in _buffs[ownerId].ToList()) 
		{
			if (buff.Id == buffId)
			{
				_buffs[ownerId].Remove(buff);
				return true;
			} 
		}
		return false;
	}
	
	public List<Buff> GetBuffs(int ownerId) 
	{
		_buffs[ownerId].Sort();
		return _buffs[ownerId];
	}
	
	public int GetLatestBuffId(Player player) 
	{
		if(_buffs.ContainsKey(player.Id)) 
		{
			return _buffs[player.Id].Count - 1;
		}
		else return 0;
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

	public override CharacterCard ShallowCopy()
	{
		CharacterCard other = (CharacterCard) MemberwiseClone();
		return other;
	}

	public override CharacterCard? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		CharacterCard? card = JsonSerializer.Deserialize<CharacterCard>(json);
		return card;
	}
}