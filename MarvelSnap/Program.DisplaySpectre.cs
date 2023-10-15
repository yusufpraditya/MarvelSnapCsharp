using System.Text;
using MarvelSnap;
using Spectre.Console;

namespace Program;

public partial class Program
{
	private static int _actionSelector = 0;
	private static int _locationSelector = -1;
	private static int _characterSelector = -1;
	private static int _selectedCard = -1;
	private static string _cardInfo = "";
	private static bool _isTakeCard = false;
	private static ConsoleKey key;
	
	static void DisplaySpectre(MarvelSnapGame game, Player player1, Player player2) 
	{
		Console.CursorVisible = false;
		
		game.SetPlayerName(player1, "Player 1");
		game.SetPlayerName(player2, "Player 2");
		game.Start();
		game.SetGameStatus(GameStatus.SelectAction);
		
		while (game.GetGameStatus() != GameStatus.GameEnded) 
		{
			Player player = game.GetPlayerTurn();
			
			Task task = Task.Run(() => GetInputKey(game, player));

			AnsiConsole.Write(MarvelSnapLayout(game));
			AnsiConsole.Write(CharacterInput(game, player));
			AnsiConsole.Write(CardInfo(game, player));
			AnsiConsole.Write(ActionInput());
			
			task.Wait();
			
			if (game.Turn > game.MaxTurn) game.SetGameStatus(GameStatus.GameEnded);
		}
	}
	
	static async Task GetInputKey(MarvelSnapGame game, Player player) 
	{
		await Task.Run(() => 
		{
			GameStatus status = game.GetGameStatus();
			List<Arena> arenas = game.GetListOfArenas();
			List<CharacterCard> handCards = game.GetHandCards(player);
			List<CharacterCard> arenaCards = game.GetArenaCardsForEachPlayer()[player];
			List<LocationCard> locationCards = game.GetLocations();
			key = Console.ReadKey(true).Key;
			
			switch (key) 
			{
				case ConsoleKey.LeftArrow:
					if (status == GameStatus.SelectAction) 
					{
						_actionSelector = _actionSelector <= 0 ? 0 : _actionSelector - 1;
					} 
					else if (status == GameStatus.SelectCharacter) 
					{
						_characterSelector = _characterSelector <= 0 ? 0 : _characterSelector - 1;
					}
					else if (status == GameStatus.SelectLocation) 
					{
						_locationSelector = _locationSelector <= 0 ? 0 : _locationSelector - 1;
					}
					break;
				case ConsoleKey.RightArrow:
					if (status == GameStatus.SelectAction) 
					{
						_actionSelector = _actionSelector >= 2 ? 2 : _actionSelector + 1;
					} 
					else if (status == GameStatus.SelectCharacter) 
					{
						if (_isTakeCard) 
						{
							_characterSelector = _characterSelector >= arenaCards.Count - 1 ? arenaCards.Count - 1 : _characterSelector + 1;
						}
							
						else 
						{
							_characterSelector = _characterSelector >= handCards.Count - 1 ? handCards.Count - 1 : _characterSelector + 1;
						}
							
					}
					else if (status == GameStatus.SelectLocation) 
					{
						_locationSelector = _locationSelector >= 2 ? 2 : _locationSelector + 1;
					}
					break;
				case ConsoleKey.Spacebar or ConsoleKey.Enter:
					if (status == GameStatus.SelectAction) 
					{
						if (_actionSelector == 0) 
						{
							_isTakeCard = false;
							game.SetGameStatus(GameStatus.SelectCharacter);
						}
						else if (_actionSelector == 1) 
						{
							_isTakeCard = true;
							game.SetGameStatus(GameStatus.SelectCharacter);
						}
						else 
						{
							game.EndTurn(player);
						}
						
						if ((handCards.Count > 0 || arenaCards.Count > 0) && _actionSelector != 2) 
						{
							SetCharacterSelector();
						}
						else if (_actionSelector == 2) 
						{
							SetNone();
						}
						else 
						{
							game.SetGameStatus(GameStatus.SelectAction);
						}
					} 
					else if (status == GameStatus.SelectCharacter) 
					{
						_selectedCard = _characterSelector;
						if (_isTakeCard) 
						{
							game.TakeCardFromArena(player, arenaCards[_selectedCard].Arena, arenaCards[_selectedCard]);
							game.SetGameStatus(GameStatus.SelectAction);
							SetActionSelector();
						}
							
						else 
						{
							if (game.GetCurrentEnergy(player) >= handCards[_selectedCard].GetCurrentEnergyCost(player.Id)) 
							{
								game.SetGameStatus(GameStatus.SelectLocation);
								SetLocationSelector();
							}
						}
					}
					else if (status == GameStatus.SelectLocation) 
					{
						if (game.GetCurrentEnergy(player) >= handCards[_selectedCard].GetCurrentEnergyCost(player.Id)) 
						{
							if (arenas[_locationSelector].IsAvailable()) 
							{
								game.PutCardInArena(player, (ArenaType) _locationSelector, handCards[_selectedCard]);
								game.SetGameStatus(GameStatus.SelectAction);
								SetActionSelector();
							}
						}
					} 
					break;
				case ConsoleKey.Escape:
					SetActionSelector();
					game.SetGameStatus(GameStatus.SelectAction);
					break;
			}
		});
	}
	
