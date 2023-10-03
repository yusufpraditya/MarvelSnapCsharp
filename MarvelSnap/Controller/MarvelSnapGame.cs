namespace MarvelSnap;

public class MarvelSnapGame
{
	private IPlayer _player1, _player2;
	private Dictionary<IPlayer, Deck> _decks = new();
	private Dictionary<ArenaId, Arena> _arenas = new();
	private Dictionary<IPlayer, bool> _playerTurn = new();
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	public Action<int>? OnTurnChanged;
	public int Turn { get; set; } = 1;
	public int MaxTurn { get; set; } = 6;
	
	public MarvelSnapGame(IPlayer player1, IPlayer player2) 
	{
		_player1 = player1;
		_player2 = player2;
		
		_playerTurn.Add(_player1, true);
		_playerTurn.Add(_player2, false);
		
		Arena arena1 = new(ArenaId.Arena1, _player1, _player2);
		Arena arena2 = new(ArenaId.Arena2, _player1, _player2);
		Arena arena3 = new(ArenaId.Arena3, _player1, _player2);
		_arenas[ArenaId.Arena1] = arena1;
		_arenas[ArenaId.Arena2] = arena2;
		_arenas[ArenaId.Arena3] = arena3;
		
		// test
		AntMan antman1 = new(CharacterType.AntMan, 0, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman2 = new(CharacterType.AntMan, 1, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman3 = new(CharacterType.AntMan, 2, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman4 = new(CharacterType.AntMan, 3, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman5 = new(CharacterType.AntMan, 4, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		
		Deck deck1 = new(_player1.Id, player1.Name);
		Deck deck2 = new(_player2.Id, player2.Name);
		
		_decks.Add(_player1, deck1);
		_decks.Add(_player2, deck2);
		
		_decks[_player1].Add(antman1);
		_decks[_player1].Add(antman2);
		_decks[_player1].Add(antman3);
		_decks[_player1].Add(antman4);
		_decks[_player1].Add(antman5);
		
		_decks[_player2].Add(antman1);
		_decks[_player2].Add(antman2);
		_decks[_player2].Add(antman3);
		_decks[_player2].Add(antman4);
		_decks[_player2].Add(antman5);
		
		_decks[_player1].Shuffle();
		_decks[_player2].Shuffle();
		
		OnTurnChanged = NotifyTurnChanged;
	}
	
	public void SetPlayerTurn(IPlayer player) 
	{
		if (player == _player1) 
		{
			_playerTurn[_player1] = true;
			_playerTurn[_player2] = false;
		} 
		else 
		{
			_playerTurn[_player1] = false;
			_playerTurn[_player2] = true;
		}
	}
	
	public IPlayer GetPlayerTurn() 
	{
		if (_playerTurn[_player1] == true) return _player1;
		else return _player2;
	}
	
	public bool NextTurn() 
	{
		if (Turn <= MaxTurn) 
		{
			Turn += 1;
			return true;
		}
		else 
		{
			return false;
		}
	}
	
	public void EndTurn(IPlayer player) 
	{

	}
	
	public CharacterCard? DrawCard(IPlayer player) 
	{
		return _decks[player].Draw();
	}
	
	public ArenaId GetArenaId(IPlayer player, CharacterCard card) 
	{
		foreach (var kvp in _arenas) 
		{
			if (kvp.Value.GetCards(player).Contains(card)) 
			{
				return kvp.Key;
			}
		}
		return ArenaId.Empty;
	}
	
	public List<CharacterCard> GetArenaCards(IPlayer player, ArenaId id) 
	{
		return _arenas[id].GetCards(player);
	}
	
	public bool PutCardInArena(IPlayer player, ArenaId id, CharacterCard card) 
	{
		if (!HasCardInArena(player, id, card)) 
		{
			bool success = _arenas[id].PutCard(player, card);
			if (success) return true;
			else return false;
		}
		return false;
	}
	
	private bool HasCardInArena(IPlayer player, ArenaId id, CharacterCard card) 
	{
		if (_arenas[id].GetCards(player).Contains(card)) return true;
		else return false;
	}
	
	public void NotifyTurnChanged(int turn) 
	{
		
	}
}