using MarvelSnap;
namespace Program;

public partial class Program 
{
	private int _consoleSelector = -1;
	
	static void Main() 
	{
		Console.Clear();
		bool isSpectre = false;
		
		Console.WriteLine("1. Built-in console");
		Console.WriteLine("2. Spectre console");
		Console.Write("Select console:");
		bool consoleStatus = int.TryParse(Console.ReadLine(), out int input);
		
		if (consoleStatus && input >= 1 && input <= 2) 
		{
			if (input == 1) isSpectre = false;
			else isSpectre = true;
		}
		
		Player player1 = new(1);
		Player player2 = new(2);
		MarvelSnapGame game = new(player1, player2);

		if (!isSpectre) 
		{
			game.OnCardRevealed += CardRevealed;
			game.OnCardPowerChanged += CardPowerChanged;
			game.OnArenaPowerChanged += ArenaPowerChanged;
			game.OnEnergyCostChanged += EnergyCostChanged;
			game.OnCardDestroyed += CardDestroyed;
			game.OnGameEnded += GameEnded;
			
			DisplayConsole(game, player1, player2);
		}
		else 
		{
			game.OnGameEnded += GameEndedSpectre;
			
			DisplaySpectre(game, player1, player2);
		}
	}
}