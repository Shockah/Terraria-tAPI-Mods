using Shockah.Base;
using System;
using TAPI;

namespace Shockah.ItemSuffixes
{
	public class MBase : ModBase
	{
		public static ModBase me = null;

		public override void OnLoad()
		{
			me = this;
		}
		
		public override void OnAllModsLoaded()
		{
			SBase.EventNPCLoot += (npc, item) =>
			{
				MItem mitem = item.GetSubClass<MItem>();
				if (mitem == null) return;
				if (mitem.CanGetSuffixes()) mitem.SetRandomSuffix();
			};
			SBase.EventTileLoot += (point, item) =>
			{
				MItem mitem = item.GetSubClass<MItem>();
				if (mitem == null) return;
				if (mitem.CanGetSuffixes()) mitem.SetRandomSuffix();
			};
		}
	}
}