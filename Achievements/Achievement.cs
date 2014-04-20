using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public class Achievement
	{
		public readonly ModBase modBase;
		public readonly string apiName;
		public Achievement parent;
		public List<Achievement> children = new List<Achievement>();
		public AchievementCategory category;
		public string name, description;
		public int value = 5;
		public bool achieved = false;
		public object icon = null;

		public Achievement(ModBase modBase, string apiName, AchievementCategory category = null, Achievement parent = null)
		{
			this.modBase = modBase;
			this.apiName = apiName;
			this.category = category;
			this.parent = parent;
		}

		public Achievement SetInfo(string name, string description = null)
		{
			this.name = name;
			this.description = description;
			return this;
		}

		public void Achieve(object icon = null)
		{
			if (!achieved)
			{
				achieved = true;
				foreach (Action<Player, Achievement, object> h in Achievements.me.EventAchieved) h(Main.localPlayer, this, icon);
			}
		}

		public static Achievement operator +(Achievement parent, Achievement child)
		{
			parent.children.Add(child);
			child.parent = parent;
			return parent;
		}
	}
}