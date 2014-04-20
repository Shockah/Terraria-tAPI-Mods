using System.Collections.Generic;
using TAPI;

namespace Shockah.ItemSuffixes
{
	public class MWorld : ModWorld
	{
		public static WGTApplyWorldItems Task = null;

		public MWorld(ModBase modBase) : base(modBase) { }

		public override void WorldGenModifyTaskList(List<WorldGenTask> list)
		{
			if (Task == null) Task = new WGTApplyWorldItems(modBase);
			list.Add(Task);
		}
	}
}