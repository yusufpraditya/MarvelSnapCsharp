namespace MarvelSnap;

public class Sentinel : CharacterCard
{
	public Sentinel(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
			
			controller.AddCardInHand(player, Copy());
		}
	}
}
