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

			AnsiConsole.Write(MarvelSnapLayout(game, player));
			AnsiConsole.Write(CharacterInput(game, player));
			AnsiConsole.Write(CardInfo());
			AnsiConsole.Write(ActionInput());
			
			if (game.Turn > game.MaxTurn) game.SetGameStatus(GameStatus.GameEnded);
			
			task.Wait();
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
						_cardInfo = "";
					} 
					else if (status == GameStatus.SelectCharacter) 
					{
						_characterSelector = _characterSelector <= 0 ? 0 : _characterSelector - 1;
						if (_isTakeCard)
							_cardInfo = arenaCards[_characterSelector].Name + ": " + arenaCards[_characterSelector].Description;
						else
							_cardInfo = handCards[_characterSelector].Name + ": " + handCards[_characterSelector].Description;
					}
					else if (status == GameStatus.SelectLocation) 
					{
						_locationSelector = _locationSelector <= 0 ? 0 : _locationSelector - 1;
						if (locationCards[_locationSelector].IsRevealed)
							_cardInfo = locationCards[_locationSelector].Name + ": " + locationCards[_locationSelector].Description;
						else
							_cardInfo =  "";
					}
					break;
				case ConsoleKey.RightArrow:
					if (status == GameStatus.SelectAction) 
					{
						_actionSelector = _actionSelector >= 2 ? 2 : _actionSelector + 1;
						_cardInfo = "";
					} 
					else if (status == GameStatus.SelectCharacter) 
					{
						if (_isTakeCard) 
						{
							_characterSelector = _characterSelector >= arenaCards.Count - 1 ? arenaCards.Count - 1 : _characterSelector + 1;
							_cardInfo = arenaCards[_characterSelector].Name + ": " + arenaCards[_characterSelector].Description;
						}
							
						else 
						{
							_characterSelector = _characterSelector >= handCards.Count - 1 ? handCards.Count - 1 : _characterSelector + 1;
							_cardInfo = handCards[_characterSelector].Name + ": " + handCards[_characterSelector].Description;
						}
							
					}
					else if (status == GameStatus.SelectLocation) 
					{
						_locationSelector = _locationSelector >= 2 ? 2 : _locationSelector + 1;
						if (locationCards[_locationSelector].IsRevealed)
							_cardInfo = locationCards[_locationSelector].Name + ": " + locationCards[_locationSelector].Description;
						else
							_cardInfo =  "";
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
						
						if (handCards.Count > 0 || arenaCards.Count > 0 && _actionSelector != 2) 
						{
							_actionSelector = -1;
							_characterSelector = 0;
							_locationSelector = -1;
							_cardInfo = handCards[_characterSelector].Name + ": " + handCards[_characterSelector].Description;
						}
						else if (_actionSelector == 2) 
						{
							_actionSelector = -1;
							_characterSelector = -1;
							_locationSelector = -1;
							_cardInfo = "";
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
							game.TakeCardFromArena(player, arenaCards[_selectedCard].Location, arenaCards[_selectedCard]);
							game.SetGameStatus(GameStatus.SelectAction);
							_actionSelector = 0;
							_characterSelector = -1;
							_locationSelector = -1;
						}
							
						else 
						{
							if (game.GetCurrentEnergy(player) >= handCards[_selectedCard].GetCurrentEnergyCost(player.Id)) 
							{
								game.SetGameStatus(GameStatus.SelectLocation);
								_actionSelector = -1;
								_characterSelector = -1;
								_locationSelector = 0;
								if (locationCards[_locationSelector].IsRevealed)
									_cardInfo = locationCards[_locationSelector].Name + ": " + locationCards[_locationSelector].Description;
								else
									_cardInfo =  "";
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
								_locationSelector = -1;
								_actionSelector = 0;
								_characterSelector = -1;
								_cardInfo = "";
							}
						}
					} 
					break;
				case ConsoleKey.Escape:
					game.SetGameStatus(GameStatus.SelectAction);
					break;
			}
		});
	}
	
	static string Cursor(int x1, int x2) 
	{
		if (x1 == x2) return "■";
		else return " ";
	}
	static Table ActionInput() 
	{
		StringBuilder sb = new StringBuilder();
		sb.Append($"[[{Cursor(_actionSelector, 0)}]] Put card(s)");
		sb.Append($"   [[{Cursor(_actionSelector, 1)}]] Take card(s)");
		sb.Append($"   [[{Cursor(_actionSelector, 2)}]] End Turn");
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
			.Caption("Energy: " + game.GetCurrentEnergy(player) +  " Turn: " + game.Turn.ToString() + "/" + game.MaxTurn.ToString())
			.Centered().Expand();
		List<CharacterCard> handCards = game.GetHandCards(player);
		Dictionary<Player, List<CharacterCard>> arenaCards = game.GetArenaCardsForEachPlayer();
		
		if (_isTakeCard) 
		{
			for(int i = 0; i < arenaCards[player].Count; i++) 
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine($"[blue]{arenaCards[player][i].GetCurrentEnergyCost(player.Id)}[/] [darkorange]{arenaCards[player][i].GetCurrentPower(player.Id)}[/]");
				sb.AppendLine($"[[{Cursor(_characterSelector, i)}]]");
				sb.Append(arenaCards[player][i].Name);
				characters.AddColumn(new TableColumn(sb.ToString()).Centered());
			}
			if (arenaCards[player].Count == 0) characters.AddColumn(new TableColumn("(Empty)").Centered());
			characters.Title(player.Name + "'s arena cards");
		}
		else 
		{
			for (int i = 0; i < handCards.Count; i++) 
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine($"[blue]{handCards[i].GetCurrentEnergyCost(player.Id)}[/] [darkorange]{handCards[i].GetCurrentPower(player.Id)}[/]");
				sb.AppendLine($"[[{Cursor(_characterSelector, i)}]]");
				sb.Append(handCards[i].Name);
				characters.AddColumn(new TableColumn(sb.ToString()).Centered());
			}
			
			if (handCards.Count == 0) characters.AddColumn(new TableColumn("(Empty)").Centered());
			characters.Title(player.Name + "'s hand cards");
		}
		
		Table table = new Table()
			.Border(TableBorder.Square)
			.BorderColor(Color.White)
			.AddColumn(new TableColumn(characters));
		return table.Expand();
	}
	
	static Table CardInfo() 
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("Card Info");
		sb.Append(_cardInfo);
		Table table = new Table()
			.Border(TableBorder.Square)
			.BorderColor(Color.White)
			.AddColumn(new TableColumn(sb.ToString()).Centered());
		return table.Expand();
	}
	
	static Table MarvelSnapLayout(MarvelSnapGame game, Player player) 
	{
		Console.Clear();
		List<Player> players = game.GetPlayers();
		
		List<CharacterCard> arenaCards1Player1 = game.GetArenaCards(players[0], ArenaType.Arena1);
		List<CharacterCard> arenaCards2Player1 = game.GetArenaCards(players[0], ArenaType.Arena2);
		List<CharacterCard> arenaCards3Player1 = game.GetArenaCards(players[0], ArenaType.Arena3);
		List<CharacterCard> arenaCards1Player2 = game.GetArenaCards(players[1], ArenaType.Arena1);
		List<CharacterCard> arenaCards2Player2 = game.GetArenaCards(players[1], ArenaType.Arena2);
		List<CharacterCard> arenaCards3Player2 = game.GetArenaCards(players[1], ArenaType.Arena3);
		
		List<StringBuilder> sb1 = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		List<StringBuilder> sb2 = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		List<StringBuilder> sb3 = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		List<StringBuilder> sb4 = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		List<StringBuilder> sb5 = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		List<StringBuilder> sb6 = new() { new("(Empty)"), new("(Empty)"), new("(Empty)"), new("(Empty)") };
		
		for (int i = 0; i < arenaCards1Player1.Count; i++)
		{
			sb1[i].Clear();
			sb1[i].AppendLine($"[blue]{arenaCards1Player1[i].GetCurrentEnergyCost(players[0].Id)}[/] [darkorange]{arenaCards1Player1[i].GetCurrentPower(players[0].Id)}[/]");
			sb1[i].AppendLine($"{arenaCards1Player1[i].Name}");
		}
		
		for (int i = 0; i < arenaCards2Player1.Count; i++)
		{
			sb2[i].Clear();
			sb2[i].AppendLine($"[blue]{arenaCards2Player1[i].GetCurrentEnergyCost(players[0].Id)}[/] [darkorange]{arenaCards2Player1[i].GetCurrentPower(players[0].Id)}[/]");
			sb2[i].AppendLine($"{arenaCards2Player1[i].Name}");
		}
		
		for (int i = 0; i < arenaCards3Player1.Count; i++)
		{
			sb3[i].Clear();
			sb3[i].AppendLine($"[blue]{arenaCards3Player1[i].GetCurrentEnergyCost(players[0].Id)}[/] [darkorange]{arenaCards3Player1[i].GetCurrentPower(players[0].Id)}[/]");
			sb3[i].AppendLine($"{arenaCards3Player1[i].Name}");
		}
		
		for (int i = 0; i < arenaCards1Player2.Count; i++)
		{
			sb4[i].Clear();
			sb4[i].AppendLine($"[blue]{arenaCards1Player2[i].GetCurrentEnergyCost(players[1].Id)}[/] [darkorange]{arenaCards1Player2[i].GetCurrentPower(players[1].Id)}[/]");
			sb4[i].AppendLine($"{arenaCards1Player2[i].Name}");
		}
		
		for (int i = 0; i < arenaCards2Player2.Count; i++)
		{
			sb5[i].Clear();
			sb5[i].AppendLine($"[blue]{arenaCards2Player2[i].GetCurrentEnergyCost(players[1].Id)}[/] [darkorange]{arenaCards2Player2[i].GetCurrentPower(players[1].Id)}[/]");
			sb5[i].AppendLine($"{arenaCards2Player2[i].Name}");
		}
		
		for (int i = 0; i < arenaCards3Player2.Count; i++)
		{
			sb6[i].Clear();
			sb6[i].AppendLine($"[blue]{arenaCards3Player2[i].GetCurrentEnergyCost(players[1].Id)}[/] [darkorange]{arenaCards3Player2[i].GetCurrentPower(players[1].Id)}[/]");
			sb6[i].AppendLine($"{arenaCards3Player2[i].Name}");
		}
		
		var arenaCards1Player1Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(sb1[0].ToString()).Footer(sb1[1].ToString()).Centered())
			.AddColumn(new TableColumn(sb1[2].ToString()).Footer(sb1[3].ToString()).Centered());
		
		var arenaCards2Player1Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(sb2[0].ToString()).Footer(sb2[1].ToString()).Centered())
			.AddColumn(new TableColumn(sb2[2].ToString()).Footer(sb2[3].ToString()).Centered());
			
		var arenaCards3Player1Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(sb3[0].ToString()).Footer(sb3[1].ToString()).Centered())
			.AddColumn(new TableColumn(sb3[2].ToString()).Footer(sb3[3].ToString()).Centered());
			
		var arenaCards1Player2Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(sb4[0].ToString()).Footer(sb4[1].ToString()).Centered())
			.AddColumn(new TableColumn(sb4[2].ToString()).Footer(sb4[3].ToString()).Centered());
		
		var arenaCards2Player2Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(sb5[0].ToString()).Footer(sb5[1].ToString()).Centered())
			.AddColumn(new TableColumn(sb5[2].ToString()).Footer(sb5[3].ToString()).Centered());
			
		var arenaCards3Player2Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(sb6[0].ToString()).Footer(sb6[1].ToString()).Centered())
			.AddColumn(new TableColumn(sb6[2].ToString()).Footer(sb6[3].ToString()).Centered());
		
		List<LocationCard> locations = game.GetLocations();
		
		StringBuilder sbLocation1 = new();
		sbLocation1.AppendLine();
		sbLocation1.AppendLine(locations[0].IsRevealed ? locations[0].Name : "(Unrevealed)");
		sbLocation1.Append($"[[{Cursor(_locationSelector, 0)}]]");
		
		StringBuilder sbLocation2 = new();
		sbLocation2.AppendLine();
		sbLocation2.AppendLine(locations[1].IsRevealed ? locations[1].Name : "(Unrevealed)");
		sbLocation2.Append($"[[{Cursor(_locationSelector, 1)}]]");
		
		StringBuilder sbLocation3 = new();
		sbLocation3.AppendLine();
		sbLocation3.AppendLine(locations[2].IsRevealed ? locations[2].Name : "(Unrevealed)");
		sbLocation3.Append($"[[{Cursor(_locationSelector, 2)}]]");
		
		var location1 = new Panel(sbLocation1.ToString());
		var location2 = new Panel(sbLocation2.ToString());
		var location3 = new Panel(sbLocation3.ToString());
			
		var table = new Table()
			.Centered()
			.Border(TableBorder.None)
			.BorderColor(Color.Black)
			.Title("[yellow]Player 2[/]")
			.Caption("[yellow]Player 1[/]")
			.AddColumn(new TableColumn(arenaCards1Player2Table).Footer(arenaCards1Player1Table).Centered()) // Arena 1
			.AddColumn(new TableColumn(arenaCards2Player2Table).Footer(arenaCards2Player1Table).Centered()) // Arena 2
			.AddColumn(new TableColumn(arenaCards3Player2Table).Footer(arenaCards3Player1Table).Centered()) // Arena 3
			.AddRow(Align.Center(location1), Align.Center(location2), Align.Center(location3));
		return table.Expand();
	}
	
	static void GameEndedSpectre(Player? player) 
	{
		if (player != null) 
		{
			AnsiConsole.Write(
			new FigletText("Winner: " + player.Name)
				.LeftJustified()
				.Color(Color.Red).Centered());
		}
		else 
		{
			AnsiConsole.Write(
			new FigletText("Game Draw!")
				.LeftJustified()
				.Color(Color.Red).Centered());
		}
		Thread.Sleep(5000);
	}
}