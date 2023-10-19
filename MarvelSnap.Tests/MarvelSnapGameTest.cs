using Moq;

namespace MarvelSnap.Tests;

public class MarvelSnapGameTest
{
	private MarvelSnapGame _controller;
	private Mock<IPlayer> _player1;
	private Mock<IPlayer> _player2;

	public MarvelSnapGameTest()
	{
		_player1 = new();
		_player2 = new();
		
		_player1.SetupProperty(p => p.Id, 1);
		_player2.SetupProperty(p => p.Id, 2);
		
		_controller = new(_player1.Object, _player2.Object);
	}

	[Fact]
	public void GetPlayers_ReturnsListIPlayer_PlayerExists()
	{	
		List<IPlayer> actual = _controller.GetPlayers();
		
		Assert.Contains(_player1.Object, actual);
		Assert.Contains(_player2.Object, actual);
	}
	
	[Fact]
	public void GetOpponent_ReturnsIPlayer_OpponentExists() 
	{
		IPlayer actual = _controller.GetOpponent(_player1.Object);
		
		Assert.Equal(_player2.Object, actual);
	}
	
	[Fact]
	public void GetPlayerTurn_ReturnsIPlayer() 
	{
		_controller.SetPlayerTurn(_player1.Object);
		
		IPlayer actual = _controller.GetPlayerTurn();
		
		Assert.Equal(_player1.Object, actual);
	}

	[Theory]
	[MemberData(nameof(PowerBuff))]
	public void GetTotalPowerOfArena_ReturnsInt(Buff powerBuff, int expectedPower) 
	{
		_controller.AddPowerBuffToArena(_player1.Object.Id, ArenaType.Arena1, powerBuff);
		
		int actualPower = _controller.GetTotalPowerOfArena(_player1.Object, ArenaType.Arena1);
		
		Assert.Equal(expectedPower, actualPower);
	}
	
	public static IEnumerable<object[]> PowerBuff() 
	{
		yield return new object[] 
		{
			new Buff(0, 3, BuffType.Power, BuffOperation.Add),
			3
		};
	}
}