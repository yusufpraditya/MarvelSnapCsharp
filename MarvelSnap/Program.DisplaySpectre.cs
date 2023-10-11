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
	private static ConsoleKey key;
	
	static void DisplaySpectre(MarvelSnapGame game, Player player1, Player player2) 
	{
		Console.CursorVisible = false;
		game.SetPlayerName(player1, "Yusuf");
		game.SetPlayerName(player2, "Praditya");
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
			
			task.Wait();
		}
	}
	
	static async Task GetInputKey(MarvelSnapGame game, Player player) 
	{
		await Task.Run(() => 
		{
			GameStatus status = game.GetGameStatus();
			List<CharacterCard> handCards = game.GetHandCards(player);
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
						_cardInfo = handCards[_characterSelector].Name + ": " + handCards[_characterSelector].Description;
					}
					else if (status == GameStatus.SelectLocation) 
					{
						_locationSelector = _locationSelector <= 0 ? 0 : _locationSelector - 1;
						_cardInfo = locationCards[_locationSelector].Name + ": " + locationCards[_locationSelector].Description;
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
						_characterSelector = _characterSelector >= handCards.Count - 1 ? handCards.Count - 1 : _characterSelector + 1;
						_cardInfo = handCards[_characterSelector].Name + ": " + handCards[_characterSelector].Description;
					}
					else if (status == GameStatus.SelectLocation) 
					{
						_locationSelector = _locationSelector >= 2 ? 2 : _locationSelector + 1;
						_cardInfo = locationCards[_locationSelector].Name + ": " + locationCards[_locationSelector].Description;
					}
					break;
				case ConsoleKey.Spacebar or ConsoleKey.Enter:
					if (status == GameStatus.SelectAction) 
					{
						if (_actionSelector == 0 || _actionSelector == 1) 
						{
							game.SetGameStatus(GameStatus.SelectCharacter);
						}
						else 
						{
							// End Turn
							game.EndTurn(player);
							bool nextPlayerStatus = game.TryGetNextPlayer(out Player? nextPlayer);
							if (nextPlayerStatus) game.SetPlayerTurn(nextPlayer);
							game.NextTurn();
						}
						
						if (handCards.Count > 0 || _actionSelector == 2) 
						{
							_actionSelector = -1;
							_characterSelector = 0;
							_locationSelector = -1;
							_cardInfo = handCards[_characterSelector].Name + ": " + handCards[_characterSelector].Description;
						}
						else 
						{
							game.SetGameStatus(GameStatus.SelectAction);
						}
					} 
					else if (status == GameStatus.SelectCharacter) 
					{
						game.SetGameStatus(GameStatus.SelectLocation);
						_selectedCard = _characterSelector;
						_actionSelector = -1;
						_characterSelector = -1;
						_locationSelector = 0;
						_cardInfo = locationCards[_locationSelector].Name + ": " + locationCards[_locationSelector].Description;
					}
					else if (status == GameStatus.SelectLocation) 
					{
						game.PutCardInArena(player, (ArenaType) _locationSelector, handCards[_selectedCard]);
						game.SetGameStatus(GameStatus.SelectAction);
						_locationSelector = -1;
						_actionSelector = 0;
						_characterSelector = -1;
						_cardInfo = "";
					} 
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
			.Title(player.Name + "'s hand")
			.Caption("Turn: " + game.Turn.ToString() + "/" + game.MaxTurn.ToString())
			.Centered();
		List<CharacterCard> handCards = game.GetHandCards(player);
		
		for (int i = 0; i < handCards.Count; i++) 
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"[[{Cursor(_characterSelector, i)}]]");
			sb.Append(handCards[i].Name);
			characters.AddColumn(new TableColumn(sb.ToString()).Centered());
		}
		
		if (handCards.Count == 0) characters.AddColumn(new TableColumn("(Empty)").Centered());
		
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
		// Dictionary<Player, List<CharacterCard>> arenaCards = game.GetArenaCardsForEachPlayer();
		// Dictionary<ArenaType, Dictionary<Player, List<CharacterCard>>> dict = new();
		
		// foreach (var card in arenaCards[player]) 
		// {
		// 	if (card.Location == ArenaType.Arena1) 
		// 	{
		// 		dict.TryAdd(card.Location, arenaCards);
		// 	}
		// }
		
		string[] arenaCardsArray1Player1 = new string[] { "(Empty)", "(Empty)", "(Empty)", "(Empty)" };
		string[] arenaCardsArray2Player1 = new string[] { "(Empty)", "(Empty)", "(Empty)", "(Empty)" };
		string[] arenaCardsArray3Player1 = new string[] { "(Empty)", "(Empty)", "(Empty)", "(Empty)" };
		string[] arenaCardsArray1Player2 = new string[] { "(Empty)", "(Empty)", "(Empty)", "(Empty)" };
		string[] arenaCardsArray2Player2 = new string[] { "(Empty)", "(Empty)", "(Empty)", "(Empty)" };
		string[] arenaCardsArray3Player2 = new string[] { "(Empty)", "(Empty)", "(Empty)", "(Empty)" };
		
		for (int i = 0; i < arenaCards1Player1.Count; i++) 
		{
			arenaCardsArray1Player1[i] = arenaCards1Player1[i].Name;
		}
		for (int i = 0; i < arenaCards2Player1.Count; i++) 
		{
			arenaCardsArray2Player1[i] = arenaCards2Player1[i].Name;
		}
		for (int i = 0; i < arenaCards3Player1.Count; i++) 
		{
			arenaCardsArray3Player1[i] = arenaCards3Player1[i].Name;
		}
		
		for (int i = 0; i < arenaCards1Player2.Count; i++) 
		{
			arenaCardsArray1Player2[i] = arenaCards1Player2[i].Name;
		}
		for (int i = 0; i < arenaCards2Player2.Count; i++) 
		{
			arenaCardsArray2Player2[i] = arenaCards2Player2[i].Name;
		}
		for (int i = 0; i < arenaCards3Player2.Count; i++) 
		{
			arenaCardsArray3Player2[i] = arenaCards3Player2[i].Name;
		}
		
		var arenaCards1Player1Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(arenaCardsArray1Player1[0]).Footer(arenaCardsArray1Player1[1]).Centered())
			.AddColumn(new TableColumn(arenaCardsArray1Player1[2]).Footer(arenaCardsArray1Player1[3]).Centered());
		
		var arenaCards2Player1Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(arenaCardsArray2Player1[0]).Footer(arenaCardsArray2Player1[1]).Centered())
			.AddColumn(new TableColumn(arenaCardsArray2Player1[2]).Footer(arenaCardsArray2Player1[3]).Centered());
			
		var arenaCards3Player1Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(arenaCardsArray3Player1[0]).Footer(arenaCardsArray3Player1[1]).Centered())
			.AddColumn(new TableColumn(arenaCardsArray3Player1[2]).Footer(arenaCardsArray3Player1[3]).Centered());
			
		var arenaCards1Player2Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(arenaCardsArray1Player2[0]).Footer(arenaCardsArray1Player2[1]).Centered())
			.AddColumn(new TableColumn(arenaCardsArray1Player2[2]).Footer(arenaCardsArray1Player2[3]).Centered());
		
		var arenaCards2Player2Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(arenaCardsArray2Player2[0]).Footer(arenaCardsArray2Player2[1]).Centered())
			.AddColumn(new TableColumn(arenaCardsArray2Player2[2]).Footer(arenaCardsArray2Player2[3]).Centered());
			
		var arenaCards3Player2Table = new Table()
			.Centered()
			.Border(TableBorder.Square)
			.BorderColor(Color.Cyan1)
			.AddColumn(new TableColumn(arenaCardsArray3Player2[0]).Footer(arenaCardsArray3Player2[1]).Centered())
			.AddColumn(new TableColumn(arenaCardsArray3Player2[2]).Footer(arenaCardsArray3Player2[3]).Centered());
		
		StringBuilder sbLocation1 = new();
		sbLocation1.AppendLine();
		sbLocation1.AppendLine("Onslaught's Citadel");
		sbLocation1.Append($"[[{Cursor(_locationSelector, 0)}]]");
		
		StringBuilder sbLocation2 = new();
		sbLocation2.AppendLine();
		sbLocation2.AppendLine("Dream Dimension");
		sbLocation2.Append($"[[{Cursor(_locationSelector, 1)}]]");
		
		StringBuilder sbLocation3 = new();
		sbLocation3.AppendLine();
		sbLocation3.AppendLine("Kyln");
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
}