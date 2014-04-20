using Shockah.Base;
using TAPI;

namespace Shockah.ItemSuffixes
{
	public class MBase : ModBase
	{
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