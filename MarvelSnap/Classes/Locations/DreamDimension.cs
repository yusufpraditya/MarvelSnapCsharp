namespace MarvelSnap;

public class DreamDimension : LocationCard
{
	private const int _FutureTurn1 = 5;
	private const int _FutureTurn2 = 6;
	private const int _BuffValue = 1;
	private const BuffType _BuffType = BuffType.Energy;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	private Dictionary<CharacterCard, Buff> _cardBuffs = new();

	public DreamDimension(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		List<IPlayer> players = controller.GetPlayers();
		if (IsRevealed) return;
		IsRevealed = true;

		foreach (var p in players)
		{
			int taskId = controller.GetLatestTaskId(p.Id) + 1;
			controller.AddFutureTask(p.Id, new FutureTask(taskId, _FutureTurn1) { Action = () => { FutureTask(controller, p.Id); } });
			controller.AddFutureTask(p.Id, new FutureTask(taskId, _FutureTurn2) { Action = () => { FutureTask(controller, p.Id); } });
		}

		controller.NotifyCardRevealed(null, this);
	}

	private void FutureTask(MarvelSnapGame controller, int ownerId)
	{
		List<IPlayer> players = controller.GetPlayers();

		// Add FindPlayerById in GC
		foreach (var player in players)
		{
			if (player.Id != ownerId)
			{
				continue;
			}

			List<CharacterCard?> handCards = controller.GetHandCards(player);
			foreach (var card in handCards)
			{
				if (card == null) continue;
				if (controller.Turn == _FutureTurn1)
				{
					int id = card.GetLatestBuffId(player) + 1;
					Buff buff = new Buff(id, _BuffValue, _BuffType, _BuffOperation);
					_cardBuffs.TryAdd(card, buff);
					card.AddBuff(player.Id, buff);
					controller.NotifyEnergyCostChanged(player, card);
				}
				if (controller.Turn == _FutureTurn2 && _cardBuffs.ContainsKey(card))
				{
					card.RemoveBuff(player.Id, _cardBuffs[card].Id);
					controller.NotifyEnergyCostChanged(player, card);
				}
			}
		}
	}

	public override void Ongoing(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

    public override DreamDimension DeepCopy()
    {
        return new DreamDimension(LocationType.DreamDimension, "Dream Dimension", "On turn 5, cards cost 1 more.");
    }
}