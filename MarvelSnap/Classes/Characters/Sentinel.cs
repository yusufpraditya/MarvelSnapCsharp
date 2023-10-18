namespace MarvelSnap;

public class Sentinel : CharacterCard
{
	public Sentinel(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);
		controller.AddCardInHand(player, DeepCopy());
		IsRevealed = true;
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

	public override Sentinel DeepCopy()
	{
		return new Sentinel(CharacterType.Sentinel, "Sentinel", "On Reveal: Add another Sentinel to your hand.", 2, 3, true);
	}
}
