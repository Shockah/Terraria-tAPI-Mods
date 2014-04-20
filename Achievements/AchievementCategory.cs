using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shockah.Achievements
{
	public class AchievementCategory
	{
		public string name;
		public AchievementCategory parent;
		public List<AchievementCategory> children = new List<AchievementCategory>();
		public List<Achievement> achievements = new List<Achievement>();

		public AchievementCategory(string name, AchievementCategory parent = null)
		{
			this.name = name;
			this.parent = parent;
		}

		public void Register()
		{
			Achievements.me.categories.Add(this);
		}

		public static AchievementCategory operator +(AchievementCategory parent, AchievementCategory child)
		{
			parent.children.Add(child);
			child.parent = parent;
			return parent;
		}
		public static AchievementCategory operator +(AchievementCategory category, Achievement a)
		{
			category.achievements.Add(a);
			a.category = category;
			return category;
		}
	}
}