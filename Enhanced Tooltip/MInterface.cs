using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.ETooltip
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerPreTooltip = null;
		
		public MInterface(ModBase modBase) : base(modBase) { }

		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (LayerPreTooltip == null) LayerPreTooltip = new ILPreTooltip();
			list.Insert(list.IndexOf(InterfaceLayer.LayerMouseText), LayerPreTooltip);
			LayerPreTooltip.visible = true;
			InterfaceLayer.LayerMouseText.visible = false;
		}
	}
}