using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public class MInterface : ModInterface
	{
		public static ILNotifiers LayerNotifiers = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (Main.hideUI) return;
			MBase.sbase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerBars; }, LayerNotifiers);
		}
	}
}