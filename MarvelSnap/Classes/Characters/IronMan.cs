namespace MarvelSnap;
public class IronMan : CharacterCard
{
	private int _buffId = 0;

	public IronMan(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);
	}

	public override void Ongoing(IPlayer? player, MarvelSnapGame controller)
	{
		if (IsOngoingEffectActivated) return;
		IsOngoingEffectActivated = true;
		OngoingEffectActivationCount++;
		_buffId = controller.GetLatestArenaBuffId(player, Arena) + 1;
		Buff buff = new(_buffId, 2, BuffType.Power, BuffOperation.Multiply);
		controller.AddPowerBuffToArena(player.Id, Arena, buff);
		Dictionary<ArenaType, Arena> arenas = controller.GetArenas();
		controller.NotifyArenaPowerChanged(player, arenas[Arena]);
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override IronMan DeepCopy()
	{
		return new IronMan(CharacterType.IronMan, "Iron Man", "Ongoing: Your total Power is doubled at this location.", 5, 0, true);
	}
}
