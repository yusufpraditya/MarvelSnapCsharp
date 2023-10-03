namespace MarvelSnap;

public class Arena
{
	Dictionary<IPlayer, List<CharacterCard>> _playerCardsInArena = new();
	private Dictionary<IPlayer, bool> _isMovable = new();
	public const int MaxCardsInArena = 4;
	public ArenaId Id { get; set; }
	public LocationCard? Location { get; set;}
	
	public Arena(ArenaId id, IPlayer player1, IPlayer player2) 
	{
		Id = id;
		_playerCardsInArena.Add(player1, new());
		_playerCardsInArena.Add(player2, new());
	}
	
	public void SetLocation(LocationCard location) 
	{
		Location = location;
	}
	
	public bool PutCard(IPlayer player, CharacterCard card) 
	{
		if (_playerCardsInArena[player].Count < MaxCardsInArena) 
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
	
	public bool SetMovable(IPlayer player, bool isMovable) 
	{
		try 
		{
			_isMovable[player] = isMovable;
			return true;
		}
		catch (Exception) 
		{
			return false;
		}
	}
	
	public bool IsMovable(IPlayer player) {
		return _isMovable[player];
	}
	
	public int GetTotalPower(IPlayer player) 
	{
		int totalPower = 0;
		foreach (var card in _playerCardsInArena[player]) 
		{
			totalPower += card.GetCurrentPower();
		}
		return totalPower;
	}
}