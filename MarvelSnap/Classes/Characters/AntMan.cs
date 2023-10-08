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

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			controller.NotifyCardRevealed(player, this);
		}
	}

	public override void Ongoing(Player player, MarvelSnapGame controller)
	{
		Dictionary<Player, List<CharacterCard>> arenaCards = controller.GetArenaCardsForEachPlayer();

		if (arenaCards[player].Count == 4) 
		{
			AddBuff(player.Id, new Buff(_id, _BuffValue, _BuffType, _BuffOperation));
			controller.NotifyPowerChanged(player, this);
			_id += 1;
		}
	}
}