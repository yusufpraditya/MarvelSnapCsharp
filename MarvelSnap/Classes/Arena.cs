namespace MarvelSnap;

public class Arena
{
	private Dictionary<Player, List<CharacterCard>> _playerCardsInArena = new();
	private Dictionary<int, List<Buff>> _powerBuffs = new();
	private bool _isAvailable = true;
	public const int MaxCardsInArena = 4;
	public ArenaType Id { get; set; }
	public LocationCard? Location { get; set;}
	
	public Arena(ArenaType id, Player player1, Player player2) 
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
	
	public bool PutCard(Player player, CharacterCard card) 
	{
		if (_isAvailable && _playerCardsInArena[player].Count < MaxCardsInArena) 
		{
			_playerCardsInArena[player].Add(card);
			return true;
		}
		return false;
	}
	
	public bool TakeCard(Player player, CharacterCard card) 
	{
		if (_playerCardsInArena[player].Count > 0) 
		{
			_playerCardsInArena[player].Remove(card);
			return true;
		}
		return false;
	}
	
	public List<CharacterCard> GetCards(Player player) 
	{
		return _playerCardsInArena[player];
	}
	
	public bool Contains(Player player, CharacterCard card) 
	{
		if (_playerCardsInArena[player].Contains(card)) return true;
		else return false;
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
		foreach (var buff in _powerBuffs[ownerId].ToList()) 
		{
			if (buff.Id == buffId)
			{
				_powerBuffs[ownerId].Remove(buff);
				return true;
			} 
		}
		return false;
	}
	
	public int GetLatestBuffId(Player player) 
	{
		return _powerBuffs[player.Id].Count - 1;
	}
	
	public int GetTotalPower(Player player) 
	{
		int totalPower = 0;
		foreach (var card in _playerCardsInArena[player]) 
		{
			totalPower += card.GetCurrentPower(player.Id);
		}
		return totalPower;
	}
}