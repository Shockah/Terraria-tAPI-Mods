using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.AccSlots
{
	public class MInterface : ModInterface
	{
		public static ILSlots layer = null;
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (layer == null)
			{
				layer = new ILSlots(modBase);
			}
			layer.visible = true;
			InterfaceLayer.Add(list, layer, InterfaceLayer.LayerInventory, false);
		}

		public override bool PreItemSlotRightClick(ItemSlot slot, ref bool release)
		{
			if (release && slot.modBase == null && (slot.type == "Inventory" || slot.type == "Chest"))
			{
				Item item = slot.MyItem;
				if (item.accessory && item.maxStack == 1 && CustomItemSlotAccessory.CanEquip(item))
				{
					ItemSlot slot2;
					Item item2;

					MPlayer mp = Main.localPlayer.GetSubClass<MPlayer>();
					int currentSlots = mp.currentSlots;
					for (int i = 0; i < currentSlots; i++)
					{
						slot2 = MInterface.layer.slotsItem[i];
						item2 = slot2.MyItem;
						if (item2.IsBlank() && item.CanEquip(Main.localPlayer, slot2))
						{
							slot.MyItem = item2;
							slot2.MyItem = item;
							Main.PlaySound(7, -1, -1, 1);
							Recipe.FindRecipes();
							return false;
						}
					}

					if (MBase.counterSlot >= currentSlots)
					{
						MBase.counterSlot = 0;
					}

					slot2 = MInterface.layer.slotsItem[MBase.counterSlot];
					if (item.CanEquip(Main.localPlayer, slot2))
					{
						item2 = slot2.MyItem;
						slot.MyItem = item2;
						slot2.MyItem = item;
						MBase.counterSlot++;
						Main.PlaySound(7, -1, -1, 1);
						Recipe.FindRecipes();
						return false;
					}
				}
			}
			return true;
		}
	}
}