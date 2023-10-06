namespace MarvelSnap;

public class MarvelSnapGame
{
	private IPlayer _player1, _player2;
	private List<LocationCard> _locations = new();
	private List<IPlayer> _players = new();
	private Dictionary<IPlayer, Deck> _decks = new();
	private Dictionary<ArenaType, Arena> _arenas = new();
	private Dictionary<IPlayer, bool> _playerTurn = new();
	private Dictionary<IPlayer, bool> _playerHasPlayed = new();
	private GameStatus _gameStatus = GameStatus.NotStarted;
	private Dictionary<IPlayer, List<CharacterCard>> _playerCardsInHand = new();
	private Dictionary<IPlayer, List<CharacterCard>> _playerCardsInArena = new();
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	public Action<int>? OnTurnChanged { get; set; }
	public Action<IPlayer, CharacterCard>? OnCardRevealed { get; set; }
	public int Turn { get; set; } = 1;
	public int MaxTurn { get; set; } = 6;
	
	public MarvelSnapGame(IPlayer player1, IPlayer player2) 
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
		
		AntMan antman1 = new(CharacterType.AntMan, 0, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman2 = new(CharacterType.AntMan, 1, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman3 = new(CharacterType.AntMan, 2, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman4 = new(CharacterType.AntMan, 3, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		AntMan antman5 = new(CharacterType.AntMan, 4, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		
		Hawkeye hawkeye1 = new(CharacterType.Hawkeye, 0, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye2 = new(CharacterType.Hawkeye, 1, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye3 = new(CharacterType.Hawkeye, 2, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye4 = new(CharacterType.Hawkeye, 3, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		Hawkeye hawkeye5 = new(CharacterType.Hawkeye, 4, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		
		LocationCard location1 = new(LocationType.OnslaughtsCitadel, 0, "Onslaught's Citadel", "Ongoing effects here are doubled.");
		LocationCard location2 = new(LocationType.DreamDimension, 1, "Dream Dimension", "On turn 5, cards cost 1 more.");
		LocationCard location3 = new(LocationType.Kyln, 2, "Kyln", "You can't play cards here after turn 4.");
		
		_locations.Add(location1);
		_locations.Add(location2);
		_locations.Add(location3);
		
		Arena arena1 = new(ArenaType.Arena1, location1.Id, _player1, _player2) { Location = location1 };
		Arena arena2 = new(ArenaType.Arena2, location2.Id, _player1, _player2) { Location = location2 };
		Arena arena3 = new(ArenaType.Arena3, location3.Id, _player1, _player2) { Location = location3 };
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
		
		// OnTurnChanged = NotifyTurnChanged;
		// OnCardRevealed = NotifyCardRevealed;
	}
	
	public void SetGameStatus(GameStatus gameStatus) 
	{
		_gameStatus = gameStatus;
	}
	
	public GameStatus GetGameStatus() 
	{
		return _gameStatus;
	}
	
	public void SetPlayerName(IPlayer player, string? name) 
	{
		player.Name = name;
	}

	public List<IPlayer> GetPlayers() 
	{
		return _players;
	}
	
	// private List<LocationCard> ShuffleLocation() 
	// {
		
	// }
	
	public void SetPlayerTurn(IPlayer? player) 
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
	
	public IPlayer GetPlayerTurn() 
	{
		if (_playerTurn[_player1] == true) return _player1;
		else return _player2;
	}
	
	public bool TryGetNextPlayer(out IPlayer? nextPlayer) 
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
	
	public void EndTurn(IPlayer player) 
	{
		_playerHasPlayed[player] = true;
	}
	
	public CharacterCard? DrawCard(IPlayer player) 
	{
		return _decks[player].Draw();
	}
	
	public List<LocationCard> GetLocations() 
	{
		return _locations;
	}
	
	public ArenaType GetArenaId(IPlayer player, CharacterCard card) 
	{
		foreach (var kvp in _arenas) 
		{
			if (kvp.Value.GetCards(player).Contains(card)) 
			{
				return kvp.Key;
			}
		}
		return ArenaType.Empty;
	}
	
	public List<CharacterCard> GetArenaCards(IPlayer player, ArenaType type) 
	{
		return _arenas[type].GetCards(player);
	}
	
	public List<CharacterCard> GetArenaCards(IPlayer player, LocationCard location) 
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
	
	public Dictionary<IPlayer, List<CharacterCard>> GetArenaCardsForEachPlayer() 
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
	
	public bool PutCardInArena(IPlayer player, ArenaType type, CharacterCard? card) 
	{
		if (!HasCardInArena(player, type, card)) 
		{
			bool success = _arenas[type].PutCard(player, card);
			if (success) return true;
			else return false;
		}
		return false;
	}
	
	private bool HasCardInArena(IPlayer player, ArenaType type, CharacterCard? card) 
	{
		if (_arenas[type].GetCards(player).Contains(card)) return true;
		else return false;
	}
	
	public void NotifyTurnChanged(int turn) 
	{
		// Turn 1
		// - Reveal Location 1
		// - Player starts with 3 cards + draw 1 card, and 1 energy
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 2
		// - Reveal Location 2
		// - Draw 1 card for each player and add 1 energy
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 3
		// - Reveal Location 3
		// - Draw 1 card for each player and add 1 energy
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 4
		// - Draw 1 card for each player and add 1 energy
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 5
		// - Draw 1 card for each player and add 1 energy
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 6
		// - Draw 1 card for each player and add 1 energy
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		
		//OnTurnChanged?.Invoke(turn);
		switch (turn) 
		{
			case 1:
				_locations[0].IsRevealed = true;
				break;
			case 2:
				_locations[1].IsRevealed = true;
				break;
			case 3:
				_locations[2].IsRevealed = true;
				break;
				
		}
		
		foreach (var kvp in _arenas) 
		{
			List<CharacterCard> player1Cards = GetArenaCards(_player1, kvp.Key);
			List<CharacterCard> player2Cards = GetArenaCards(_player2, kvp.Key);
			List<CharacterCard> playerCards = player1Cards.Concat(player2Cards).ToList();
			
			foreach (var card in playerCards) 
			{
				switch (card.CharacterType) 
				{
					case CharacterType.AntMan:
						AntMan antman = (AntMan) card;
						antman.Ongoing(_player1, this);
						break;
				}
			}
		}
	}
	
	public void NotifyCardRevealed(IPlayer player, CharacterCard card) 
	{
		switch (card.CharacterType) 
		{
			case CharacterType.Hawkeye:
				Hawkeye hawkeye = (Hawkeye) card;
				hawkeye.OnReveal(player, this);
				break;
		}
	}
}