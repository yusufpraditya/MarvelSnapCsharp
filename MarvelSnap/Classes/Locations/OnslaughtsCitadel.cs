namespace MarvelSnap;

public class OnslaughtsCitadel : LocationCard
{
	public OnslaughtsCitadel(LocationType id, string name, string description) : base(id, name, description)
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
		List<IPlayer> players = controller.GetPlayers();
		Dictionary<IPlayer, List<CharacterCard>> playerCardsInArena = controller.GetArenaCardsForEachPlayer();
		Dictionary<ArenaType, Arena> arenas = controller.GetArenas();

		foreach (var p in players)
		{
			foreach (var card in playerCardsInArena[p])
			{
				if (arenas[card.Arena].Location == this && card.OngoingEffectActivationCount < CharacterCard.MaxOngoingEffectActivation)
				{
					card.IsOngoingEffectActivated = false;
					card.Ongoing(p, controller);
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

    public override OnslaughtsCitadel DeepCopy()
    {
        return new OnslaughtsCitadel(LocationType.OnslaughtsCitadel, "Onslaught's Citadel", "Ongoing effects here are doubled.");
    }
}