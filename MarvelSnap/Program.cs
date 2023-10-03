using MarvelSnap;

public class Program 
{
	static void Main() 
	{
		// test
		Player player1 = new(1, "Yusuf");
		Player player2 = new(2, "Praditya");
		MarvelSnapGame game = new(player1, player2);
		CharacterCard? card;
		
		card = game.DrawCard(player1);
		Console.WriteLine("Card id (shuffle): " + card.Id);
		
		Console.WriteLine("Base power: " + card?.GetCurrentPower());
		
		card?.AddBuff(new Buff(4, BuffType.Power, BuffOperation.Add));
		Console.WriteLine("Base + buff: " + card?.GetCurrentPower());
		
		AntMan? antman = (AntMan?) card;
		
		game.PutCardInArena(player1, ArenaId.Arena1, antman);
		game.PutCardInArena(player1, ArenaId.Arena1, antman);
		
		antman?.Ongoing(player1, game);
		
	}
}