﻿namespace MarvelSnap;

public class MisterFantastic : CharacterCard
{
	private bool _powerBuffActivated;
	private int _id = 0;
	private const int _BuffValue = 2;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public MisterFantastic(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
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
		List<LocationCard> locations = controller.GetLocations();
		List<Arena> arenas = controller.GetListOfArenas();
		
		if (HasMoved) 
		{
			_powerBuffActivated = false;
			HasMoved = false;
			foreach (var arena in arenas) 
			{
				controller.RemovePowerBuffFromArena(player.Id, arena.Id, _id);
			}
		}
		if (!_powerBuffActivated) 
		{
			_powerBuffActivated = true;
			foreach (var arena in arenas) 
			{
				_id = arena.GetLatestBuffId(player) + 1;
				Buff buff = new(_id, _BuffValue, _BuffType, _BuffOperation);
				if (Location == arena.Id) 
				{
					if (arena.Location == locations[0]) 
					{
						controller.AddPowerBuffToArena(player.Id, ArenaType.Arena2, buff);
						controller.NotifyArenaPowerChanged(player, arenas[1]);
					}
					else if (arena.Location == locations[1]) 
					{
						controller.AddPowerBuffToArena(player.Id, ArenaType.Arena1, buff);
						controller.AddPowerBuffToArena(player.Id, ArenaType.Arena3, buff);
						controller.NotifyArenaPowerChanged(player, arenas[0]);
						controller.NotifyArenaPowerChanged(player, arenas[2]);
					}
					else if (arena.Location == locations[2]) 
					{
						controller.AddPowerBuffToArena(player.Id, ArenaType.Arena2, buff);
						controller.NotifyArenaPowerChanged(player, arenas[1]);
					}
				}
			}
		}
	}
}