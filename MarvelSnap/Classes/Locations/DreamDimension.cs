namespace MarvelSnap;

public class DreamDimension : LocationCard
{
	private const int _FutureTurn = 5;
	private MarvelSnapGame? _controller;
	
	public DreamDimension(LocationType id, string name, string description) : base(id, name, description)
	{
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		_controller = controller;
		if (!IsRevealed) 
		{
			IsRevealed = true;
			int taskId = controller.GetLatestTaskId() + 1;
			controller.AddFutureTask(player.Id, new FutureTask(taskId, _FutureTurn) { Action = FutureTask });
			controller.NotifyCardRevealed(player, this);
		}
	}
	
	private void FutureTask() 
	{
		if (_controller != null) 
		{
			Dictionary<Player, List<CharacterCard>> handCards = _controller.GetHandCardsForEachPlayer();
			foreach (var kvp in handCards) 
			{
				foreach (var card in kvp.Value) 
				{
					card.BaseEnergyCost += 1;
				}
			}
		}
	}
}