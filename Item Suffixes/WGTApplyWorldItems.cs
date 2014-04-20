using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class WGTApplyWorldItems : WorldGenTask
	{
		public WGTApplyWorldItems(ModBase modBase) : base(modBase.modName) { }
		
		public override void Generate()
		{
			Main.statusText = "Applying suffixes to items...";

			for (int i = 0; i < Main.chest.Length; i++)
			{
				Chest c = Main.chest[i];
				if (c == null) continue;
				if (c.x <= 0 || c.y <= 0) continue;

				for (int j = 0; j < Chest.maxItems; j++)
				{
					Item item = c.item[j];
					if (!item.IsBlank())
					{
						MItem mitem = item.GetSubClass<MItem>();
						if (mitem != null)
						{
							if (!mitem.CanGetSuffixes()) continue;
							mitem.SetRandomSuffix();
						}
					}
				}
			}
		}
	}
}