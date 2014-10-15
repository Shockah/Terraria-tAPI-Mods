using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class MInterface : ModInterface
	{
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			InterfaceFCMItems.displayIds = false;
			if (UICore.currentInterface is InterfaceFCMNPCs && InterfaceFCMNPCs.spawning != null)
			{
				foreach (InterfaceLayer il in list)
				{
					if (il != InterfaceLayer.LayerCursor && il != InterfaceLayer.LayerCurrentInterface) il.visible = false;
				}
			}
		}
	}
}