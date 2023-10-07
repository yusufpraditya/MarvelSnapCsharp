namespace MarvelSnap;

public class AntMan : CharacterCard
{
	private int _id = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public AntMan(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
	}

	public override void Ongoing(Player player, MarvelSnapGame controller)
	{
		Console.WriteLine("ongoing");
		Thread.Sleep(1000);
		ArenaType? arenaId = controller.GetArenaId(player, this);
		List<CharacterCard> arenaCards = controller.GetArenaCards(player, (ArenaType) arenaId);

		if (arenaCards.Count == 4) 
		{
			AddBuff(player.Id, new Buff(_id, _BuffValue, _BuffType, _BuffOperation));
			controller.NotifyPowerChanged(player, this);
			_id += 1;
		}
	}
}