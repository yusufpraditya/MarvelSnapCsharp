namespace MarvelSnap;

public class FutureTask
{
	private int _turn;
	public int Id { get; set; }
	public Action? Action { get; set; }
	
	public FutureTask(int id, int turn) 
	{
		Id = id;
		_turn = turn;
	}
	
	public bool Run(int turn) 
	{
		if (turn == _turn) 
		{
			Action?.Invoke();
			return true;
		}
		return false;
	}
}
