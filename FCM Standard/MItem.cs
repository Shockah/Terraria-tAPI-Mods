using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	[GlobalMod] public class MItem : ModItem
	{
		public MItem(ModBase modBase, Item item) : base(modBase, item) { }

		[CallPriority(-10000f)] public override string OnAffixName(string currentName, string oldName)
		{
			if (InterfaceFCMItems.displayIds)
			{
				currentName = "[" + item.type + (item.netID != item.type ? "/" + item.netID : "") + "] " + currentName;
				InterfaceFCMItems.displayIds = false;
			}
			return currentName;
		}
	}
}