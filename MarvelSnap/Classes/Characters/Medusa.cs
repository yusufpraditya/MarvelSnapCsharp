using System.Text.Json;

namespace MarvelSnap;

public class Medusa : CharacterCard
{
	private int _buffId = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public Medusa(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}
	
	public Medusa() 
	{
		
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
			Dictionary<ArenaType, Arena> arenas = controller.GetArenas();
			foreach (var kvp in arenas) 
			{
				if (kvp.Value.GetCards(player).Contains(this)) 
				{
					if (kvp.Key == ArenaType.Arena2) 
					{
						AddBuff(player.Id, new Buff(_buffId, _BuffValue, _BuffType, _BuffOperation));
						controller.NotifyPowerChanged(player, this);
						_buffId += 1;
					}
				}
			}
		}
	}
	
	public override Medusa? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Medusa? card = JsonSerializer.Deserialize<Medusa>(json);
		return card;
	}
}