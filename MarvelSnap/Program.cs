using MarvelSnap;
using Spectre.Console;

namespace Program;

public partial class Program
{
	static void Main()
	{
		Console.Clear();
		bool isSpectre = false;

		string[] choices = new[] { "Built-in console", "Spectre console" };

		var choice = AnsiConsole.Prompt(
			new SelectionPrompt<string>()
				.Title("Select [green]console[/]")
				.AddChoices(choices));

		if (choice == choices[0]) isSpectre = false;
		else isSpectre = true;

		IPlayer player1 = new(1);
		IPlayer player2 = new(2);
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
			game.OnCardPowerChanged += CardPowerChangedSpectre;
			game.OnGameEnded += GameEndedSpectre;

			DisplaySpectre(game, player1, player2);
		}
	}
}