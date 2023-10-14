using System.Text.Json;

namespace MarvelSnap;
public class IronMan : CharacterCard
{
	private int _buffId = 0;

	public IronMan(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
		
	}
	
	public IronMan() 
	{
		
	}

	public override void OnReveal(Player? player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			CardTurn = controller.Turn;
			controller.NotifyCardRevealed(player, this);
		}
	}

	public override void Ongoing(Player? player, MarvelSnapGame controller)
	{
		if (!IsOngoingEffectActivated) 
		{
			IsOngoingEffectActivated = true;
			_buffId = controller.GetLatestArenaBuffId(player, Arena) + 1;
			Buff buff = new(_buffId, 2, BuffType.Power, BuffOperation.Multiply);
			controller.AddPowerBuffToArena(player.Id, Arena, buff);
			Dictionary<ArenaType, Arena> arenas = controller.GetArenas();
			controller.NotifyArenaPowerChanged(player, arenas[Arena]);
		}
	}
	
	public override IronMan? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		IronMan? card = JsonSerializer.Deserialize<IronMan>(json);
		return card;
	}
}