	static void SetActionSelector() 
	{
		_actionSelector = 0;
		_locationSelector = -1;
		_characterSelector = -1;
	}
	
	static void SetLocationSelector() 
	{
		_locationSelector = 0;
		_actionSelector = -1;
		_characterSelector = -1;
	}
	
	static void SetCharacterSelector() 
	{
		_characterSelector = 0;
		_locationSelector = -1;
		_actionSelector = -1;
	}
	
	static void SetNone() 
	{
		_characterSelector = -1;
		_locationSelector = -1;
		_actionSelector = -1;
	}
	
	static string Cursor(int x1, int x2) 
	{
		if (x1 == x2) return "■";
		else return " ";
	}
	static Table ActionInput() 
	{
		StringBuilder sb = new StringBuilder();
		sb.Append($"[yellow2][[{Cursor(_actionSelector, 0)}]] Put card(s)[/]");
		sb.Append($"[chartreuse1]   [[{Cursor(_actionSelector, 1)}]] Take card(s)[/]");
		sb.Append($"[orangered1]   [[{Cursor(_actionSelector, 2)}]] End Turn[/]");
		Table table = new Table()
			.Border(TableBorder.Square)
			.BorderColor(Color.White)
			.AddColumn(new TableColumn(sb.ToString()).Centered());
		return table.Expand();
	}
	
	static Table CharacterInput(MarvelSnapGame game, Player player) 
	{
		Table characters = new Table()
			.Border(TableBorder.Square)
			.BorderColor(Color.White)
			.Caption($"[blue]Energy: {game.GetCurrentEnergy(player)}[/] [white]Turn: {game.Turn}/{game.MaxTurn}[/]");
		List<CharacterCard> handCards = game.GetHandCards(player);
		Dictionary<Player, List<CharacterCard>> arenaCards = game.GetArenaCardsForEachPlayer();
		
		if (_isTakeCard) 
		{
			arenaCards[player] = arenaCards[player].Where(card => !card.IsRevealed).ToList();
			characters = CardsTable(game, characters, player, arenaCards[player]);
			characters.Title($"[chartreuse1]{player.Name}'s arena cards[/]");
		}
		else 
		{
			characters = CardsTable(game, characters, player, handCards);
			characters.Title($"[yellow2]{player.Name}'s hand cards[/]");
		}
		
		characters.Centered().Expand();
		
		Table table = new Table()
			.Border(TableBorder.None)
			.BorderColor(Color.White)
			.AddColumn(new TableColumn(characters));
		
		foreach (var column in table.Columns) 
		{
			column.Width = (int?)(Console.WindowWidth / table.Columns.Count * 0.8);
		}
 
		return table.Centered();
	}
	
	static Table CardsTable(MarvelSnapGame game, Table table, Player player, List<CharacterCard> cards) 
	{
		for(int i = 0; i < cards.Count; i++) 
		{
			StringBuilder sb = new StringBuilder();
			if (game.GetCurrentEnergy(player) < cards[i].GetCurrentEnergyCost(player.Id)) 
			{
				sb.AppendLine($"[grey]{cards[i].GetCurrentEnergyCost(player.Id)}[/] [grey]{cards[i].GetCurrentPower(player.Id)}[/]");
				sb.AppendLine($"[grey][[{Cursor(_characterSelector, i)}]][/]");
				sb.Append($"[grey]{cards[i].Name}[/]");
			}
				
			else 
			{
				sb.AppendLine($"[blue]{cards[i].GetCurrentEnergyCost(player.Id)}[/] [darkorange]{cards[i].GetCurrentPower(player.Id)}[/]");
				sb.AppendLine($"[[{Cursor(_characterSelector, i)}]]");
				sb.Append(cards[i].Name);
			}
			
			table.AddColumn(new TableColumn(sb.ToString()).Centered());
		}
		if (cards.Count == 0) table.AddColumn(new TableColumn("(Empty)").Centered());
		return table;
	}
	
