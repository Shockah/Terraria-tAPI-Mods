using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	[GlobalMod] public class MItem : ModItem
	{
		public static uint autoIncrement = 0;
		
		public static bool CanGetSuffixes(Item item)
		{
			return !item.IsBlank() && (((item.bodySlot != -1 || item.headSlot != -1 || item.legSlot != -1) && !item.vanity) || (item.accessory && item.createTile == -1)) && item.maxStack == 1;
		}

		public uint myId;
		public ItemSuffix suffix = ItemSuffix.list[0];
		public int resetDamage = 0, resetCrit = 0, resetTooltip = 0;
		
		public MItem(ModBase modBase, Item item) : base(modBase, item) { }

		public override void Initialize()
		{
			myId = ++autoIncrement;
		}

		public bool CanGetSuffixes()
		{
			return CanGetSuffixes(item);
		}
		public void SetRandomSuffix()
		{
			suffix = ItemSuffix.RandomAllowedSuffix(item);
		}

		public override void Save(BinBuffer bb)
		{
			if (!CanGetSuffixes()) return;
			bb.Write((byte)suffix.id);
		}
		public override void Load(BinBuffer bb)
		{
			if (!CanGetSuffixes()) return;
			suffix = ItemSuffix.list[bb.ReadByte()];
		}

		public override void OnCraft(Recipe recipe)
		{
			if (!CanGetSuffixes()) return;
			SetRandomSuffix();
		}
		public override bool PreReforge()
		{
			MItem mitem = item.GetSubClass<MItem>();
			if (mitem != null)
			{
				if (mitem.resetDamage != 0)
				{
					item.damage = mitem.resetDamage;
					item.crit = mitem.resetCrit;
				}
				mitem.resetDamage = item.damage;
				mitem.resetCrit = item.crit;
			}
			return true;
		}

		public override string OnAffixName(string currentName, string oldName)
		{
			if (!CanGetSuffixes())
			{
				return currentName;
			}
			else
			{
				if (suffix != null)
				{
					for (int j = 0; j < resetTooltip; j++) item.toolTips.RemoveAt(item.toolTips.Count - 1);
					resetTooltip = 0;
					suffix.AddTooltips(item);
				}
				
				return currentName + " " + suffix.displayName;
			}
		}
	}
}