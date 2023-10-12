namespace MarvelSnap;

public class MarvelSnapGame
{
	private Player _player1, _player2;
	private List<LocationCard> _locations = new();
	private List<Player> _players = new();
	private GameStatus _gameStatus = GameStatus.NotStarted;
	private Dictionary<Player, Deck> _decks = new();
	private Dictionary<ArenaType, Arena> _dictArenas = new();
	private List<Arena> _listArenas = new();
	private Dictionary<Player, bool> _playerTurn = new();
	private Dictionary<Player, bool> _playerHasPlayed = new();
	private Dictionary<int, List<Buff>> _energyBuffs = new();
	private int _baseEnergy = 1;
	private bool _hasStarted;
	private Dictionary<Player, List<CharacterCard?>> _playerCardsInHand = new();
	private Dictionary<Player, List<CharacterCard>> _playerCardsInArena = new();
	private Dictionary<int, List<FutureTask>> _futureTasks = new();
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	public Action<int>? OnTurnChanged { get; set; }
	public Action<Player?, Card>? OnCardRevealed { get; set; }
	public Action<Player, CharacterCard>? OnCardPowerChanged { get; set; }
	public Action<Player, Arena>? OnArenaPowerChanged { get; set; }
	public Action<Player, CharacterCard>? OnEnergyCostChanged { get; set; }
	public Action<Player?>? OnGameEnded { get; set; }
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
		
