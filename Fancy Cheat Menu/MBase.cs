using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using TAPI;
using Terraria;
namespace Shockah.FCM
{
	public class MBase : APIModBase
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
			InterfaceFCMPrefixes.Reset();
			SBase.EventSTooltipDraw += () => { return tip; };
			SBase.EventMenuStateChange += (menu) => { if (!menu) new FrameFCMButtons().Create(); };
		}

		public override void OnReload()
		{
			FrameFCMButtons.Clear();
			InterfaceFCMItems.me = null;
			InterfaceFCMNPCs.me = null;
			InterfaceFCMPrefixes.me = null;
		}

		public override void PostGameDraw(SpriteBatch sb)
		{
			if (!(Interface.current is InterfaceFCMNPCs) && InterfaceFCMNPCs.spawning != null)
			{
				InterfaceFCMBase.resetInterface = false;
				InterfaceFCMNPCs.spawning = null;
				InterfaceFCMNPCs.me.Open();
			}
		}
	}
}