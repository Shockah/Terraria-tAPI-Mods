using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerTooltip = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (Main.hideUI) return;
			if (LayerTooltip == null) LayerTooltip = new ILTooltip();
			SBase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerMouseText; }, LayerTooltip);
		}
	}
}