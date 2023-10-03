namespace MarvelSnap;

public class MarvelSnapGame
{
	private IPlayer _player1, _player2;
	public MarvelSnapGame(IPlayer player1, IPlayer player2) 
	{
		_player1 = player1;
		_player2 = player2;
	}
}
