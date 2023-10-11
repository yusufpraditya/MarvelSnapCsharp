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
			game.OnEnergyCostChanged += EnergyCostChanged;
			game.OnGameEnded += GameEnded;
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
		int currentPower = card.GetCurrentPower(player.Id);
		List<Buff> powerBuffs = card.GetBuffs(player.Id);
		
		string baseWithBuff = card.BasePower.ToString();
		foreach (var buff in powerBuffs) 
		{
			if (buff.Type == BuffType.Power) 
			{
				baseWithBuff += $" {buff.GetSymbol()}{buff.Value}";
			}
		}
		
		Console.WriteLine($"{card.Name} power has changed: {currentPower} ({baseWithBuff})");
		Thread.Sleep(1000);
	}
	
	static void ArenaPowerChanged(Player player, Arena arena) 
	{
		List<Buff> powerBuffs = arena.GetPowerBuffs(player.Id);
		string buffString = "";
		foreach (var buff in powerBuffs) 
		{
			if (buff.Type == BuffType.Power)
				buffString += buff.GetSymbol() + buff.Value.ToString() + " ";
		}
		Console.WriteLine($"({player.Name}) {arena.Id} power has changed: {buffString}");
		Thread.Sleep(1000);
	}
	
	static void EnergyCostChanged(Player player, CharacterCard card) 
	{
		List<Buff> buffs = card.GetBuffs(player.Id);
		string buffString = "";
		foreach (var buff in buffs) 
		{
			if (buff.Type == BuffType.Energy)
				buffString += buff.GetSymbol() + buff.Value.ToString() + " ";
		}
		Console.WriteLine(card.Name + " energy cost has changed: " + buffString);
		Thread.Sleep(1000);
	}
	
	static void GameEnded(Player? player) 
	{
		if (player != null) 
		{
			Console.WriteLine($"{player.Name} has won the match.");
			Thread.Sleep(1000);
		}
		else 
		{
			Console.WriteLine("Game Draw!");
			Thread.Sleep(1000);
		}
	}
}