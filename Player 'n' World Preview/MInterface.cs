using System.Collections.Generic;
using TAPI;

namespace Shockah.PNWPreview
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerPNWThread = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (LayerPNWThread == null) LayerPNWThread = new ILPNWThread();
			list.Add(LayerPNWThread);
		}
	}
}