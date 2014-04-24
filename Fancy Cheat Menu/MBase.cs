using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using TAPI;
using Terraria;
namespace Shockah.FCM
{
	public class MBase : ModBase
	{
		public static ModBase me { get; private set; }
		public static STooltip tip = new STooltip();
		
		public override void OnLoad()
		{
			me = this;
		}

		public override void OnAllModsLoaded()
		{
			InterfaceFCMItems.Reset();
			InterfaceFCMNPCs.Reset();
			SBase.EventSTooltipDraw += () => { return tip; };
		}

		public override void PostGameDraw(SpriteBatch sb)
		{
			if (!(Interface.current is InterfaceFCMNPCs) && InterfaceFCMNPCs.spawning != null)
			{
				InterfaceFCMNPCs.spawning = null;
				new InterfaceFCMNPCs().Open();
			}
		}
	}
}