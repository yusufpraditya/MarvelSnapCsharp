namespace MarvelSnap;

public class Elektra : CharacterCard
{
	public Elektra(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);

		IPlayer opponent = controller.GetOpponent(player);
		List<CharacterCard> arenaCards = controller.GetArenaCards(opponent, Arena);
		List<CharacterCard> targetCards = new();
		Random random = new();

		foreach (var card in arenaCards)
		{
			if (card.IsRevealed && card.BaseEnergyCost == 1)
			{
				targetCards.Add(card);
			}
		}

		if (targetCards.Count > 0)
		{
			int target = random.Next(0, targetCards.Count - 1);
			controller.DestroyCard(opponent, targetCards[target]);
			controller.NotifyCardDestroyed(player, this, targetCards[target]);
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

	public override Elektra DeepCopy()
	{
		return new Elektra(CharacterType.Elektra, "Elektra", "On Reveal: Destroy a random enemy 1-Cost card at this location.", 1, 1, true);
	}
}
