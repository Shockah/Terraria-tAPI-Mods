using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.MoveAnything
{
	public class MInterface : ModInterface
	{
		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			InterfaceLayer.LayerBars.visible = false;
		}
	}
}