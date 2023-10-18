namespace MarvelSnap.Tests;

public class ControllerTests
{
	private MarvelSnapGame _controller;
	private IPlayer _player1 = new Player(1);
	private IPlayer _player2 = new Player(2);

	public ControllerTests()
	{
		_controller = new(_player1, _player2);
	}

	[Fact]
	public void GetPlayersTest()
	{	
		Assert.Contains(_player1, _controller.GetPlayers());
		Assert.Contains(_player2, _controller.GetPlayers());
	}
}