	static Table CardInfo(MarvelSnapGame game, Player player) 
	{
		List<CharacterCard> handCards = game.GetHandCards(player);
		List<CharacterCard> arenaCards = game.GetArenaCardsForEachPlayer()[player];
		List<LocationCard> locationCards = game.GetLocations();
		
		if (_characterSelector >= 0) 
		{
			if (_isTakeCard && arenaCards.Count > 0)
				_cardInfo = arenaCards[_characterSelector].Name + "\n" + arenaCards[_characterSelector].Description;
			if (!_isTakeCard && handCards.Count > 0)
				_cardInfo = handCards[_characterSelector].Name + "\n" + handCards[_characterSelector].Description;
		}
		if (_locationSelector >= 0) 
		{
			if (locationCards[_locationSelector].IsRevealed)
				_cardInfo = locationCards[_locationSelector].Name + "\n" + locationCards[_locationSelector].Description;
			else
				_cardInfo =  "";
		}
		if (_actionSelector >= 0)
			_cardInfo = "";
			
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("[yellow]Card Info[/]");
		sb.Append(_cardInfo);
		Table table = new Table()
			.Border(TableBorder.Square)
			.BorderColor(Color.White)
			.AddColumn(new TableColumn(sb.ToString()).Centered());
		return table.Expand();
	}
	
	static Table MarvelSnapLayout(MarvelSnapGame game) 
	{
		Console.Clear();
		List<Player> players = game.GetPlayers();
		List<LocationCard> locations = game.GetLocations();
		
		List<Table> playerCards = new() { new(), new(), new(), new(), new(), new() };
		int index = 0;
		foreach (var player in players) 
		{
			foreach (var arena in Enum.GetValues<ArenaType>()) 
			{
				playerCards[index] = GetArenaCardsTable(player, game.GetArenaCards(player, arena));
				index++;
			}
		}
		
		var location1 = new Panel(GetLocationInfo(game, locations[0], 0));
		var location2 = new Panel(GetLocationInfo(game, locations[1], 1));
		var location3 = new Panel(GetLocationInfo(game, locations[2], 2));
			
		var table = new Table()
			.Centered()
			.Border(TableBorder.None)
			.BorderColor(Color.White)
			.Title("[yellow]Player 2[/]")
			.Caption("[yellow]Player 1[/]\n")
			.AddColumn(new TableColumn(playerCards[3]).Footer(playerCards[0]).Centered()) // Arena 1
			.AddColumn(new TableColumn(playerCards[4]).Footer(playerCards[1]).Centered()) // Arena 2
			.AddColumn(new TableColumn(playerCards[5]).Footer(playerCards[2]).Centered()) // Arena 3
			.AddRow(Align.Center(location1), Align.Center(location2), Align.Center(location3));

		return table.Expand();
	}
	
	static Table GetArenaCardsTable(Player player, List<CharacterCard> arenaCards) 
	{
		List<StringBuilder> sb = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		
		for (int i = 0; i < arenaCards.Count; i++)
		{
			sb[i].Clear();
			sb[i].AppendLine($"[bold blue]{arenaCards[i].GetCurrentEnergyCost(player.Id)}[/] [bold darkorange]{arenaCards[i].GetCurrentPower(player.Id)}[/]");
			sb[i].AppendLine($"{arenaCards[i].Name}");
		}
		
		Table table = new Table()
							.Centered()
							.Border(TableBorder.Square)
							.BorderColor(Color.Cyan1);
		
		table.AddColumn(new TableColumn(sb[0].ToString()).Footer(sb[1].ToString()).Centered())
				.AddColumn(new TableColumn(sb[2].ToString()).Footer(sb[3].ToString()).Centered());

		return table;
	}
	
	static string GetLocationInfo(MarvelSnapGame game, LocationCard location, int selector) 
	{
		List<Player> players = game.GetPlayers();
		List<Arena> arenas = game.GetListOfArenas();
		StringBuilder sb = new();
		
		if (arenas[selector].IsAvailable()) 
		{
			sb.AppendLine(game.GetTotalPowerOfArena(players[1], (ArenaType) selector).ToString());
			sb.AppendLine(location.IsRevealed ? location.Name : "(Unrevealed)");
			sb.AppendLine($"[[{Cursor(_locationSelector, selector)}]]");
			sb.Append(game.GetTotalPowerOfArena(players[0], (ArenaType) selector).ToString());
		}
		else 
		{
			sb.AppendLine($"[grey]{game.GetTotalPowerOfArena(players[1], (ArenaType) selector)}[/]");
			sb.AppendLine($"[grey]{(location.IsRevealed ? location.Name : "(Unrevealed)")}[/]");
			sb.AppendLine($"[grey][[{Cursor(_locationSelector, selector)}]][/]");
			sb.Append($"[grey]{game.GetTotalPowerOfArena(players[0], (ArenaType) selector)}[/]");
		}
		
		return sb.ToString();
	}
	
	static void GameEndedSpectre(Player? player) 
	{
		if (player != null) 
		{
			AnsiConsole.Write(
			new FigletText("Winner: " + player.Name)
				.Color(Color.Cyan1).Centered());
		}
		else 
		{
			AnsiConsole.Write(
			new FigletText("Game Draw!")
				.Color(Color.Green).Centered());
		}
	}
}