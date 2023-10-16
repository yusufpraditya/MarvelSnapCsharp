﻿using System.Text.Json;

namespace MarvelSnap;

public class Kyln : LocationCard
{
	public Kyln(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public Kyln()
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (!IsRevealed)
		{
			IsRevealed = true;
			controller.NotifyCardRevealed(null, this);
		}
	}

	public override void Ongoing(IPlayer? player, MarvelSnapGame controller)
	{
		if (controller.Turn > 4)
		{
			List<Arena> arenas = controller.GetListOfArenas();
			foreach (var arena in arenas)
			{
				if (arena.Location == this)
					arena.SetAvailable(false);
			}
		}
	}

	public override Kyln? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		Kyln? card = JsonSerializer.Deserialize<Kyln>(json);
		return card;
	}

	public override void OnDestroyed(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}

	public override void OnMoved(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
	}
}