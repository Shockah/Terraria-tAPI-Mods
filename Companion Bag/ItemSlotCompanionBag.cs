using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class ItemSlotCompanionBag : Interface.ItemSlot
	{
		public ItemSlotCompanionBag(ModBase modBase, int index) : base(modBase, "Slot", index, (slot, item) => { Main.localPlayer.GetSubClass<MPlayer>().companions[slot.index] = item; }, (slot) => { return Main.localPlayer.GetSubClass<MPlayer>().companions[slot.index]; }) { }

		public override bool AllowsItem(Item item)
		{
			if (Main.mouseItem.IsBlank()) return true;
			if (item.maxStack > 1 || item.damage > 0) return false;
			foreach (Item item2 in Main.localPlayer.GetSubClass<MPlayer>().companions) if (item2.type == item.type) return false;
			return (item.shoot > 0 && Main.projPet[item.shoot]) || (item.buffType > 0 && (Main.vanityPet[item.buffType] || Main.lightPet[item.buffType]));
		}

		public override bool IsMouseOnItemSlot()
		{
			if (FrameCompanionBagInterface.me.dragging.HasValue) return false;
			return base.IsMouseOnItemSlot();
		}

		public override void OnLeftClick(ref bool release)
		{
			bool wasBlank = MyItem.IsBlank();
			base.OnLeftClick(ref release);
			if (wasBlank != MyItem.IsBlank())
			{
				InterfaceCompanionBag.needUpdate = true;

				if (Main.mouseItem.IsBlank())
				{
					List<Item> companions = Main.localPlayer.GetSubClass<MPlayer>().companions;
					for (int i = 0; i < companions.Count; i++) if (companions[i].IsBlank()) companions.RemoveAt(i);
					companions.Add(new Item());
				}
			}
		}

		public override void OnRightClick(ref bool release)
		{
			if (release)
			{
				
			}
		}
	}
}
