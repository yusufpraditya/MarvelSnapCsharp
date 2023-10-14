using System.Text.Json;

namespace MarvelSnap;

public class AntMan : CharacterCard
{
	private int _buffId = 0;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public AntMan(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}

	public AntMan() 
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

	public override void Ongoing(Player player, MarvelSnapGame controller)
	{
		List<CharacterCard> arenaCards = controller.GetArenaCards(player, Arena);

		if (!IsOngoingEffectActivated)
		{
			if (arenaCards.Count == 4) 
			{
				IsOngoingEffectActivated = true;
				OngoingEffectActivationCount++;
				int buffId = GetLatestBuffId(player) + 1;
				AddBuff(player.Id, new Buff(buffId, _BuffValue, _BuffType, _BuffOperation));
				controller.NotifyPowerChanged(player, this);
			}
		}
	}

	public override AntMan? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		AntMan? card = JsonSerializer.Deserialize<AntMan>(json);
		return card;
	}
}