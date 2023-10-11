using MarvelSnap;

namespace Program;

public partial class Program
{
	static void DisplayConsole(MarvelSnapGame game, Player player1, Player player2) 
	{
		InputPlayerName(game, player1, player2);
		game.Start();
		game.SetGameStatus(GameStatus.Ongoing);
		
		while (game.GetGameStatus() != GameStatus.GameEnded) 
		{
			if (game.GetGameStatus() == GameStatus.Ongoing) 
			{
				Player player = game.GetPlayerTurn();
				Console.WriteLine("Player: " + player.Id + " Turn: " + game.Turn);
				ChooseAction(out int action);
				switch (action) 
				{
					case 1:
						DisplayPutCard(game, player);
						break;
					case 2:
						DisplayTakeCard(); // todo
						break;
					case 3:
						game.EndTurn(player);
						bool status = game.TryGetNextPlayer(out Player? nextPlayer);
						if (status) game.SetPlayerTurn(nextPlayer);
						bool canNextTurn = game.NextTurn();
						// if (canNextTurn) game.SetGameStatus(GameStatus.NewTurn);
						break;
				}
			}
			
			if (game.Turn > game.MaxTurn) game.SetGameStatus(GameStatus.GameEnded);
		}
		game.SetGameStatus(GameStatus.NotStarted);
	}
	
	static void InputPlayerName(MarvelSnapGame game, Player player1, Player player2) 
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
	
	static void DisplayPutCard(MarvelSnapGame game, Player player) 
	{
		List<LocationCard> locations = game.GetLocations();
		
		Console.WriteLine("Following are current cards in each location.");
		
		foreach (var location in locations) 
		{
			Console.WriteLine(location.Name);
			List<CharacterCard> arenaCards = game.GetArenaCards(player, location);
			if (arenaCards.Count == 0) Console.WriteLine("(Empty)");
			else 
			{
				foreach (var card in arenaCards) 
				{
					Console.WriteLine(card.Name);
				}
			}
		}

		Console.WriteLine();
		Console.WriteLine("Following are your current cards in your hand.");
		List<CharacterCard?> handCards = game.GetHandCards(player);
		foreach (var card in handCards) 
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
				bool canPut = game.PutCardInArena(player, (ArenaType) (input2 - 1), handCards[input - 1]);
				if (!canPut) 
				{
					Console.WriteLine("Arena is full!");
					Thread.Sleep(1000);
				} 
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
