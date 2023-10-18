namespace MarvelSnap;

public class StarkTower : LocationCard
{
	public StarkTower(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		controller.NotifyCardRevealed(null, this);
	}

	public override void Ongoing(IPlayer? player, MarvelSnapGame controller)
	{
		if (controller.Turn <= 5) return;
		List<IPlayer> players = controller.GetPlayers();
		Dictionary<ArenaType, Arena> arenas = controller.GetArenas();
		Dictionary<IPlayer, List<CharacterCard>> playerCardsInArena = controller.GetArenaCardsForEachPlayer();

		foreach (var p in players)
		{
			foreach (var card in playerCardsInArena[p])
			{
				if (arenas[card.Arena].Location == this)
				{
					int buffId = card.GetLatestBuffId(p) + 1;
					Buff buff = new(buffId, 2, BuffType.Power, BuffOperation.Add);
					card.AddBuff(p.Id, buff);
					controller.NotifyPowerChanged(p, card);
				}
			}
		}
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

    public override StarkTower DeepCopy()
    {
        return new StarkTower(LocationType.StarkTower, "Stark Tower", "After turn 5, give all cards here +2 Power.");
    }
}