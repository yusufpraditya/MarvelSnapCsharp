﻿namespace MarvelSnap;

public class Hawkeye : CharacterCard
{
	private int _id = 0;
	private int _cardCount;
	private Player? _player;
	private MarvelSnapGame? _controller;
	private const int _BuffValue = 3;
	private const BuffType _BuffType = BuffType.Power;
	private const BuffOperation _BuffOperation = BuffOperation.Add;
	
	public Hawkeye(CharacterType id, string name, string description, int baseEnergyCost, int basePower, bool hasAbility) : base(id, name, description, baseEnergyCost, basePower, hasAbility)
	{
	}

	public override void OnReveal(Player player, MarvelSnapGame controller)
	{
		_player = player;
		_controller = controller;
		if (!IsRevealed) 
		{
			IsRevealed = true;
			Dictionary<Player, List<CharacterCard>> arenaCards = controller.GetArenaCardsForEachPlayer();
			_cardCount = arenaCards[player].Count;
			controller.AddFutureTask(player.Id, new FutureTask(0, controller.Turn + 1) { Action = FutureTask });
			controller.NotifyCardRevealed(player, this);
		}
	}
	
	private void FutureTask() 
	{
		if (_controller != null && _player != null) 
		{
			Dictionary<Player, List<CharacterCard>> arenaCards = _controller.GetArenaCardsForEachPlayer();
			
			if (arenaCards[_player].Count > _cardCount) 
			{
				AddBuff(_player.Id, new Buff(_id, _BuffValue, _BuffType, _BuffOperation));
				_controller.NotifyPowerChanged(_player, this);
				_id += 1;
			}
		}
	}
}