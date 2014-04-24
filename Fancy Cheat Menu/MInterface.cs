using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class MInterface : ModInterface
	{
		public static InterfaceLayer LayerFCMButtons = null;

		public MInterface(ModBase modBase) : base(modBase) { }
		
		public override void ModifyInterfaceLayerList(List<InterfaceLayer> list)
		{
			if (Main.hideUI) return;
			if (LayerFCMButtons == null) LayerFCMButtons = new ILFCMButtons();
			SBase.InsertAfter(list, (il) => { return il == InterfaceLayer.LayerInventory; }, LayerFCMButtons);

			foreach (InterfaceLayer il in list) il.visible = true;
			if (Interface.current is InterfaceFCMNPCs && InterfaceFCMNPCs.spawning != null)
			{
				foreach (InterfaceLayer il in list)
				{
					if (il != InterfaceLayer.LayerCursor && il != LayerFCMButtons && il != InterfaceLayer.LayerCurrentInterface) il.visible = false;
				}
			}
		}

		public override bool OverrideChat()
		{
			return Interface.current is InterfaceFCMBase && ((InterfaceFCMBase)Interface.current).typing == null;
		}
		public override bool KeyboardInputFocused()
		{
			return Interface.current is InterfaceFCMBase && ((InterfaceFCMBase)Interface.current).typing != null;
		}

		public override bool PreDrawCrafting(SpriteBatch sb)
		{
			if (Interface.current is InterfaceFCMBase) return false;
			return true;
		}
	}
}