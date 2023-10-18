namespace MarvelSnap;

public class ProjectPegasus : LocationCard
{
	public ProjectPegasus(LocationType id, string name, string description) : base(id, name, description)
	{

	}
	
	public override void OnReveal(IPlayer? player, MarvelSnapGame controller)
	{
		if (IsRevealed) return;
		IsRevealed = true;
		List<IPlayer> players = controller.GetPlayers();

		foreach (var p in players)
		{
			int taskId = controller.GetLatestTaskId(p.Id) + 1;
			int buffId = controller.GetLatestEnergyBuffId(p.Id) + 1;
			Buff buff = new(buffId, 5, BuffType.Energy, BuffOperation.Add);
			controller.AddEnergyBuff(p.Id, buff);
			controller.AddFutureTask(p.Id, new FutureTask(taskId, controller.Turn) { Action = () => { FutureTask(controller, p.Id, buffId); } });
		}

		controller.NotifyCardRevealed(null, this);
	}

	private void FutureTask(MarvelSnapGame controller, int ownerId, int buffId)
	{
		controller.RemoveEnergyBuff(ownerId, buffId);
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

	public override ProjectPegasus DeepCopy()
	{
		return new ProjectPegasus(LocationType.ProjectPegasus, "Project Pegasus", "+5 Energy this turn.");
	}
}