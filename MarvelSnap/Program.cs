using MarvelSnap;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Spectre.Console;

namespace Program;

public partial class Program
{
	private static GameStatus _gameStatus = GameStatus.NotStarted;
	
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

		IPlayer player1 = new Player(1);
		IPlayer player2 = new Player(2);
		
		var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.ClearProviders();
			builder.AddNLog("nlog.config");
		});
		ILogger<MarvelSnapGame> logger = loggerFactory.CreateLogger<MarvelSnapGame>();
		MarvelSnapGame game = new(player1, player2, logger);

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
	
	static void SetGameStatus(GameStatus gameStatus)
	{
		_gameStatus = gameStatus;
	}

	static GameStatus GetGameStatus()
	{
		return _gameStatus;
	}
}

public enum GameStatus 
{
	NotStarted,
	Ongoing,
	SelectAction,
	SelectLocation,
	SelectCharacter,
	GameEnded
}