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
	private static bool _isEnded = false;
	private static ConsoleKey _key;
	private static Dictionary<CharacterCard, string> _buffedCards = new();

	static void DisplaySpectre(MarvelSnapGame game, IPlayer player1, IPlayer player2)
	{
		Console.CursorVisible = false;
		game.SetPlayerName(player1, "Player 1");
		game.SetPlayerName(player2, "Player 2");
		game.Start();
		game.SetGameStatus(GameStatus.SelectAction);

		while (game.GetGameStatus() != GameStatus.GameEnded)
		{
			IPlayer player = game.GetPlayerTurn();

			Task task = Task.Run(() => GetInputKey(game, player));

			AnsiConsole.Write(MarvelSnapLayout(game));
			AnsiConsole.Write(CharacterInput(game, player));
			AnsiConsole.Write(CardInfo(game, player));
			AnsiConsole.Write(ActionInput());

			task.Wait();

			if (_isEnded)
				game.SetGameStatus(GameStatus.GameEnded);
		}
	}

	static async Task GetInputKey(MarvelSnapGame game, IPlayer player)
	{
		await Task.Run(() =>
		{
			GameStatus status = game.GetGameStatus();
			List<Arena> arenas = game.GetListOfArenas();
			List<CharacterCard> handCards = game.GetHandCards(player);
			List<CharacterCard> arenaCards = game.GetArenaCardsForEachPlayer()[player];
			List<LocationCard> locationCards = game.GetLocations();
			_key = Console.ReadKey(true).Key;

			switch (_key)
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
							if (arenaCards.Count > 0)
							{
								game.TakeCardFromArena(player, arenaCards[_selectedCard].Arena, arenaCards[_selectedCard]);
								game.SetGameStatus(GameStatus.SelectAction);
								SetActionSelector();
							}
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
								game.PutCardInArena(player, (ArenaType)_locationSelector, handCards[_selectedCard]);
								game.SetGameStatus(GameStatus.SelectAction);
								SetActionSelector();
							}
						}
					}
					break;
				case ConsoleKey.Escape:
					if (status == GameStatus.SelectLocation)
					{
						SetCharacterSelector();
						game.SetGameStatus(GameStatus.SelectCharacter);
					}
					else
					{
						SetActionSelector();
						game.SetGameStatus(GameStatus.SelectAction);
					}
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
			.Title("[[Space/Enter]] Select    [[Esc]] Back    [[Left/Right]] Move cursor")
			.AddColumn(new TableColumn(sb.ToString()).Centered());
		return table.Expand();
	}

	static Table CharacterInput(MarvelSnapGame game, IPlayer player)
	{
		Table characters = new Table()
			.Border(TableBorder.Square)
			.BorderColor(Color.White)
			.Caption($"[blue]Energy: {game.GetCurrentEnergy(player)}[/] | [white]Turn: {game.Turn}/{game.MaxTurn}[/] | [cyan]First revealer: {game.GetRevealers()[0].Name}[/]");
		List<CharacterCard> handCards = game.GetHandCards(player);
		Dictionary<IPlayer, List<CharacterCard>> arenaCards = game.GetArenaCardsForEachPlayer();

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

	static Table CardsTable(MarvelSnapGame game, Table table, IPlayer player, List<CharacterCard> cards)
	{
		for (int i = 0; i < cards.Count; i++)
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
				int energyCost = cards[i].GetCurrentEnergyCost(player.Id);
				if (energyCost > cards[i].BaseEnergyCost)
					sb.AppendLine($"[red]{energyCost}[/] [darkorange]{cards[i].GetCurrentPower(player.Id)}[/]");
				else if (energyCost == cards[i].BaseEnergyCost)
					sb.AppendLine($"[blue]{energyCost}[/] [darkorange]{cards[i].GetCurrentPower(player.Id)}[/]");
				else
					sb.AppendLine($"[green]{energyCost}[/] [darkorange]{cards[i].GetCurrentPower(player.Id)}[/]");
				sb.AppendLine($"[[{Cursor(_characterSelector, i)}]]");
				sb.Append(cards[i].Name);
			}

			table.AddColumn(new TableColumn(sb.ToString()).Centered());
		}
		if (cards.Count == 0) table.AddColumn(new TableColumn("(Empty)").Centered());
		return table;
	}

	static Table CardInfo(MarvelSnapGame game, IPlayer player)
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
				_cardInfo = "\n";
		}
		if (_actionSelector >= 0)
			_cardInfo = "\n";

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
		List<IPlayer> players = game.GetPlayers();
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

	static Table GetArenaCardsTable(IPlayer player, List<CharacterCard> arenaCards)
	{
		List<StringBuilder> sb = new() { new("\n(Empty)\n"), new("\n(Empty)\n"), new("\n(Empty)\n"), new("\n(Empty)\n") };

		for (int i = 0; i < arenaCards.Count; i++)
		{
			int energyCost = arenaCards[i].GetCurrentEnergyCost(player.Id);
			sb[i].Clear();
			if (energyCost > arenaCards[i].BaseEnergyCost)
				sb[i].AppendLine($"[red]{energyCost}[/] {(arenaCards[i].GetCurrentPower(player.Id) == arenaCards[i].BasePower ? "[darkorange]" : "[green]")}{arenaCards[i].GetCurrentPower(player.Id)}[/]");
			else if (energyCost == arenaCards[i].BaseEnergyCost)
				sb[i].AppendLine($"[blue]{energyCost}[/] {(arenaCards[i].GetCurrentPower(player.Id) == arenaCards[i].BasePower ? "[darkorange]" : "[green]")}{arenaCards[i].GetCurrentPower(player.Id)}[/]");
			else
				sb[i].AppendLine($"[green]{energyCost}[/] {(arenaCards[i].GetCurrentPower(player.Id) == arenaCards[i].BasePower ? "[darkorange]" : "[green]")}{arenaCards[i].GetCurrentPower(player.Id)}[/]");
			sb[i].AppendLine($"{arenaCards[i].Name}");
			if (_buffedCards.ContainsKey(arenaCards[i]))
			{
				sb[i].Append(_buffedCards[arenaCards[i]]);
			}
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
		List<IPlayer> players = game.GetPlayers();
		List<Arena> arenas = game.GetListOfArenas();
		List<CharacterCard> arenaCards1 = arenas[selector].GetCards(players[0]);
		List<CharacterCard> arenaCards2 = arenas[selector].GetCards(players[1]);
		StringBuilder sb = new();

		int player1TotalPower = game.GetTotalPowerOfArena(players[0], (ArenaType)selector);
		int player2TotalPower = game.GetTotalPowerOfArena(players[1], (ArenaType)selector);

		if (player2TotalPower > player1TotalPower)
			sb.AppendLine($"[darkorange]{player2TotalPower}[/] ({GetCardsPower(players[1], arenaCards2)}{GetPowerBuffs(players[1], arenas[selector])})");
		else
			sb.AppendLine($"{player2TotalPower} ({GetCardsPower(players[1], arenaCards2)}{GetPowerBuffs(players[1], arenas[selector])})");
		if (arenas[selector].IsAvailable())
		{
			sb.AppendLine(location.IsRevealed ? location.Name : "(Unrevealed)");
			sb.AppendLine($"[[{Cursor(_locationSelector, selector)}]]");
		}
		else
		{
			sb.AppendLine($"[grey]{(location.IsRevealed ? location.Name : "(Unrevealed)")}[/]");
			sb.AppendLine($"[grey][[{Cursor(_locationSelector, selector)}]][/]");
		}
		if (player1TotalPower > player2TotalPower)
			sb.Append($"[darkorange]{player1TotalPower}[/] ({GetCardsPower(players[0], arenaCards1)}{GetPowerBuffs(players[0], arenas[selector])})");
		else
			sb.Append($"{player1TotalPower} ({GetCardsPower(players[0], arenaCards1)}{GetPowerBuffs(players[0], arenas[selector])})");
		return sb.ToString();
	}

	static string GetCardsPower(IPlayer player, List<CharacterCard> cards)
	{
		string powerString = "";
		foreach (var card in cards)
		{
			if (card.GetCurrentPower(player.Id) > 0)
				powerString += $"[green]+{card.GetCurrentPower(player.Id)}[/]";
		}
		return powerString;
	}

	static string GetPowerBuffs(IPlayer player, Arena arena)
	{
		string buffString = "";
		List<Buff> powerBuffs = arena.GetPowerBuffs(player.Id);
		foreach (var buff in powerBuffs)
		{
			if (buff.Type == BuffType.Power)
				buffString += $"[yellow]{buff.GetSymbol()}{buff.Value}[/]";
		}
		return buffString;
	}

	static void CardPowerChangedSpectre(IPlayer player, CharacterCard card)
	{
		if (!_buffedCards.ContainsKey(card))
			_buffedCards.Add(card, "");

		List<Buff> powerBuffs = card.GetBuffs(player.Id);

		string baseWithBuff = $"[darkorange]{card.BasePower}[/]";
		foreach (var buff in powerBuffs)
		{
			if (buff.Type == BuffType.Power)
			{
				baseWithBuff += $"[green]{buff.GetSymbol()}{buff.Value}[/]";
			}
		}
		_buffedCards[card] = baseWithBuff;
	}

	static void GameEndedSpectre(MarvelSnapGame game, IPlayer? player)
	{
		AnsiConsole.Write(MarvelSnapLayout(game));
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
		_isEnded = true;
	}
}