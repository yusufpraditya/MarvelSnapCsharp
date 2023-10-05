﻿namespace MarvelSnap;

public class MarvelSnapGame
{
	private IPlayer _player1, _player2;
	private List<LocationCard> _locations = new();
	private Dictionary<IPlayer, Deck> _decks = new();
	private Dictionary<ArenaId, Arena> _arenas = new();
	private Dictionary<IPlayer, bool> _playerTurn = new();
	private Dictionary<IPlayer, bool> _playerHasPlayed = new();
	// private Dictionary<IPlayer, CharacterCard> _playerCards = new();
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	public Action<int>? OnTurnChanged { get; set; }
	public Action<IPlayer, CharacterCard> OnCardRevealed { get; set; }
	public int Turn { get; set; } = 1;
	public int MaxTurn { get; set; } = 6;
	
	public MarvelSnapGame(IPlayer player1, IPlayer player2) 
	{
		_player1 = player1;
		_player2 = player2;
		
		_playerTurn.Add(_player1, true);
		_playerTurn.Add(_player2, false);
		_playerHasPlayed.Add(_player1, false);
		_playerHasPlayed.Add(_player2, false);
		
		Arena arena1 = new(ArenaId.Arena1, _player1, _player2);
		Arena arena2 = new(ArenaId.Arena2, _player1, _player2);
		Arena arena3 = new(ArenaId.Arena3, _player1, _player2);
		_arenas[ArenaId.Arena1] = arena1;
		_arenas[ArenaId.Arena2] = arena2;
		_arenas[ArenaId.Arena3] = arena3;
		
		// test
		// AntMan antman1 = new(CharacterType.AntMan, 0, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman2 = new(CharacterType.AntMan, 1, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman3 = new(CharacterType.AntMan, 2, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman4 = new(CharacterType.AntMan, 3, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		// AntMan antman5 = new(CharacterType.AntMan, 4, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
		
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
		
		Deck deck1 = new(_player1.Id, player1.Name);
		Deck deck2 = new(_player2.Id, player2.Name);
		
		_decks.Add(_player1, deck1);
		_decks.Add(_player2, deck2);
		
		_decks[_player1].Add(hawkeye1);
		_decks[_player1].Add(hawkeye2);
		_decks[_player1].Add(hawkeye3);
		_decks[_player1].Add(hawkeye4);
		_decks[_player1].Add(hawkeye5);
		
		_decks[_player2].Add(hawkeye1);
		_decks[_player2].Add(hawkeye2);
		_decks[_player2].Add(hawkeye3);
		_decks[_player2].Add(hawkeye4);
		_decks[_player2].Add(hawkeye5);
		
		_decks[_player1].Shuffle();
		_decks[_player2].Shuffle();
		
		OnTurnChanged = NotifyTurnChanged;
		OnCardRevealed = NotifyCardRevealed;
	}
	
	private List<LocationCard> ShuffleLocation() 
	{
		
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
	
	public bool HasPlayed()
	{
		if (_playerHasPlayed[_player1] == true && _playerHasPlayed[_player2] == true) return true;
		else return false;
	}
	
	public IPlayer GetPlayerTurn() 
	{
		if (_playerTurn[_player1] == true) return _player1;
		else return _player2;
	}
	
	public bool NextTurn() 
	{
		if (Turn <= MaxTurn && HasPlayed()) 
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
		// Turn 1
		// - Reveal Location 1
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 2
		// - Reveal Location 2
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 3
		// - Reveal Location 3
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 4
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 5
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
		// Turn 6
		// - Player can put card / take card / end turn
		// - Activate Ongoing / On Reveal (if the card has the ability)
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
			
			foreach (var card in player1Cards) 
			{
				switch (card.CharacterType) 
				{
					case CharacterType.AntMan:
						AntMan antman = (AntMan) card;
						antman.Ongoing(_player1, this);
						break;
				}
			}
			foreach (var card in player2Cards) 
			{
				switch (card.CharacterType) 
				{
					case CharacterType.AntMan:
						AntMan antman = (AntMan) card;
						antman.Ongoing(_player2, this);
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