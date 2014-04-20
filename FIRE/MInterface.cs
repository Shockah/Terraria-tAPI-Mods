using System.Collections.Generic;
using TAPI;

namespace Shockah.FIRE
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerFIRE = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (LayerFIRE == null) LayerFIRE = new ILFIRE();
			list.Insert(0, LayerFIRE);
		}
	}
}