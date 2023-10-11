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

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
			
			controller.AddCardInHand(player, DeepCopy());
		}
	}
	
	public override Sentinel? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Sentinel? card = JsonSerializer.Deserialize<Sentinel>(json);
		return card;
	}
}
