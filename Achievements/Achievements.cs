using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public class Achievements
	{
		public static Achievements me = null;
		
		internal Achievements() { }
		
		public SEvent<Action<Player, Achievement, object>> EventAchieved = new SEvent<Action<Player, Achievement, object>>();
		
		internal List<AchievementCategory> categories = new List<AchievementCategory>();

		public List<Achievement> AllAchievements()
		{
			List<Achievement> ret = new List<Achievement>();
			foreach (AchievementCategory ac in categories) ret.AddRange(AllAchievements(ac));
			return ret;
		}
		public List<Achievement> AllAchievements(AchievementCategory ac)
		{
			List<Achievement> ret = new List<Achievement>();
			foreach (AchievementCategory ac2 in ac.children) ret.AddRange(AllAchievements(ac2));
			foreach (Achievement a in ac.achievements) ret.AddRange(AllAchievements(a));
			return ret;
		}
		public List<Achievement> AllAchievements(Achievement a)
		{
			List<Achievement> ret = new List<Achievement>();
			ret.Add(a);
			foreach (Achievement a2 in a.children) ret.AddRange(AllAchievements(a2));
			return ret;
		}
	}
}