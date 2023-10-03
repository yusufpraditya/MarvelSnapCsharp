namespace MarvelSnap;

public class Hawkeye : CharacterCard, IRevealable
{
	public Hawkeye(CharacterType type, int id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(type, id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}

    public void OnReveal(IPlayer player, MarvelSnapGame controller)
    {
        
    }
}
