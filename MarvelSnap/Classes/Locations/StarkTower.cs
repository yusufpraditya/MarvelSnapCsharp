using System.Text.Json;

namespace MarvelSnap;

public class StarkTower : LocationCard
{
	public StarkTower(LocationType id, string name, string description) : base(id, name, description)
	{
		
	}
	
	public StarkTower() 
	{
		
	}

	public override void OnReveal(Player? player, MarvelSnapGame controller)
	{
		if (!IsRevealed) 
		{
			IsRevealed = true;
			controller.NotifyCardRevealed(null, this);
		}
	}

	public override void Ongoing(Player? player, MarvelSnapGame controller)
	{
		if (controller.Turn > 5) 
		{
			List<Player> players = controller.GetPlayers();
			List<Arena> arenas = controller.GetListOfArenas();
			
			foreach (var p in players) 
			{
				foreach (var arena in arenas) 
				{
					List<CharacterCard> arenaCards = controller.GetArenaCards(p, arena.Id);
					foreach (var card in arenaCards) 
					{
						if (arena.Location == this) 
						{
							int buffId = card.GetLatestBuffId(p) + 1;
							Buff buff = new(buffId, 2, BuffType.Power, BuffOperation.Add);
							card.AddBuff(p.Id, buff);
							controller.NotifyPowerChanged(p, card);
						}
					}
				}
			}
		}
	}

	public override StarkTower? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		StarkTower? card = JsonSerializer.Deserialize<StarkTower>(json);
		return card;
	}
}
