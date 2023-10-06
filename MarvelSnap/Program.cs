using MarvelSnap;

public class Program 
{
	static void Main() 
	{
		// test, will refactor later
		
		Player player1 = new(1);
		Player player2 = new(2);
		MarvelSnapGame game = new(player1, player2);
		Dictionary<IPlayer, List<CharacterCard?>> playerCardsInHand = new() 
		{
			{ player1, new() },
			{ player2, new() }
		};
		
		InputPlayerName(game, player1, player2);
		game.SetGameStatus(GameStatus.NewTurn);
		
		while (game.GetGameStatus() != GameStatus.GameEnded) 
		{
			if (game.GetGameStatus() == GameStatus.NewTurn) 
			{
				playerCardsInHand[player1].Add(game.DrawCard(player1));
				playerCardsInHand[player2].Add(game.DrawCard(player2));
				playerCardsInHand[player1].Add(game.DrawCard(player1));
				playerCardsInHand[player2].Add(game.DrawCard(player2));
				playerCardsInHand[player1].Add(game.DrawCard(player1));
				playerCardsInHand[player2].Add(game.DrawCard(player2));
				game.SetGameStatus(GameStatus.PlayersTurn);
			}
			
			if (game.GetGameStatus() == GameStatus.PlayersTurn) 
			{
				Console.WriteLine("Turn: " + game.Turn);
				IPlayer player = game.GetPlayerTurn();
				ChooseAction(out int action);
				switch (action) 
				{
					case 1:
						DisplayPutCard(game, player, playerCardsInHand);
						break;
					case 2:
						DisplayTakeCard(); // todo
						break;
					case 3:
						game.EndTurn(player);
						bool status = game.TryGetNextPlayer(out IPlayer? nextPlayer);
						if (status) game.SetPlayerTurn(nextPlayer);
						if (game.PlayersHavePlayed()) 
						{
							game.SetGameStatus(GameStatus.NewTurn);
							game.NextTurn();
						}
						break;
				}
			}
			
			if (game.Turn > game.MaxTurn) game.SetGameStatus(GameStatus.GameEnded);
		}
		game.SetGameStatus(GameStatus.NotStarted);
	}
	
	static void InputPlayerName(MarvelSnapGame game, IPlayer player1, IPlayer player2) 
	{
		Console.Clear();
		Console.WriteLine("Welcome to Marvel Snap game! Please input your name first.");
		Console.Write("Input name for player 1: ");
		string? name = Console.ReadLine();
		game.SetPlayerName(player1, name);
		Console.Write("Input name for player 2: ");
		name = Console.ReadLine();
		game.SetPlayerName(player2, name);
		Console.Clear();
	}
	
	static void ChooseAction(out int action) 
	{
		action = 0;
		Console.WriteLine("Please choose one of the following actions");
		Console.WriteLine("1. Put card(s) to a location");
		Console.WriteLine("2. Take card(s) from a location");
		Console.WriteLine("3. End turn");
		Console.WriteLine();
		Console.Write("Your input: ");
		bool status = int.TryParse(Console.ReadLine(), out int input);
		if (status && input >= 1 && input <= 3) 
		{
			action = input;
		}
		else 
		{
			Console.WriteLine("Please input valid number (1-3).");
			Thread.Sleep(1000);
		}
		Console.Clear();
	}
	
	static void DisplayPutCard(MarvelSnapGame game, IPlayer player, Dictionary<IPlayer, List<CharacterCard?>> playerCardsInHand) 
	{
		List<LocationCard> locations = game.GetLocations();
		Dictionary<IPlayer, List<CharacterCard>> playerCardsInArena = game.GetArenaCardsForEachPlayer();
		
		Console.WriteLine("Following are current cards in each location.");
		foreach (var location in locations) 
		{
			Console.WriteLine(location.Name);
			foreach (var kvp in playerCardsInArena) 
			{
				if (playerCardsInArena[kvp.Key].Count == 0) Console.WriteLine("(Empty)");
					else 
					{
						foreach (var card in playerCardsInArena[kvp.Key]) 
						{
							Console.WriteLine(card.Name);
						}
				}
			}
		}

		Console.WriteLine();
		Console.WriteLine("Following are your current cards in your hand.");
		foreach (var card in playerCardsInHand[player]) 
		{
			Console.WriteLine(card?.Name);
		}
		
		Console.WriteLine();
		Console.Write("Choose card you want to put in: ");
		bool status = int.TryParse(Console.ReadLine(), out int input);
		if (status && input >= 1 && input <= 3) 
		{
			Console.WriteLine("1. " + locations[0].Name);
			Console.WriteLine("2. " + locations[1].Name);
			Console.WriteLine("3. " + locations[2].Name);
			Console.Write("Choose location you want to put in: ");
			bool status2 = int.TryParse(Console.ReadLine(), out int input2);
			if (status2 && input2 >= 1 && input2 <= 3) 
			{
				game.PutCardInArena(player, (ArenaType) locations[input2 - 1].Id, playerCardsInHand[player][input - 1]);
			}
		}
		else 
		{
			Console.WriteLine("Please input valid number (1-3).");
		}
		Console.Clear();
	}
	
	static void DisplayTakeCard() 
	{
		
	}
}