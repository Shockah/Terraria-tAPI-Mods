using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using TAPI.UIKit;

namespace Shockah.AccSlots
{
	public class MInterface : ModInterface
	{
		public static ILSlots layer = null;
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			for (int i = MBase.ACC_SLOT_1; i <= MBase.ACC_SLOT_5; i++) ItemSlot.equip[i].active = false;
			for (int i = MBase.ACC_SOCIAL_1; i <= MBase.ACC_SOCIAL_5; i++) ItemSlot.equip[i].active = false;
			for (int i = MBase.ACC_DYE_1; i <= MBase.ACC_DYE_5; i++) ItemSlot.dye[i].active = false;
			
			if (layer == null)
			{
				layer = new ILSlots(modBase);
			}
			layer.visible = true;
			InterfaceLayer.Add(list, layer, InterfaceLayer.LayerInventory, false);
		}
	}
}