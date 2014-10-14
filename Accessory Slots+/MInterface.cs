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
			if (layer == null)
			{
				layer = new ILSlots(modBase);
			}
			layer.visible = true;
			InterfaceLayer.Add(list, layer, InterfaceLayer.LayerInventory, false);
		}
	}
}