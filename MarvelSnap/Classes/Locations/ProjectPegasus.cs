using System.Text.Json;

namespace MarvelSnap;

public class ProjectPegasus : LocationCard
{
	private MarvelSnapGame? _controller;

	public ProjectPegasus(LocationType id, string name, string description) : base(id, name, description)
	{

	}

	public ProjectPegasus()
	{

	}

	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (!IsRevealed)
		{
			_controller = controller;
			IsRevealed = true;
			List<IPlayer> players = controller.GetPlayers();

			foreach (var p in players)
			{
				int taskId = controller.GetLatestTaskId(p.Id) + 1;
				int buffId = controller.GetLatestEnergyBuffId(p.Id) + 1;
				Buff buff = new(buffId, 5, BuffType.Energy, BuffOperation.Add);
				controller.AddEnergyBuff(p.Id, buff);
				controller.AddFutureTask(p.Id, new FutureTask(taskId, controller.Turn) { Action = () => { FutureTask(p.Id, buffId); } });
			}

			controller.NotifyCardRevealed(null, this);
		}
	}

	private void FutureTask(int ownerId, int buffId)
	{
		if (_controller != null)
		{
			_controller.RemoveEnergyBuff(ownerId, buffId);
		}
	}

	public override ProjectPegasus? DeepCopy()
	{
		string json = JsonSerializer.Serialize(this);
		ProjectPegasus? card = JsonSerializer.Deserialize<ProjectPegasus>(json);
		return card;
	}

	public override void Ongoing(IPlayer player, MarvelSnapGame controller)
	{
		// ignored
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
