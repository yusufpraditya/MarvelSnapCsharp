using System.Text.Json;

namespace MarvelSnap;

public class Sentinel : CharacterCard
{
	public Sentinel(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public Sentinel()
	{

	}

	public override void OnReveal(IPlayer player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		CardTurn = controller.Turn;
		controller.NotifyCardRevealed(player, this);
		var newSentinel = DeepCopy();
		newSentinel.IsRevealed = false;

		controller.AddCardInHand(player, newSentinel);
	}

	public override Sentinel DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Sentinel card = JsonSerializer.Deserialize<Sentinel>(json)!;
		return card;
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
}