		AntMan antman = new(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		Medusa medusa = new(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		Hawkeye hawkeye = new(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		StarLord starlord = new(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		Sentinel sentinel = new(CharacterType.Sentinel, "Sentinel", "On Reveal: Add another Sentinel to your hand.", 2, 3, true);
		MisterFantastic misterFantastic = new(CharacterType.MisterFantastic, "Mister Fantastic", "Ongoing: Adjacent locations have +2 Power.", 3, 2, true);
		
		OnslaughtsCitadel onslaughtsCitadel = new(LocationType.OnslaughtsCitadel, "Onslaught's Citadel", "Ongoing effects here are doubled.");
		DreamDimension dreamDimension = new(LocationType.DreamDimension, "Dream Dimension", "On turn 5, cards cost 1 more.");
		Kyln kyln = new(LocationType.Kyln, "Kyln", "You can't play cards here after turn 4.");
		
		_locations.Add(onslaughtsCitadel);
		_locations.Add(dreamDimension);
		_locations.Add(kyln);
		
		Arena arena1 = new(ArenaType.Arena1, _player1, _player2) { Location = onslaughtsCitadel };
		Arena arena2 = new(ArenaType.Arena2, _player1, _player2) { Location = dreamDimension };
		Arena arena3 = new(ArenaType.Arena3, _player1, _player2) { Location = kyln };
		
		_listArenas.Add(arena1);
		_listArenas.Add(arena2);
		_listArenas.Add(arena3);
		
		_dictArenas[ArenaType.Arena1] = arena1;
		_dictArenas[ArenaType.Arena2] = arena2;
		_dictArenas[ArenaType.Arena3] = arena3;
		
		Deck deck1 = new(_player1.Id, _player1.Name);
		Deck deck2 = new(_player2.Id, _player2.Name);
		
		_decks.Add(_player1, deck1);
		_decks.Add(_player2, deck2);
		
		// _decks[_player2].Add(antman.Copy());
		// _decks[_player2].Add(medusa.Copy());
		// _decks[_player2].Add(hawkeye.Copy());
		// _decks[_player2].Add(starlord.Copy());
		// _decks[_player2].Add(sentinel.Copy());
		// _decks[_player2].Add(misterFantastic.Copy());
		// _decks[_player2].Add(antman.Copy());
		// _decks[_player2].Add(medusa.Copy());
		// _decks[_player2].Add(hawkeye.Copy());
		// _decks[_player2].Add(starlord.Copy());
		// _decks[_player2].Add(sentinel.Copy());
		
		// _decks[_player1].Add(antman.Copy());
		// _decks[_player1].Add(medusa.Copy());
		// _decks[_player1].Add(hawkeye.Copy());
		// _decks[_player1].Add(starlord.Copy());
		// _decks[_player1].Add(sentinel.Copy());
		// _decks[_player1].Add(misterFantastic.Copy());
		// _decks[_player2].Add(antman.Copy());
		// _decks[_player2].Add(medusa.Copy());
		// _decks[_player2].Add(hawkeye.Copy());
		// _decks[_player2].Add(starlord.Copy());
		// _decks[_player2].Add(sentinel.Copy());
		
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		
		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(antman.DeepCopy());
		
		_decks[_player1].Shuffle();
		_decks[_player2].Shuffle();
	}
	
	public void Start() 
	{
		if (!_hasStarted) 
		{
			_hasStarted = true;
			DrawCard(_player1);
			DrawCard(_player1);
			DrawCard(_player1);
			DrawCard(_player2);
			DrawCard(_player2);
			DrawCard(_player2);
			
			_locations[0].OnReveal(null, this);
			_locations[0].Ongoing(null, this);
		}
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
			if (Turn == 2) 
			{
				_locations[1].OnReveal(null, this);
				_locations[1].Ongoing(null, this);
			}
			else if (Turn == 3) 
			{
				_locations[2].OnReveal(null, this);
				_locations[2].Ongoing(null, this);
			}
			_baseEnergy = Turn;
			_playerHasPlayed[_player1] = false;
			_playerHasPlayed[_player2] = false;
			SetPlayerTurn(_player1);
			DrawCard(_player1);
			DrawCard(_player2);
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
			if (Turn == MaxTurn) 
			{
				Player? winner = GetPlayerWinner();
				OnGameEnded?.Invoke(winner);
			}
		}
	}
	
	public void AddPowerBuffToArena(int ownerId, ArenaType type, Buff buff) 
	{
		_dictArenas[type].AddPowerBuff(ownerId, buff);
	}
	
	public bool RemovePowerBuffFromArena(int ownerId, ArenaType type, int buffId) 
	{
		return _dictArenas[type].RemovePowerBuff(ownerId, buffId);
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
	
	public bool AddCardInHand(Player player, CharacterCard? card) 
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
	
	public List<CharacterCard?> GetHandCards(Player player) 
	{
		return _playerCardsInHand[player];
	}
	
	public Dictionary<Player, List<CharacterCard?>> GetHandCardsForEachPlayer() 
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
		return _dictArenas;
	}
	
	public List<Arena> GetListOfArenas() 
	{
		return _listArenas;
	}
	
	public List<CharacterCard> GetArenaCards(Player player, ArenaType type) 
	{
		return _dictArenas[type].GetCards(player);
	}
	
	public List<CharacterCard> GetArenaCards(Player player, LocationCard location) 
	{
		foreach (var kvp in _dictArenas) 
		{
			if (kvp.Value.Location == location) 
			{
				return _dictArenas[kvp.Key].GetCards(player);
			}
		}
		return new List<CharacterCard>();
	}
	
	public Dictionary<Player, List<CharacterCard>> GetArenaCardsForEachPlayer() 
	{
		return _playerCardsInArena;
	}
	
	public bool PutCardInArena(Player player, ArenaType type, CharacterCard card) 
	{
		if (!HasCardInArena(player, type, card)) 
		{
			bool success = _dictArenas[type].PutCard(player, card);
			if (success) 
			{
				_playerCardsInArena[player] = _dictArenas[type].GetCards(player);
				card.Location = type;
				card.CardTurn = Turn;
				RemoveCardInHand(player, card);
				return true;
			}
			else return false;
		}
		return false;
	}
	
	public bool TakeCardFromArena(Player player, ArenaType type, CharacterCard card) 
	{
		if (HasCardInArena(player, type, card)) 
		{
			bool success = _dictArenas[type].TakeCard(player, card);
			if (success) 
			{
				_playerCardsInArena[player] = _dictArenas[type].GetCards(player);
				AddCardInHand(player, card);
			}
		}
		return false;
	}
	
	private bool HasCardInArena(Player player, ArenaType type, CharacterCard card) 
	{
		if (_dictArenas[type].GetCards(player).Contains(card)) return true;
		else return false;
	}
	
	public Player? GetPlayerWinner() 
	{
		List<Player?> playerWinner = new();
		int player1Count = 0;
		int player2Count = 0;
		foreach (var arena in _listArenas) 
		{
			playerWinner.Add(arena.GetWinner());
		}
		foreach (var player in playerWinner) 
		{
			if (player == _player1) player1Count += 1;
			else if (player == _player2) player2Count += 1;
		}
		if (player1Count > player2Count) return _player1;
		else if (player2Count > player1Count) return _player2;
		else return null;
	}
	
	public void NotifyCardRevealed(Player? player, Card card) 
	{
		OnCardRevealed?.Invoke(player, card);
	}
	
	public void NotifyPowerChanged(Player player, CharacterCard card) 
	{
		OnCardPowerChanged?.Invoke(player, card);
	}
	
	public void NotifyArenaPowerChanged(Player player, Arena arena) 
	{
		OnArenaPowerChanged?.Invoke(player, arena);
	}
	
	public void NotifyEnergyCostChanged(Player player, CharacterCard card) 
	{
		OnEnergyCostChanged?.Invoke(player, card);
	}
}