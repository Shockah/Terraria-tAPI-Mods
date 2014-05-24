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
			new InterfaceFCMSuffixes();
			FrameFCMButtons.EventCreatingButtonList += (list) =>
			{
				int lbprefixes = -1;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].name == "Prefixes")
					{
						lbprefixes = i;
						break;
					}
				}

				LittleButton lb2 = new LittleButton("Suffixes", textures["FCMModuleSuffixes.png"], () => { return Interface.current is InterfaceFCMSuffixes; }, () => { InterfaceFCMSuffixes.me.Open(); });
				if (lbprefixes == -1 || lbprefixes == list.Count - 1) list.Add(lb2);
				else list.Insert(lbprefixes + 1, lb2);
			};
		}
	}
}