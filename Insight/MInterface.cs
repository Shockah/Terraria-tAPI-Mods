using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Insight
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerChestContents = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (LayerChestContents == null)
			{
				LayerChestContents = new ILChestContents(modBase);
				ILChestContents.itemBack = modBase.textures["Images/ItemBackground.png"];
			}
			list.Insert(list.IndexOf(InterfaceLayer.LayerMouseText), LayerChestContents);
			LayerChestContents.visible = !Main.hideUI;
		}
	}
}