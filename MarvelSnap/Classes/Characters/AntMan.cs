namespace MarvelSnap;

public class AntMan : CharacterCard, IOngoing
{
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public AntMan(CharacterType type, int id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(type, id, name, description, baseEnergyCost, basePower, hasAbility)
	{

	}

	public void Ongoing(IPlayer player, MarvelSnapGame controller)
	{
		ArenaId arenaId = controller.GetArenaId(player, this);
		List<CharacterCard> arenaCards = controller.GetArenaCards(player, arenaId);
		Console.WriteLine("Arena cards count: " + arenaCards.Count);
		if (arenaCards.Count == 4) 
		{
			AddBuff(new Buff(_BuffValue, _BuffType, _BuffOperation));
		}
	}
}