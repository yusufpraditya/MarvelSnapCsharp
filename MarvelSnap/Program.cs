using MarvelSnap;

public class Program 
{
	static void Main() 
	{
		// test
		Player player1 = new(1);
		Player player2 = new(2);
		MarvelSnapGame game = new(player1, player2);
		CharacterCard? card;
		
		card = game.DrawCard(player1);
		Console.WriteLine("Card id (shuffle): " + card?.Id);
		
		Console.WriteLine("Base power: " + card?.GetCurrentPower());
		
		card?.AddBuff(new Buff(4, BuffType.Power, BuffOperation.Add));
		Console.WriteLine("Base + buff: " + card?.GetCurrentPower());
		
		// AntMan? antman = (AntMan?) card;
		
		// game.PutCardInArena(player1, ArenaId.Arena1, antman);
		// game.PutCardInArena(player1, ArenaId.Arena1, antman);
		
		// antman?.Ongoing(player1, game);
		
		// Hawkeye? hawkeye = (Hawkeye?) card;
		// game.OnCardRevealed(player1, hawkeye);
		
		while (game.GetGameStatus() != GameStatus.GameEnded) 
		{
			InputPlayerName(game, player1, player2);
		}
	}
	
	static void InputPlayerName(MarvelSnapGame game, IPlayer player1, IPlayer player2) 
	{
		Console.Write("Input name for player 1: ");
		string? name = Console.ReadLine();
		game.SetPlayerName(player1, name);
		Console.Write("Input name for player 2: ");
		name = Console.ReadLine();
		game.SetPlayerName(player2, name);
	}
}