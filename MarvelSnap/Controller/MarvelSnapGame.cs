namespace MarvelSnap;

public class MarvelSnapGame
{
	private Player _player1, _player2;
	private List<LocationCard> _locations = new();
	private List<Player> _players = new();
	private Dictionary<Player, Deck> _decks = new();
	private Dictionary<ArenaType, Arena> _arenas = new();
	private Dictionary<Player, bool> _playerTurn = new();
	private Dictionary<Player, bool> _playerHasPlayed = new();
	private GameStatus _gameStatus = GameStatus.NotStarted;
	private Dictionary<Player, List<CharacterCard>> _playerCardsInHand = new();
	private Dictionary<Player, List<CharacterCard>> _playerCardsInArena = new();
	private Dictionary<int, List<FutureTask>> _futureTasks = new();
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	public Action<int>? OnTurnChanged { get; set; }
	public Action<Player, Card>? OnCardRevealed { get; set; }
	public Action<Player, Card>? OnCardPowerChanged { get; set; }
	public Action<Player>? OnGameEnded { get; set; }
	public int Turn { get; set; } = 1;
	public int MaxTurn { get; set; } = 6;
	
	public MarvelSnapGame(Player player1, Player player2) 
	{
		_player1 = player1;
		_player2 = player2;
		
		_players.Add(_player1);
		_players.Add(_player2);
		
		_playerTurn.Add(_player1, true);
		_playerTurn.Add(_player2, false);
		_playerHasPlayed.Add(_player1, false);
		_playerHasPlayed.Add(_player2, false);
		
		_playerCardsInHand.Add(_player1, new());
		_playerCardsInHand.Add(_player2, new());
		_playerCardsInArena.Add(_player1, new());
		_playerCardsInArena.Add(_player2, new());
		
		// test, will refactor later
		
		AntMan antman1 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman2 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman3 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman4 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman5 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		
		Hawkeye hawkeye1 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye2 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye3 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye4 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye5 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		
		LocationCard location1 = new(LocationType.OnslaughtsCitadel, "Onslaught's Citadel", "Ongoing effects here are doubled.");
		LocationCard location2 = new(LocationType.DreamDimension, "Dream Dimension", "On turn 5, cards cost 1 more.");
		LocationCard location3 = new(LocationType.Kyln, "Kyln", "You can't play cards here after turn 4.");
		
		_locations.Add(location1);
		_locations.Add(location2);
		_locations.Add(location3);
		
		Arena arena1 = new(ArenaType.Arena1, _player1, _player2) { Location = location1 };
		Arena arena2 = new(ArenaType.Arena2, _player1, _player2) { Location = location2 };
		Arena arena3 = new(ArenaType.Arena3, _player1, _player2) { Location = location3 };
		_arenas[ArenaType.Arena1] = arena1;
		_arenas[ArenaType.Arena2] = arena2;
		_arenas[ArenaType.Arena3] = arena3;
		
		Deck deck1 = new(_player1.Id, _player1.Name);
		Deck deck2 = new(_player2.Id, _player2.Name);
		
		_decks.Add(_player1, deck1);
		_decks.Add(_player2, deck2);
		
		_decks[_player1].Add(hawkeye1);
		_decks[_player1].Add(hawkeye2);
		_decks[_player1].Add(hawkeye3);
		_decks[_player1].Add(hawkeye4);
		_decks[_player1].Add(hawkeye5);
		
		_decks[_player2].Add(antman1);
		_decks[_player2].Add(antman2);
		_decks[_player2].Add(antman3);
		_decks[_player2].Add(antman4);
		_decks[_player2].Add(antman5);
		
		_decks[_player1].Shuffle();
		_decks[_player2].Shuffle();
	}
	
	// public bool SetDeck(Player player, Deck deck) 
	// {
		
	// }
	
	public void SetGameStatus(GameStatus gameStatus) 
	{
		_gameStatus = gameStatus;
	}
	
	public GameStatus GetGameStatus() 
	{
		return _gameStatus;
	}
	
	public void SetPlayerName(Player player, string? name) 
	{
		player.Name = name;
	}

	public List<Player> GetPlayers() 
	{
		return _players;
	}
	
	// private List<LocationCard> ShuffleLocation() 
	// {
		
	// }
	
	public void SetPlayerTurn(Player? player) 
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
	
	public bool PlayersHavePlayed()
	{
		if (_playerHasPlayed[_player1] == true && _playerHasPlayed[_player2] == true) return true;
		else return false;
	}
	
	public Player GetPlayerTurn() 
	{
		if (_playerTurn[_player1] == true) return _player1;
		else return _player2;
	}
	
	public bool TryGetNextPlayer(out Player? nextPlayer) 
	{
		nextPlayer = null;
		if (!PlayersHavePlayed()) 
		{
			if (_playerHasPlayed[_player1]) nextPlayer = _player2;
			else nextPlayer = _player1;
			return true;
		}
		return false;
	}
	
