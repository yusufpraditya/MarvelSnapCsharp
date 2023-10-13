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