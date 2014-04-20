using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ChestContents
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerChestContents = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (Main.hideUI) return;
			if (LayerChestContents == null)
			{
				LayerChestContents = new ILChestContents(modBase.modName);
				ILChestContents.itemBack = modBase.textures["Images/ItemBackground.png"];
			}
			SBase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerMiniHPBars; }, LayerChestContents);
		}
	}
}