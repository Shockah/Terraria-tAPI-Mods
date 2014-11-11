using Shockah.Base;
using Shockah.FCM;
using Shockah.ItemSuffixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Bridge.ItemSuffixes_FCM
{
	public class MBase : ModBase
	{
		public override void OnAllModsLoaded()
		{
			InterfaceFCMSuffixes.Reset();
			
			FrameFCMButtons.EventCreatingButtonList += (list) =>
			{
				list.Add(new LittleButton("Suffixes", textures["FCMModuleSuffixes"], () => { return UICore.currentInterface is InterfaceFCMSuffixes; }, () => { InterfaceFCMSuffixes.me.Open(); }, -2.5f));
			};

			SBase.EventMenuStateChange += (menu) =>
			{
				if (!menu)
				{
					new InterfaceFCMSuffixes();
				}
			};
		}
	}
}