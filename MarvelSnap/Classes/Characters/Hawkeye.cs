namespace MarvelSnap;

public class Hawkeye : CharacterCard
{
	private int _cardCount;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;

	public Hawkeye(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		List<CharacterCard> arenaCards = controller.GetArenaCards(player, Arena);
		_cardCount = arenaCards.Count;
		controller.AddFutureTask(player.Id, new FutureTask(0, controller.Turn + 1) { Action = () => { FutureTask(player, controller); } });
		controller.NotifyCardRevealed(player, this);
	}

	private void FutureTask(IPlayer player, MarvelSnapGame controller)
	{
		List<CharacterCard> arenaCards = controller.GetArenaCards(player, Arena);

		if (arenaCards.Count > _cardCount)
		{
			int buffId = GetLatestBuffId(player) + 1;
			AddBuff(player.Id, new Buff(buffId, _BuffValue, _BuffType, _BuffOperation));
			controller.NotifyPowerChanged(player, this);
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

	public override Hawkeye DeepCopy()
	{
		return new Hawkeye(CharacterType.Hawkeye, "Hawkeye", "On Reveal: If you play a card here next turn, +3 Power.", 1, 1, true);
	}
}