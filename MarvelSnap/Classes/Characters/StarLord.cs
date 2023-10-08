namespace MarvelSnap;

public class StarLord : CharacterCard
{
	private int _id = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public StarLord(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
			
			Player opponent = controller.GetOpponent(player);
			Dictionary<Player, List<CharacterCard>> arenaCards = controller.GetArenaCardsForEachPlayer();

			foreach (var card in arenaCards[opponent]) 
			{
				if (card.CardTurn == CardTurn) 
				{
					if (card.Location == Location) 
					{
						AddBuff(player.Id, new Buff(_id, _BuffValue, _BuffType, _BuffOperation));
						controller.NotifyPowerChanged(player, this);
						_id += 1;
					}
				}
			}
		}
	}
}
