using MarvelSnap;
namespace Program;

public partial class Program 
{
	static void Main() 
	{
		bool isSpectre = true;
		
		Player player1 = new(1);
		Player player2 = new(2);
		MarvelSnapGame game = new(player1, player2);

		if (!isSpectre) 
		{
			game.OnCardRevealed += CardRevealed;
			game.OnCardPowerChanged += CardPowerChanged;
			game.OnArenaPowerChanged += ArenaPowerChanged;
			DisplayConsole(game, player1, player2);
		}
		else 
		{
			DisplaySpectre(game, player1, player2);
		}
	}
	
	static void CardRevealed(Player? player, Card card) 
	{
		if (player != null) 
		{
			Console.WriteLine(player.Name + " revealed " + card.Name + ".");
			Thread.Sleep(1000);	
		}
		else 
		{
			Console.WriteLine(card.Name + " revealed.");
			Thread.Sleep(1000);
		}
	}
	
	static void CardPowerChanged(Player player, CharacterCard card) 
	{
		List<Buff> buffs = card.GetBuffs(player.Id);
		buffs.Sort();
		string buffString = "";
		foreach (var buff in buffs) 
		{
			buffString += buff.GetSymbol() + buff.Value.ToString() + " ";
		}
		Console.WriteLine(card.Name + " power has changed: " + buffString);
		Thread.Sleep(1000);
	}
	
	static void ArenaPowerChanged(Player player, Arena arena) 
	{
		List<Buff> powerBuffs = arena.GetPowerBuffs(player.Id);
		string buffString = "";
		foreach (var buff in powerBuffs) 
		{
			buffString += buff.GetSymbol() + buff.Value.ToString() + " ";
		}
		Console.WriteLine($"({player.Name}) {arena.Id} power has changed: {buffString}");
		Thread.Sleep(1000);
	}
}