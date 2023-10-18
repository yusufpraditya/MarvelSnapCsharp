namespace MarvelSnap;

public class AntMan : CharacterCard
{
	private int _buffId = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;

	public AntMan(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);
	}

	public override void Ongoing(IPlayer player, MarvelSnapGame controller)
	{
		List<CharacterCard> arenaCards = controller.GetArenaCards(player, Arena);

		if (!IsOngoingEffectActivated && arenaCards.Count == 4)
		{
			IsOngoingEffectActivated = true;
			OngoingEffectActivationCount++;
			_buffId = GetLatestBuffId(player) + 1;
			AddBuff(player.Id, new Buff(_buffId, _BuffValue, _BuffType, _BuffOperation));
			controller.NotifyPowerChanged(player, this);

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

	public override AntMan DeepCopy()
	{
		return new AntMan(CharacterType.AntMan, "Ant-Man", "Ongoing: If you have 3 other cards here, +3 Power.", 1, 1, true);
	}
}