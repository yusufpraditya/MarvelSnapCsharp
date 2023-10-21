using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace MarvelSnap;

public class MarvelSnapGame
{
	private ILogger<MarvelSnapGame>? _log;
	private IPlayer _player1, _player2;
	private List<LocationCard> _locations = new();
	private List<IPlayer> _players = new();
	private Dictionary<IPlayer, Deck> _decks = new();
	private Dictionary<ArenaType, Arena> _dictArenas = new();
	private List<Arena> _listArenas = new();
	private Dictionary<IPlayer, bool> _playerTurn = new();
	private Dictionary<IPlayer, bool> _playerHasPlayed = new();
	private Dictionary<int, List<Buff>> _energyBuffs = new();
	private int _baseEnergy = 1;
	private int _energySpent;
	private bool _hasStarted;
	private Dictionary<IPlayer, List<CharacterCard?>> _playerCardsInHand = new();
	private Dictionary<IPlayer, List<CharacterCard>> _playerCardsInArena = new();
	private Dictionary<IPlayer, List<CharacterCard>> _destroyedCards = new();
	private Dictionary<int, List<FutureTask>> _futureTasks = new();
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	public Action<int>? OnTurnChanged { get; set; }
	public Action<IPlayer?, Card>? OnCardRevealed { get; set; }
	public Action<IPlayer, CharacterCard>? OnCardPowerChanged { get; set; }
	public Action<IPlayer, Arena>? OnArenaPowerChanged { get; set; }
	public Action<IPlayer, CharacterCard>? OnEnergyCostChanged { get; set; }
	public Action<IPlayer, CharacterCard, CharacterCard>? OnCardDestroyed { get; set; }
	public Action<MarvelSnapGame, IPlayer?>? OnGameEnded { get; set; }
	public int Turn { get; set; } = 1;
	public int MaxTurn { get; set; } = 6;

	public MarvelSnapGame(IPlayer player1, IPlayer player2, ILogger<MarvelSnapGame>? log = null)
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
		_destroyedCards.Add(_player1, new());
		_destroyedCards.Add(_player2, new());

		_energyBuffs.Add(_player1.Id, new());
		_energyBuffs.Add(_player2.Id, new());

		_futureTasks.Add(_player1.Id, new());
		_futureTasks.Add(_player2.Id, new());

