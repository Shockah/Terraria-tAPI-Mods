using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerTooltip = null, LayerFrames = null, LayerPopupMenus = null;
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (LayerTooltip == null)
			{
				LayerTooltip = new ILTooltip();
				LayerFrames = new ILFrames();
				LayerPopupMenus = new IlPopupMenus();
			}
			SBase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerMouseText; }, LayerTooltip);
			SBase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerCurrentInterface; }, LayerPopupMenus);
			SBase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerCurrentInterface; }, LayerFrames);

			LayerTooltip.visible = LayerPopupMenus.visible = LayerFrames.visible = !Main.hideUI;
		}
	}
}