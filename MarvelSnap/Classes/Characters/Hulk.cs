using System.Text.Json;

namespace MarvelSnap;

public class Hulk : CharacterCard
{
	public Hulk(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}
	
	public Hulk()
	{
		
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
		}
	}
	
	public override Hulk? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Hulk? card = JsonSerializer.Deserialize<Hulk>(json);
		return card;
	}
}
