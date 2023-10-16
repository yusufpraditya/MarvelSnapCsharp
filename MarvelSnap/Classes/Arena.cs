namespace MarvelSnap;

public class Arena
{
	private Dictionary<IPlayer, List<CharacterCard>> _playerCardsInArena = new();
	private Dictionary<int, List<Buff>> _powerBuffs = new();
	private bool _isAvailable = true;
	public const int MaxCardsInArena = 4;
	public ArenaType Id { get; set; }
	public LocationCard? Location { get; set; }

	public Arena(ArenaType id, IPlayer player1, IPlayer player2)
	{
		Id = id;
		_playerCardsInArena.Add(player1, new());
		_playerCardsInArena.Add(player2, new());
		_powerBuffs.Add(player1.Id, new());
		_powerBuffs.Add(player2.Id, new());
	}

	public void SetLocation(LocationCard location)
	{
		Location = location;
	}

	public bool PutCard(IPlayer player, CharacterCard card)
	{
		if (_isAvailable && _playerCardsInArena[player].Count < MaxCardsInArena)
		{
			_playerCardsInArena[player].Add(card);
			return true;
		}
		return false;
	}

	public bool TakeCard(IPlayer player, CharacterCard card)
	{
		if (_playerCardsInArena[player].Count > 0)
		{
			_playerCardsInArena[player].Remove(card);
			return true;
		}
		return false;
	}

	public List<CharacterCard> GetCards(IPlayer player)
	{
		return _playerCardsInArena[player];
	}

	public bool Contains(IPlayer player, CharacterCard card)
	{
		return _playerCardsInArena[player].Contains(card);
	}

	public void SetAvailable(bool isAvailable)
	{
		_isAvailable = isAvailable;
	}

	public bool IsAvailable()
	{
		return _isAvailable;
	}

	public void AddPowerBuff(int ownerId, Buff buff)
	{
		_powerBuffs[ownerId].Add(buff);
	}

	public bool RemovePowerBuff(int ownerId, int buffId)
	{
		return _powerBuffs[ownerId].RemoveAll(b => b.Id == buffId) > 0;
	}

	public List<Buff> GetPowerBuffs(int ownerId)
	{
		return _powerBuffs[ownerId];
	}

	public int GetLatestBuffId(IPlayer player)
	{
		if (_powerBuffs.ContainsKey(player.Id))
		{
			return _powerBuffs[player.Id].Count - 1;
		}
		return 0;
	}

	public int GetTotalPower(IPlayer player)
	{
		int cardsPower = 0;
		foreach (var card in _playerCardsInArena[player])
		{
			cardsPower += card.GetCurrentPower(player.Id);
		}
		if (!_powerBuffs.ContainsKey(player.Id))
		{
			return cardsPower;
		}

		int totalPower = cardsPower;
		_powerBuffs[player.Id].Sort();
		foreach (var buff in _powerBuffs[player.Id])
		{
			if (buff.Operation == BuffOperation.Add)
			{
				totalPower += buff.Value;
			}
			else
			{
				totalPower *= buff.Value;
			}
		}
		return totalPower;
	}
}