		AntMan antman = new AntMan(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		Medusa medusa = new Medusa(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
		Hawkeye hawkeye = new Hawkeye(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
		StarLord starLord = new StarLord(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
		Sentinel sentinel = new Sentinel(CharacterType.Sentinel, "Sentinel", "On Reveal: Add another Sentinel to your hand.", 2, 3, true);
		MisterFantastic misterFantastic = new(CharacterType.MisterFantastic, "Mister Fantastic", "Ongoing: Adjacent locations have +2 Power.", 3, 2, true);
		IronMan ironMan = new IronMan(CharacterType.IronMan, "Iron Man", "Ongoing: Your total Power is doubled at this location.", 5, 0, true);
		Hulk hulk = new Hulk(CharacterType.Hulk, "Hulk", "HULK SMASH!", 6, 12, false);
		Elektra elektra = new Elektra(CharacterType.Elektra, "Elektra", "On Reveal: Destroy a random enemy 1-Cost card at this location.", 1, 1, true);

		OnslaughtsCitadel onslaughtsCitadel = new OnslaughtsCitadel(LocationType.OnslaughtsCitadel, "Onslaught's Citadel", "Ongoing effects here are doubled.");
		DreamDimension dreamDimension = new DreamDimension(LocationType.DreamDimension, "Dream Dimension", "On turn 5, cards cost 1 more.");
		Kyln kyln = new Kyln(LocationType.Kyln, "Kyln", "You can't play cards here after turn 4.");
		Limbo limbo = new Limbo(LocationType.Limbo, "Limbo", "There is a turn 7 this game.");
		ProjectPegasus projectPegasus = new ProjectPegasus(LocationType.ProjectPegasus, "Project Pegasus", "+5 Energy this turn.");
		StarkTower starkTower = new StarkTower(LocationType.StarkTower, "Stark Tower", "After turn 5, give all cards here +2 Power.");

		_locations.Add(onslaughtsCitadel);
		_locations.Add(dreamDimension);
		_locations.Add(kyln);
		_locations.Add(limbo);
		_locations.Add(projectPegasus);
		_locations.Add(starkTower);

		Deck deck1 = new(_player1.Id, _player1.Name);
		Deck deck2 = new(_player2.Id, _player2.Name);

		_decks.Add(_player1, deck1);
		_decks.Add(_player2, deck2);

		_decks[_player1].Add(antman.DeepCopy());
		_decks[_player1].Add(elektra.DeepCopy());
		_decks[_player1].Add(hawkeye.DeepCopy());
		_decks[_player1].Add(hulk.DeepCopy());
		_decks[_player1].Add(ironMan.DeepCopy());
		_decks[_player1].Add(medusa.DeepCopy());
		_decks[_player1].Add(misterFantastic.DeepCopy());
		_decks[_player1].Add(sentinel.DeepCopy());
		_decks[_player1].Add(starLord.DeepCopy());

		_decks[_player2].Add(antman.DeepCopy());
		_decks[_player2].Add(elektra.DeepCopy());
		_decks[_player2].Add(hawkeye.DeepCopy());
		_decks[_player2].Add(hulk.DeepCopy());
		_decks[_player2].Add(ironMan.DeepCopy());
		_decks[_player2].Add(medusa.DeepCopy());
		_decks[_player2].Add(misterFantastic.DeepCopy());
		_decks[_player2].Add(sentinel.DeepCopy());
		_decks[_player2].Add(starLord.DeepCopy());

		_decks[_player1].Shuffle();
		_decks[_player2].Shuffle();

		ShuffleLocation();

		Arena arena1 = new(ArenaType.Arena1, _player1, _player2) { Location = _locations[0] };
		Arena arena2 = new(ArenaType.Arena2, _player1, _player2) { Location = _locations[1] };
		Arena arena3 = new(ArenaType.Arena3, _player1, _player2) { Location = _locations[2] };

		_listArenas.Add(arena1);
		_listArenas.Add(arena2);
		_listArenas.Add(arena3);

		_dictArenas[ArenaType.Arena1] = arena1;
		_dictArenas[ArenaType.Arena2] = arena2;
		_dictArenas[ArenaType.Arena3] = arena3;
		
		_log = log;
	}

	/// <summary>
	/// Starts the game by drawing initial cards for each player and reveals first location.
	/// </summary>
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
			
			_log?.LogInformation("Game initialized. First location ({0}) is revealed.", _locations[0].Name);
		}
	}

	/// <summary>
	/// Gets list of players that are currently playing the game.
	/// </summary>
	/// <returns>List of players.</returns>
	public List<IPlayer> GetPlayers()
	{
		return _players;
	}

	/// <summary>
	/// Gets the opponent of specified player.
	/// </summary>
	/// <param name="player">The player that will be used to check its opponent.</param>
	/// <returns>IPlayer of opponent.</returns>
	public IPlayer GetOpponent(IPlayer player)
	{
		if (!_players.Contains(player)) throw new Exception("Player does not exist!");
		if (_players[0] != player) return _players[0];
		return _players[1];
	}

	/// <summary>
	/// Shuffles locations for each game.
	/// </summary>
	private void ShuffleLocation()
	{
		for (int i = _locations.Count - 1; i > 0; i--)
		{
			int randomIndex = RandomNumberGenerator.GetInt32(i + 1);
			LocationCard randomLocation = _locations[randomIndex];
			_locations[randomIndex] = _locations[i];
			_locations[i] = randomLocation;
		}
	}

	/// <summary>
	/// Sets current player's turn.
	/// </summary>
	/// <param name="player">Player which will be set its turn.</param>
	public void SetPlayerTurn(IPlayer? player)
	{
		if (player == _player1)
		{
			_playerTurn[_player1] = true;
			_playerTurn[_player2] = false;
			_log?.LogInformation("Player turn is set to {0}.", _player1.Name);
		}
		else
		{
			_playerTurn[_player1] = false;
			_playerTurn[_player2] = true;
			_log?.LogInformation("Player turn is set to {0}.", _player2.Name);
		}
	}

	/// <summary>
	/// Checks if two players have played.
	/// </summary>
	/// <returns>true if both players have played; otherwise, false.</returns>
	public bool PlayersHavePlayed()
	{
		if (_playerHasPlayed[_player1] == true && _playerHasPlayed[_player2] == true) return true;
		else return false;
	}

	/// <summary>
	/// Gets current player's turn.
	/// </summary>
	/// <returns>Player of current turn.</returns>
	public IPlayer GetPlayerTurn()
	{
		if (_playerTurn[_player1] == true) return _player1;
		return _player2;
	}

	/// <summary>
	/// Attempts to get next player's turn.
	/// </summary>
	/// <param name="nextPlayer">Instance of next player.</param>
	/// <returns>true if next player exists; otherwise, false.</returns>
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
			if (Turn > MaxTurn)
			{
				OnGameEnded?.Invoke(this, GetPlayerWinner());
				_log?.LogInformation("Game has ended.");
				return false;
			}
			_locations[0].Ongoing(null, this);
			if (Turn >= 2)
			{
				_locations[1].OnReveal(null, this);
				_locations[1].Ongoing(null, this);
				_log?.LogInformation("Location 2 is revealed ({0}).", _locations[1].Name);
			}
			if (Turn >= 3)
			{
				_locations[2].OnReveal(null, this);
				_locations[2].Ongoing(null, this);
				_log?.LogInformation("Location 3 is revealed ({0})", _locations[2]. Name);
			}
			_baseEnergy = Turn;
			_energySpent = 0;
			_playerHasPlayed[_player1] = false;
			_playerHasPlayed[_player2] = false;
			SetPlayerTurn(_player1);

			if (_playerCardsInHand[_player1].Count < MaxCardInHand)
				DrawCard(_player1);
			if (_playerCardsInHand[_player2].Count < MaxCardInHand)
				DrawCard(_player2);
			
			_log?.LogInformation("{0} and {1} has drawn their new card.", _player1.Name, _player2.Name);

			List<IPlayer> revealers = GetRevealers();
			foreach (var revealer in revealers)
			{
				// https://code-maze.com/csharp-remove-elements-from-list-iteration/
				for (int i = _futureTasks[revealer.Id].Count - 1; i >= 0; i--)
				{
					bool status = _futureTasks[revealer.Id][i].Run(Turn);
					if (status) 
					{
						_log?.LogInformation("Running future task.");
						RemoveFutureTask(revealer.Id, _futureTasks[revealer.Id][i]);
						_log?.LogInformation("Future task removed.");
					}
				}
			}
			return true;
		}
		else
		{
			return false;
		}
	}

	public void EndTurn(IPlayer player)
	{
		_log?.LogInformation("{0} has ended the turn.", player.Name);
		_playerHasPlayed[player] = true;

		if (PlayersHavePlayed())
		{
			_log?.LogInformation("Start revealing each card that has been placed this turn.");
			List<IPlayer> revealers = GetRevealers();

			foreach (var revealer in revealers)
			{
				foreach (var card in _playerCardsInArena[revealer])
				{
					card.OnReveal(revealer, this);
					card.Ongoing(revealer, this);
					card.OnDestroyed(revealer, this);
					card.OnMoved(revealer, this);
				}
			}
		}

		bool nextPlayerStatus = TryGetNextPlayer(out IPlayer? nextPlayer);
		if (nextPlayerStatus) SetPlayerTurn(nextPlayer);
		NextTurn();
	}

	public List<IPlayer> GetRevealers()
	{
		List<IPlayer> revealers = new();
		IPlayer? firstRevealer = GetPlayerWinner();
		IPlayer secondRevealer;
		Random random = new();

		if (firstRevealer != null)
		{
			secondRevealer = GetOpponent(firstRevealer);
			revealers.Add(firstRevealer);
			revealers.Add(secondRevealer);
		}
		else
		{
			int randomIndex = random.Next(0, 1);
			revealers.Add(_players[randomIndex]);
			if (randomIndex == 0) revealers.Add(_players[1]);
			else revealers.Add(_players[0]);
		}
		return revealers;
	}

	public void AddPowerBuffToArena(int ownerId, ArenaType type, Buff buff)
	{
		_dictArenas[type].AddPowerBuff(ownerId, buff);
	}

	public bool RemovePowerBuffFromArena(int ownerId, ArenaType type, int buffId)
	{
		return _dictArenas[type].RemovePowerBuff(ownerId, buffId);
	}

	public int GetTotalPowerOfArena(IPlayer player, ArenaType type)
	{
		return _dictArenas[type].GetTotalPower(player);
	}

	public bool AddEnergyBuff(int ownerId, Buff buff)
	{
		if (!_energyBuffs.ContainsKey(ownerId)) return false;
		_energyBuffs[ownerId].Add(buff);
		return true;
	}

	public bool RemoveEnergyBuff(int ownerId, int buffId)
	{
		// removeall
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

	public int GetCurrentEnergy(IPlayer player)
	{
		// _baseEnergy = Turn;
		int currentEnergy = Turn;
		_energySpent = 0;
		foreach (var card in _playerCardsInArena[player])
		{
			if (card.CardTurn == Turn)
				_energySpent += card.GetCurrentEnergyCost(player.Id);
		}

		if (!_energyBuffs.ContainsKey(player.Id))
		{
			return _baseEnergy - _energySpent;
		}
		if (_energyBuffs[player.Id].Count > 0)
		{
			_energyBuffs[player.Id].Aggregate(currentEnergy, (acc, value) => value.Apply(acc));
			return currentEnergy - _energySpent;
		}
		else
		{
			return _baseEnergy - _energySpent;
		}
	}

	public bool DrawCard(IPlayer player)
	{
		CharacterCard? cardToDraw = _decks[player].Draw();
		if (cardToDraw != null)
		{
			_log?.LogInformation("Card {0} is drawn to {1}.", cardToDraw.Name, player.Name);
			AddCardInHand(player, cardToDraw);
			return true;
		}
		return false;
	}

	public bool AddCardInHand(IPlayer player, CharacterCard? card)
	{
		if (!_playerCardsInHand.ContainsKey(player)) return false;
		_playerCardsInHand[player].Add(card);
		_log?.LogInformation("Card {0} is added to {1}'s hand.", card?.Name, player.Name);
		return true;
	}

	public bool RemoveCardInHand(IPlayer player, CharacterCard card)
	{
		if (!_playerCardsInHand.ContainsKey(player)) return false;
		_playerCardsInHand[player].Remove(card);
		_log?.LogInformation("Card {0} is removed from {1}'s hand.", card.Name, player.Name);
		return true;
	}

	public bool HasCardInHand(IPlayer player, CharacterCard card)
	{
		if (!_playerCardsInHand[player].Contains(card)) return false;
		return true;
	}

	public List<CharacterCard?> GetHandCards(IPlayer player)
	{
		return _playerCardsInHand[player];
	}

	public Dictionary<IPlayer, List<CharacterCard?>> GetHandCardsForEachPlayer()
	{
		return _playerCardsInHand;
	}

	public void AddFutureTask(int ownerId, FutureTask task)
	{
		bool status = _futureTasks.TryAdd(ownerId, new List<FutureTask>() { task });
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

	public int GetLatestTaskId(int ownerId)
	{
		return _futureTasks[ownerId].Count - 1;
	}

	public int GetLatestEnergyBuffId(int ownerId)
	{
		return _energyBuffs[ownerId].Count - 1;
	}

	public int GetLatestArenaBuffId(IPlayer? player, ArenaType type)
	{
		return _dictArenas[type].GetLatestBuffId(player);
	}

	public List<LocationCard> GetLocations()
	{
		return _locations;
	}

	public bool DestroyCard(IPlayer player, CharacterCard card)
	{
		if (HasCardInArena(player, card.Arena, card) && card.IsRevealed)
		{
			bool success = _dictArenas[card.Arena].TakeCard(player, card);
			if (success)
			{
				card.IsDestroyed = true;
				card.OnDestroyed(player, this);
				_playerCardsInArena[player].Remove(card);
				_destroyedCards[player].Add(card);
				_log?.LogInformation("Card {0} owned by {1} is destroyed.", card.Name, player.Name);
				return true;
			}
		}
		return false;
	}

	public Dictionary<ArenaType, Arena> GetArenas()
	{
		return _dictArenas;
	}

	public List<Arena> GetListOfArenas()
	{
		return _listArenas;
	}

	public List<CharacterCard> GetArenaCards(IPlayer player)
	{
		List<CharacterCard> arenaCards = new();
		foreach (var kvp in _dictArenas)
		{
			arenaCards = arenaCards.Concat(_dictArenas[kvp.Key].GetCards(player)).ToList();
		}
		return arenaCards;
	}

	public List<CharacterCard> GetArenaCards(IPlayer player, ArenaType type)
	{
		return _dictArenas[type].GetCards(player);
	}

	public IEnumerable<CharacterCard> GetArenaCards(IPlayer player, LocationCard location) // return ienumerable
	{
		foreach (var kvp in _dictArenas)
		{
			if (kvp.Value.Location == location)
			{
				return _dictArenas[kvp.Key].GetCards(player);
			}
		}
		return Enumerable.Empty<CharacterCard>();
	}

	public Dictionary<IPlayer, List<CharacterCard>> GetArenaCardsForEachPlayer()
	{
		return _playerCardsInArena;
	}

	public bool PutCardInArena(IPlayer player, ArenaType type, CharacterCard card)
	{
		if (HasCardInArena(player, type, card)) return false;
		
		bool success = _dictArenas[type].PutCard(player, card);
		
		if (!success) return false;
		_playerCardsInArena[player].Add(card);
		card.Arena = type;
		card.CardTurn = Turn;
		_log?.LogInformation("Card {0} is placed at {1}.", card.Name, card.Arena);
		RemoveCardInHand(player, card);
		return true;
	}

	public bool TakeCardFromArena(IPlayer player, ArenaType type, CharacterCard card)
	{
		if (!HasCardInArena(player, type, card) || card.IsRevealed) return false;
		
		bool success = _dictArenas[type].TakeCard(player, card);
		
		if (!success) return false;
		_playerCardsInArena[player].Remove(card);
		_log?.LogInformation("Card {0} is taken from {1}", card.Name, card.Arena);
		AddCardInHand(player, card);
		return true;
	}

	private bool HasCardInArena(IPlayer player, ArenaType type, CharacterCard card)
	{
		return _dictArenas[type].GetCards(player).Contains(card);
	}

	public IPlayer? GetPlayerWinner()
	{
		Dictionary<IPlayer, List<int>> playerScores = new()
		{
			{ _player1, new() },
			{ _player2, new() }
		};

		List<int> result = new();

		int player1Count = 0;
		int player2Count = 0;

		foreach (var player in _players)
		{
			foreach (var arena in _listArenas)
			{
				playerScores[player].Add(arena.GetTotalPower(player));
			}
		}

		result.Add(playerScores[_player1][0] - playerScores[_player2][0]);
		result.Add(playerScores[_player1][1] - playerScores[_player2][1]);
		result.Add(playerScores[_player1][2] - playerScores[_player2][2]);

		foreach (var score in result)
		{
			if (score > 0) player1Count += 1;
			else if (score < 0) player2Count += 1;
		}

		if (player1Count == player2Count)
		{
			if (result.Sum() > 0) player1Count += 1;
			else if (result.Sum() < 0) player2Count += 1;
		}

		if (player1Count > player2Count) return _player1;
		else if (player2Count > player1Count) return _player2;
		return null;
	}

	public void NotifyCardRevealed(IPlayer? player, Card card)
	{
		_log?.LogInformation("Notify revealed card to listeners.");
		OnCardRevealed?.Invoke(player, card);
	}

	public void NotifyPowerChanged(IPlayer player, CharacterCard card)
	{
		_log?.LogInformation("Notify card's power changes to listeners.");
		OnCardPowerChanged?.Invoke(player, card);
	}

	public void NotifyArenaPowerChanged(IPlayer player, Arena arena)
	{
		_log?.LogInformation("Notify arena's power changes to listeners.");
		OnArenaPowerChanged?.Invoke(player, arena);
	}

	public void NotifyEnergyCostChanged(IPlayer player, CharacterCard card)
	{
		_log?.LogInformation("Notify energy cost changes to listeners.");
		OnEnergyCostChanged?.Invoke(player, card);
	}

	public void NotifyCardDestroyed(IPlayer player, CharacterCard destroyer, CharacterCard target)
	{
		_log?.LogInformation("Notify destroyed card to listeners.");
		OnCardDestroyed?.Invoke(player, destroyer, target);
	}
}