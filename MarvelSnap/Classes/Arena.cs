namespace MarvelSnap;

public class Arena
{
	Dictionary<Player, List<CharacterCard>> _playerCardsInArena = new();
	private bool _isAvailable = true;
	public const int MaxCardsInArena = 4;
	public ArenaType Id { get; set; }
	public LocationCard? Location { get; set;}
	
	public Arena(ArenaType id, Player player1, Player player2) 
	{
		Id = id;
		_playerCardsInArena.Add(player1, new());
		_playerCardsInArena.Add(player2, new());
	}
	
	public void SetLocation(LocationCard location) 
	{
		Location = location;
	}
	
	public bool PutCard(Player player, CharacterCard? card) 
	{
		if (_playerCardsInArena[player].Count < MaxCardsInArena) 
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

	public void SetAvailable(bool isAvailable) 
	{
		_isAvailable = isAvailable;
	}
	
	public bool IsAvailable() 
	{
		return _isAvailable;
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