	public bool NextTurn() 
	{
		if (Turn <= MaxTurn && PlayersHavePlayed()) 
		{
			Turn += 1;
			_playerHasPlayed[_player1] = false;
			_playerHasPlayed[_player2] = false;
			return true;
		}
		else 
		{
			return false;
		}
	}
	
	public void EndTurn(Player player) 
	{
		_playerHasPlayed[player] = true;
		if (PlayersHavePlayed()) 
		{
			List<CharacterCard> cards = new();
			foreach (var p in _players) 
			{
				foreach (var kvp in _arenas) 
				{
					cards = cards.Concat(_arenas[kvp.Key].GetCards(p)).ToList();
				}
			}
			
			Console.WriteLine("Count: " + cards.Count);
			Thread.Sleep(1000);
			
			foreach (var card in cards) 
			{
				Console.WriteLine("YOooo");
				Console.WriteLine(card.Name);
				Thread.Sleep(1000);
				
				card.Ongoing(player, this);
				card.OnReveal(player, this);
				card.OnDestroyed(player, this);
				card.OnMoved(player, this);
				foreach (var kvp in _futureTasks) 
				{
					foreach (var task in _futureTasks[kvp.Key]) 
					{
						bool status = task.Run(Turn);
						if (status) _futureTasks.Remove(kvp.Key);
					}
				}
			}
		}
		
	}
	
	public bool DrawCard(Player player) 
	{
		CharacterCard? cardToDraw = _decks[player].Draw();
		if (cardToDraw != null) 
		{
			AddCardInHand(player, cardToDraw);
			return true;
		}
		return false;
	}
	
	public bool AddCardInHand(Player player, CharacterCard card) 
	{
		try 
		{
			_playerCardsInHand[player].Add(card);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
	
	public bool RemoveCardInHand(Player player, CharacterCard card) 
	{
		try 
		{
			_playerCardsInHand[player].Remove(card);
			return true;
		}
		catch (Exception) 
		{
			return false;
		}
	}
	
	public bool HasCardInHand(Player player, CharacterCard card) 
	{
		if (_playerCardsInHand[player].Contains(card)) return true;
		else return false;
	}
	
	public List<CharacterCard> GetHandCards(Player player) 
	{
		return _playerCardsInHand[player];
	}
	
	public Dictionary<Player, List<CharacterCard>> GetHandCardsForEachPlayer() 
	{
		return _playerCardsInHand;
	}
 	
	public void AddFutureTask(int ownerId, FutureTask task) 
	{
		bool status = _futureTasks.TryAdd(ownerId, new List<FutureTask>(){ task });
		if (!status) _futureTasks[ownerId].Add(task);
	}
	
	// public bool RemoveFutureTask(int ownerId, int taskId) 
	// {
		
	// }
	
	public int GetLatestTaskId() 
	{
		return _futureTasks.Count - 1;
	}
	
	public List<LocationCard> GetLocations() 
	{
		return _locations;
	}
	
	public ArenaType? GetArenaId(Player player, CharacterCard card) 
	{
		foreach (var kvp in _arenas) 
		{
			Console.WriteLine(kvp.Value.Location.Name);
			Thread.Sleep(1000);
			
			if (kvp.Value.GetCards(player).Contains(card)) 
			{
				return kvp.Key;
			}
		}
		return null;
	}
	
	public List<CharacterCard> GetArenaCards(Player player, ArenaType type) 
	{
		Console.WriteLine(type);
		Thread.Sleep(1000);
		return _arenas[type].GetCards(player);
	}
	
	public List<CharacterCard> GetArenaCards(Player player, LocationCard location) 
	{
		foreach (var kvp in _arenas) 
		{
			if (kvp.Value.Location == location) 
			{
				return _arenas[kvp.Key].GetCards(player);
			}
		}
		return new List<CharacterCard>();
	}
	
	public Dictionary<Player, List<CharacterCard>> GetArenaCardsForEachPlayer() 
	{
		List<LocationCard> locations = GetLocations();
		foreach (var player in _players) 
		{
			List<CharacterCard> arenaCards = new();
			foreach (var location in locations) 
			{
				arenaCards = arenaCards.Concat(GetArenaCards(player, location)).ToList();
			}
			_playerCardsInArena[player] = arenaCards;
		}
		return _playerCardsInArena;
	}
	
	public bool PutCardInArena(Player player, ArenaType type, CharacterCard card) 
	{
		if (!HasCardInArena(player, type, card)) 
		{
			bool success = _arenas[type].PutCard(player, card);
			if (success) 
			{
				RemoveCardInHand(player, card);
				return true;
			}
			else return false;
		}
		return false;
	}
	
	private bool HasCardInArena(Player player, ArenaType type, CharacterCard card) 
	{
		Console.WriteLine("Type: " + type);
		Thread.Sleep(1000);
		if (_arenas[type].GetCards(player).Contains(card)) return true;
		else return false;
	}
	
	public void NotifyCardRevealed(Player player, Card card) 
	{
		OnCardRevealed?.Invoke(player, card);
	}
	
	public void NotifyPowerChanged(Player player, Card card) 
	{
		OnCardPowerChanged?.Invoke(player, card);
	}
}