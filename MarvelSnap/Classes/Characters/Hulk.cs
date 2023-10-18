namespace MarvelSnap;

public class Hulk : CharacterCard
{
	public Hulk(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
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

	public override Hulk DeepCopy()
	{
		return new Hulk(CharacterType.Hulk, "Hulk", "HULK SMASH!", 6, 12, false);
	}
}
