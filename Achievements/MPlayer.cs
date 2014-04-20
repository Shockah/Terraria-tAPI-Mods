using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public class MPlayer : ModPlayer
	{
		public MPlayer(ModBase modBase, Player p) : base(modBase, p) { }

		public override void Save(BinBuffer bb)
		{
			List<Achievement> list = Achievements.me.AllAchievements();
			bb.Write(list.Count);
			foreach (Achievement a in list)
			{
				bb.Write(a.modBase.modName);
				bb.Write(a.apiName);
				bb.Write(a.achieved);
			}
		}

		public override void Load(BinBuffer bb)
		{
			if (bb.IsEmpty()) return;
			List<Achievement> list = Achievements.me.AllAchievements();
			foreach (Achievement a in list) a.achieved = false;
			int count = bb.ReadInt();
			while (count-- > 0)
			{
				string modName = bb.ReadString();
				string apiName = bb.ReadString();
				bool achieved = bb.ReadBool();
				if (!achieved) continue;
				Achievement a = list.Find((_a) => { return _a.modBase.modName == modName && _a.apiName == apiName; });
				if (a != null) a.achieved = true;
			}
		}
	}
}