namespace MarvelSnap;

public abstract class CharacterCard : Card
{
	private Dictionary<int, List<Buff>> _buffs = new();
	public int BaseEnergyCost { get; set; }
	public int BasePower { get; set; }
	public bool HasAbility { get; set; }
	public bool HasMoved { get; set; }
	public bool IsOngoingEffectActivated { get; set; }
	public bool IsDestroyed { get; set; }
	public int OngoingEffectActivationCount { get; set; } = 0;
	public int CardTurn { get; set; }
	public ArenaType Arena { get; set; }
	public const int MaxOngoingEffectActivation = 2;

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
		if (!_buffs.ContainsKey(ownerId))
		{
			return BasePower;
		}
		_buffs[ownerId].Sort();
		var buffs = _buffs[ownerId].Where(buff => buff.Type == BuffType.Power);
		if (buffs.Count() > 0)
		{
			int currentPower = 0;
			foreach (var buff in buffs)
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
			return currentPower;
		}
		return BasePower;
	}

	public int GetCurrentEnergyCost(int ownerId)
	{
		if (!_buffs.ContainsKey(ownerId))
		{
			return BaseEnergyCost;
		}

		var buffs = _buffs[ownerId].Where(buff => buff.Type == BuffType.Energy);

		int energyCost = 0;
		foreach (var buff in buffs)
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

		if (energyCost > 0) return energyCost;
		return BaseEnergyCost;
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

	public int GetLatestBuffId(IPlayer player)
	{
		if (_buffs.ContainsKey(player.Id))
		{
			return _buffs[player.Id].Count - 1;
		}
		return 0;
	}

	public override CharacterCard ShallowCopy()
	{
		CharacterCard other = (CharacterCard)MemberwiseClone();
		return other;
	}
}