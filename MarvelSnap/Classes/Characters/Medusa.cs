﻿namespace MarvelSnap;

public class Medusa : CharacterCard
{
	private int _buffId = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;

	public Medusa(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);
		Dictionary<ArenaType, Arena> arenas = controller.GetArenas();
		foreach (var kvp in arenas)
		{
			if (kvp.Value.GetCards(player).Contains(this) && kvp.Key == ArenaType.Arena2)
			{
				AddBuff(player.Id, new Buff(_buffId, _BuffValue, _BuffType, _BuffOperation));
				controller.NotifyPowerChanged(player, this);
				_buffId += 1;
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

	public override Medusa DeepCopy()
	{
		return new Medusa(CharacterType.Medusa, "Medusa", "On Reveal: If this is at the middle location, +3 Power.", 2, 2, true);
	}
}