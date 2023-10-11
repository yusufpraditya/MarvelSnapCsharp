namespace MarvelSnap;

public class DreamDimension : LocationCard
{
	private const int _FutureTurn1 = 5;
	private const int _FutureTurn2 = 6;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Energy;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	private Dictionary<CharacterCard, Buff> _cardBuffs = new();
	private MarvelSnapGame? _controller;
	
	public DreamDimension(LocationType id, string name, string description) : base(id, name, description)
	{
	}

	public override void OnReveal(Player? player, MarvelSnapGame controller)
	{
		_controller = controller;
		List<Player> players = controller.GetPlayers();
		if (!IsRevealed) 
		{
			IsRevealed = true;
			int taskId = controller.GetLatestTaskId() + 1;
			foreach (var p in players) 
			{
				controller.AddFutureTask(p.Id, new FutureTask(taskId, _FutureTurn1) { Action = FutureTask });
				controller.AddFutureTask(p.Id, new FutureTask(taskId, _FutureTurn2) { Action = FutureTask });
			}
			
			controller.NotifyCardRevealed(null, this);
		}
	}
	
	private void FutureTask() 
	{
		if (_controller != null) 
		{
			List<Player> players = _controller.GetPlayers();
			Dictionary<Player, List<CharacterCard>> handCards = _controller.GetHandCardsForEachPlayer();
			
			foreach (var player in players) 
			{
				foreach (var card in handCards[player]) 
				{
					if (_controller.Turn == _FutureTurn1) 
					{
						int id = card.GetLatestBuffId(player) + 1;
						Buff buff = new Buff(id, _BuffValue, _BuffType, _BuffOperation);
						_cardBuffs.TryAdd(card, buff);
						card.AddBuff(player.Id, buff);
						_controller.NotifyEnergyCostChanged(player, card);
					}
					if (_controller.Turn == _FutureTurn2) 
					{
						if (_cardBuffs.ContainsKey(card))
							card.RemoveBuff(player.Id, _cardBuffs[card].Id);
					}
				}
			}
		}
	}
}