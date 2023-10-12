﻿using MarvelSnap;

namespace Program;

public partial class Program
{
	static void DisplayConsole(MarvelSnapGame game, Player player1, Player player2) 
	{
		Console.CursorVisible = false;
		InputPlayerName(player1, player2);
		game.Start();
		game.SetGameStatus(GameStatus.Ongoing);
		
		while (game.GetGameStatus() != GameStatus.GameEnded) 
		{
			if (game.GetGameStatus() == GameStatus.Ongoing) 
			{
				Player player = game.GetPlayerTurn();
				int playerEnergy = game.GetCurrentEnergy(player);
				Console.WriteLine();
				Console.WriteLine($"Player: {player.Name} Energy: {playerEnergy} Turn: {game.Turn}/{game.MaxTurn}");
				ChooseAction(out int action);
				switch (action) 
				{
					case 1:
						DisplayPutCard(game, player);
						break;
					case 2:
						DisplayTakeCard(game, player); // todo
						break;
					case 3:
						game.EndTurn(player);
						bool status = game.TryGetNextPlayer(out Player? nextPlayer);
						if (status) game.SetPlayerTurn(nextPlayer);
						game.NextTurn();
						break;
				}
			}
			
			if (game.Turn > game.MaxTurn) game.SetGameStatus(GameStatus.GameEnded);
		}
		game.SetGameStatus(GameStatus.NotStarted);
	}
	
	static void InputPlayerName(Player player1, Player player2) 
	{
		Console.Clear();
		Console.WriteLine("Welcome to Marvel Snap game! Please input your name first.");
		SetPlayerName("Input name for player 1: ", player1);
		SetPlayerName("Input name for player 2: ", player2);
		Console.Clear();
	}
	
	static void SetPlayerName(string text, Player player) 
	{
		Console.Write(text);
		string? inputName = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(inputName)) 
		{
			Console.WriteLine("Please input valid name.");
			Thread.Sleep(1000);
			SetPlayerName(text, player);
		}
		else 
		{
			player.Name = inputName;
		}
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
		DisplayArenaCards(game);
		
		List<CharacterCard?> handCards = game.GetHandCards(player);
		if (handCards.Count > 0) 
		{
			int input;
			while (true) 
			{
				Console.WriteLine($"Following are your current cards in your hand. (Your energy: {game.GetCurrentEnergy(player)})");
				int number = 1;
				foreach (var card in handCards) 
				{
					Console.WriteLine($"{number}. {card?.Name} (Cost: {card?.GetCurrentEnergyCost(player.Id)} - Power: {card?.GetCurrentPower(player.Id)})");
					number++;
				}
				Console.WriteLine();
				Console.WriteLine("0. Back to action menu");
				Console.WriteLine();
				Console.Write("Choose card you want to put in: ");
				
				bool status = int.TryParse(Console.ReadLine(), out input);
				if (status && input >= 0 && input <= handCards.Count) 
				{
					if (input == 0) break;
					if (game.GetCurrentEnergy(player) < handCards[input - 1].GetCurrentEnergyCost(player.Id)) 
					{
						Console.Clear();
						Console.WriteLine("Insufficient energy!");
						Thread.Sleep(1000);
						Console.Clear();
					}
					else break;
				}
				else 
				{
					Console.Clear();
					Console.WriteLine("Please input valid card!");
					Thread.Sleep(1000);
					Console.Clear();
				}
			}
			
			List<LocationCard> locations = game.GetLocations();
			List<Arena> arenas = game.GetListOfArenas();
			while (true && input != 0)
			{
				Console.WriteLine();
				Console.WriteLine($"1. {locations[0].Name} {(arenas[0].IsAvailable() ? "" : "(disabled)")}");
				Console.WriteLine($"   {locations[0].Description}");
				Console.WriteLine($"2. {(locations[1].IsRevealed ? locations[1].Name : "(Unrevealed)")} {(arenas[1].IsAvailable() ? "" : "(disabled)")}");
				Console.WriteLine($"   {(locations[1].IsRevealed ? locations[1].Description : "")}");
				Console.WriteLine($"3. {(locations[2].IsRevealed ? locations[2].Name : "(Unrevealed)")} {(arenas[2].IsAvailable() ? "" : "(disabled)")}");
				Console.WriteLine($"   {(locations[2].IsRevealed ? locations[2].Description : "")}");
				Console.WriteLine();
				Console.Write("Choose location you want to put in: ");
				bool status2 = int.TryParse(Console.ReadLine(), out int input2);
				if (status2 && input2 >= 1 && input2 <= 3) 
				{
					if (arenas[input2 - 1].IsAvailable()) 
					{
						bool canPut = game.PutCardInArena(player, (ArenaType) (input2 - 1), handCards[input - 1]);
						if (!canPut) 
						{
							Console.WriteLine("Arena is full!");
							Thread.Sleep(1000);
						} 
						break;
					}
					else 
					{
						Console.Clear();
						Console.WriteLine("You can't play card(s) here!");
						Thread.Sleep(1000);
						Console.Clear();
					}
				}
				else 
				{
					Console.WriteLine("Please input valid location!");
					Thread.Sleep(1000);
					Console.Clear();
				}
			}
			Console.Clear();
		}
		else 
		{
			Console.Clear();
			Console.WriteLine("Your have no card(s) in your hand!");
			Thread.Sleep(1000);
			Console.Clear();
		}
	}
	
	static void DisplayTakeCard(MarvelSnapGame game, Player player) 
	{
		DisplayArenaCards(game);
		
		Dictionary<Player, List<CharacterCard>> arenaCards = game.GetArenaCardsForEachPlayer();
		
		if (arenaCards[player].Count > 0) 
		{
			while (true)
			{
				int number = 1;
				foreach (var card in arenaCards[player])
				{
					Console.WriteLine($"{number}. ({card.Location}) {card.Name}");
					number++;
				}

				Console.WriteLine();
				Console.Write("Choose card you want to take: ");

				bool status = int.TryParse(Console.ReadLine(), out int input);

				if (status && input >= 1 && input <= arenaCards[player].Count)
				{
					game.TakeCardFromArena(player, arenaCards[player][input - 1].Location, arenaCards[player][input - 1]);
					Console.Clear();
					break;
				}
				else
				{
					Console.WriteLine("Please input valid card!");
					Thread.Sleep(1000);
					Console.Clear();
				}
			}

		}
		else 
		{
			Console.Clear();
			Console.WriteLine("Your have no card(s) in arena!");
			Thread.Sleep(1000);
			Console.Clear();
		}
	}
	
	static void DisplayArenaCards(MarvelSnapGame game) 
	{
		List<LocationCard> locations = game.GetLocations();
		List<Player> players = game.GetPlayers();
		
		Console.WriteLine("Following are current cards in each location.");
		
		foreach (var location in locations) 
		{
			Console.WriteLine(location.IsRevealed ? location.Name : "(Unrevealed)");
			foreach (var player in players) 
			{
				List<CharacterCard> arenaCards = game.GetArenaCards(player, location);
				if (arenaCards.Count == 0) Console.WriteLine($"({player.Name}) (Empty)");
				else 
				{
					foreach (var card in arenaCards) 
					{
						Console.WriteLine($"({player.Name}) {card.Name}");
					}
				}
			}
			Console.WriteLine();
		}

		Console.WriteLine();
	}
}