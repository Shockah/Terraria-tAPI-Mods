using Shockah.Base;
using TAPI;
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
	}
}