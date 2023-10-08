namespace MarvelSnap;

public class MarvelSnapGame
{
	private Player _player1, _player2;
	private List<LocationCard> _locations = new();
	private List<Player> _players = new();
	private GameStatus _gameStatus = GameStatus.NotStarted;
	private Dictionary<Player, Deck> _decks = new();
	private Dictionary<ArenaType, Arena> _arenas = new();
	private Dictionary<Player, bool> _playerTurn = new();
	private Dictionary<Player, bool> _playerHasPlayed = new();
	private Dictionary<int, List<Buff>> _energyBuffs = new();
	private int _baseEnergy = 1;
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
		
		_energyBuffs.Add(_player1.Id, new());
		_energyBuffs.Add(_player2.Id, new());
		
		_futureTasks.Add(_player1.Id, new());
		_futureTasks.Add(_player2.Id, new());
		
		// test, will refactor later
		
		// AntMan antman1 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman2 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman3 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman4 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman5 = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		
		Medusa medusa1 = new(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		Medusa medusa2 = new(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		Medusa medusa3 = new(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		Medusa medusa4 = new(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		Medusa medusa5 = new(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		
		// Hawkeye hawkeye1 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		// Hawkeye hawkeye2 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		// Hawkeye hawkeye3 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		// Hawkeye hawkeye4 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		// Hawkeye hawkeye5 = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		
		StarLord starlord1 = new(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		StarLord starlord2 = new(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		StarLord starlord3 = new(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		StarLord starlord4 = new(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		StarLord starlord5 = new(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		
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
		
		_decks[_player1].Add(medusa1);
		_decks[_player1].Add(medusa2);
		_decks[_player1].Add(medusa3);
		_decks[_player1].Add(medusa4);
		_decks[_player1].Add(medusa5);
		
		_decks[_player2].Add(starlord1);
		_decks[_player2].Add(starlord2);
		_decks[_player2].Add(starlord3);
		_decks[_player2].Add(starlord4);
		_decks[_player2].Add(starlord5);
		
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
	
	public Player GetOpponent(Player player) 
	{
		if (_players[0] != player) return _players[0];
		else return _players[1];
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
			_baseEnergy = Turn;
			_playerHasPlayed[_player1] = false;
			_playerHasPlayed[_player2] = false;
			SetPlayerTurn(_player1);
			
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
			foreach (var p in _players) 
			{
				foreach (var card in _playerCardsInArena[p]) 
				{
					card.OnReveal(p, this);
					card.Ongoing(p, this);
					card.OnDestroyed(p, this);
					card.OnMoved(p, this);	
				}
				
				// https://code-maze.com/csharp-remove-elements-from-list-iteration/
				for (int i = _futureTasks[p.Id].Count - 1; i >= 0; i--) 
				{
					bool status = _futureTasks[p.Id][i].Run(Turn);
					if (status) RemoveFutureTask(p.Id, _futureTasks[p.Id][i]);
				}
			}
		}
	}
	
	public bool AddEnergyBuff(int ownerId, Buff buff) 
	{
		try 
		{
			_energyBuffs[ownerId].Add(buff);
			return true;
		}
		catch (Exception) 
		{
			return false;
		}
	}
	
	public bool RemoveEnergyBuff(int ownerId, int buffId) 
	{
		foreach (var buff in _energyBuffs[ownerId].ToList()) 
		{
			if (buff.Id == buffId) 
			{
				_energyBuffs[ownerId].Remove(buff);
				return true;
			}
		}
		return false;
	}
	
	public int GetCurrentEnergy(Player player) 
	{
		if (_energyBuffs[player.Id].Count > 0) 
		{
			int currentPower = 0;
			foreach (var buff in _energyBuffs[player.Id]) 
			{
				currentPower += buff.Apply(_baseEnergy);
			}
			return currentPower;
		}
		else return _baseEnergy;
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
	
	public bool RemoveFutureTask(int ownerId, FutureTask task) 
	{
		if (_futureTasks[ownerId].Count > 0) 
		{
			_futureTasks[ownerId].Remove(task);
			return true;
		}
		return false;
	}
	
	public int GetLatestTaskId() 
	{
		return _futureTasks.Count - 1;
	}
	
	public List<LocationCard> GetLocations() 
	{
		return _locations;
	}
	
	public Dictionary<ArenaType, Arena> GetArenas() 
	{
		return _arenas;
	}
	
	public List<CharacterCard> GetArenaCards(Player player, ArenaType type) 
	{
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
				_playerCardsInArena[player] = _arenas[type].GetCards(player);
				card.Location = type;
				RemoveCardInHand(player, card);
				return true;
			}
			else return false;
		}
		return false;
	}
	
	private bool HasCardInArena(Player player, ArenaType type, CharacterCard card) 
	{
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