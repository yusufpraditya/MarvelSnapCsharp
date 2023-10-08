namespace MarvelSnap;

public class Medusa : CharacterCard
{
	private int _id = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public Medusa(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			Dictionary<ArenaType, Arena> arenas = controller.GetArenas();
			foreach (var kvp in arenas) 
			{
				if (kvp.Value.GetCards(player).Contains(this)) 
				{
					if (kvp.Key == ArenaType.Arena2) 
					{
						AddBuff(player.Id, new Buff(_id, _BuffValue, _BuffType, _BuffOperation));
						controller.NotifyPowerChanged(player, this);
						_id += 1;
					}
					controller.NotifyCardRevealed(player, this);
				}
			}
		}
	}
}