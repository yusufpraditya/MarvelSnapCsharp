namespace MarvelSnap;

public class MarvelSnapGame
{
	private IPlayer _player1, _player2;
	public const int MaxCardInHand = 7;
	public const int DefaultMaxTurn = 6;
	
	public MarvelSnapGame(IPlayer player1, IPlayer player2) 
	{
		_player1 = player1;
		_player2 = player2;
	}
}
