namespace MarvelSnap;

public class MisterFantastic : CharacterCard
{
	private int _buffId = 0;
	private const int _BuffValue = 2;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;

	public MisterFantastic(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
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
		List<LocationCard> locations = controller.GetLocations();
		List<Arena> arenas = controller.GetListOfArenas();

		if (HasMoved)
		{
			IsOngoingEffectActivated = false;
			OngoingEffectActivationCount = 0;
			HasMoved = false;
			foreach (var arena in arenas)
			{
				controller.RemovePowerBuffFromArena(player.Id, arena.Id, _buffId);
			}
		}

		if (IsOngoingEffectActivated) return;
		IsOngoingEffectActivated = true;
		OngoingEffectActivationCount++;
		foreach (var arena in arenas)
		{
			_buffId = arena.GetLatestBuffId(player) + 1;
			Buff buff = new(_buffId, _BuffValue, _BuffType, _BuffOperation);
			if (Arena == arena.Id)
			{
				if (arena.Location == locations[0])
				{
					controller.AddPowerBuffToArena(player.Id, ArenaType.Arena2, buff);
					controller.NotifyArenaPowerChanged(player, arenas[1]);
				}
				else if (arena.Location == locations[1])
				{
					controller.AddPowerBuffToArena(player.Id, ArenaType.Arena1, buff);
					controller.AddPowerBuffToArena(player.Id, ArenaType.Arena3, buff);
					controller.NotifyArenaPowerChanged(player, arenas[0]);
					controller.NotifyArenaPowerChanged(player, arenas[2]);
				}
				else if (arena.Location == locations[2])
				{
					controller.AddPowerBuffToArena(player.Id, ArenaType.Arena2, buff);
					controller.NotifyArenaPowerChanged(player, arenas[1]);
				}
			}
		}
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		if (!IsDestroyed) return;
		List<LocationCard> locations = controller.GetLocations();
		List<Arena> arenas = controller.GetListOfArenas();
		IsOngoingEffectActivated = false;
		OngoingEffectActivationCount = 0;

		foreach (var arena in arenas)
		{
			if (Arena == arena.Id)
			{
				if (arena.Location == locations[0])
				{
					controller.RemovePowerBuffFromArena(player.Id, ArenaType.Arena2, _buffId);
				}
				else if (arena.Location == locations[1])
				{
					controller.RemovePowerBuffFromArena(player.Id, ArenaType.Arena1, _buffId);
					controller.RemovePowerBuffFromArena(player.Id, ArenaType.Arena3, _buffId);
				}
				else if (arena.Location == locations[2])
				{
					controller.RemovePowerBuffFromArena(player.Id, ArenaType.Arena2, _buffId);
				}
			}
		}
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override MisterFantastic DeepCopy()
	{
		return new(CharacterType.MisterFantastic, "Mister Fantastic", "Ongoing: Adjacent locations have +2 Power.", 3, 2, true);
	}
}
