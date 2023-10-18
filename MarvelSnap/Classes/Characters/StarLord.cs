namespace MarvelSnap;

public class StarLord : CharacterCard
{
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;

	public StarLord(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);

		IPlayer opponent = controller.GetOpponent(player);
		Dictionary<IPlayer, List<CharacterCard>> arenaCards = controller.GetArenaCardsForEachPlayer();

		foreach (var card in arenaCards[opponent])
		{
			if (card.CardTurn == CardTurn)
			{
				if (card.Arena == Arena)
				{
					int buffId = GetLatestBuffId(player) + 1;
					AddBuff(player.Id, new Buff(buffId, _BuffValue, _BuffType, _BuffOperation));
					controller.NotifyPowerChanged(player, this);
					break;
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

	public override StarLord DeepCopy()
	{
		return new StarLord(CharacterType.StarLord, "Star Lord", "On Reveal: If your opponent played a card here this turn, +3 Power.", 2, 2, true);
	}
}
