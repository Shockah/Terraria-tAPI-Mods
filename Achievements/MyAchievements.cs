using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public static class MyAchievements
	{
		public static AchievementCategory
			CategoryGeneral = null;
		public static Achievement[]
			ARareFind = null;
		
		public static void Init()
		{
			(CategoryGeneral = new AchievementCategory("General")).Register();

			string[] rarityNames = new string[] { "Blue", "Green", "Orange", "Red", "Magenta", "Purple", "Light Green", "Yellow", "Cyan" };
			ARareFind = new Achievement[rarityNames.Length];
			for (int i = 0; i < rarityNames.Length; i++)
			{
				if (i == 0)
				{
					CategoryGeneral += (ARareFind[i] = new Achievement(MBase.me, "RareFind" + i)
						.SetInfo("A Rare (" + rarityNames[i] + ") Find", "Obtain a " + rarityNames[i].ToLower() + "item"));
				}
				else
				{
					ARareFind[i - 1] += (ARareFind[i] = new Achievement(MBase.me, "RareFind" + i)
						.SetInfo("A Rare (" + rarityNames[i] + ") Find", "Obtain a " + rarityNames[i].ToLower() + "item"));
				}
			}

			MBase.sbase.EventInventoryChange += (player, type, slot, oldItem, newItem) =>
				{
					for (int i = 0; i < ARareFind.Length; i++)
					{
						if (newItem.rare >= i + 1) ARareFind[i].Achieve(newItem);
					}
				};

			Achievements.me.EventAchieved += (player, achievement, icon) =>
				{
					MInterface.LayerNotifiers.Add(achievement, icon);
				};
		}
	}
}