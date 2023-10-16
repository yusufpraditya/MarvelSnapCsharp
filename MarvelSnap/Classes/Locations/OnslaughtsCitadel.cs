using System.Text.Json;

namespace MarvelSnap;

public class OnslaughtsCitadel : LocationCard
{
	public OnslaughtsCitadel(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public OnslaughtsCitadel()
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (!IsRevealed)
		{
			IsRevealed = true;
			controller.NotifyCardRevealed(null, this);
		}
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
				if (arenas[card.Arena].Location == this)
				{
					if (card.OngoingEffectActivationCount < CharacterCard.MaxOngoingEffectActivation)
					{
						card.IsOngoingEffectActivated = false;
						card.Ongoing(p, controller);
					}

				}
			}
		}
	}

	public override OnslaughtsCitadel? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		OnslaughtsCitadel? card = JsonSerializer.Deserialize<OnslaughtsCitadel>(json);
		return card;
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}
}