using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class MBase : ModBase
	{
		public static ModBase me { get; private set; }
		
		public override void OnLoad()
		{
			me = this;
		}

		public override void OnAllModsLoaded()
		{
			InterfaceFCMItems.Reset();
			InterfaceFCMNPCs.Reset();
			InterfaceFCMPrefixes.Reset();

			new InterfaceFCMItems();
			new InterfaceFCMNPCs();
			new InterfaceFCMPrefixes();

			FrameFCMButtons.EventCreatingButtonList += (list) =>
			{
				list.Add(new LittleButton("Items", textures["Images/ModuleItems.png"], () => Interface.current is InterfaceFCMItems, () => InterfaceFCMItems.me.Open(), 0f));
				list.Add(new LittleButton("NPCs", textures["Images/ModuleNPCs.png"], () => Interface.current is InterfaceFCMNPCs, () => InterfaceFCMNPCs.me.Open(), -1f));
				list.Add(new LittleButton("Prefixes", textures["Images/ModulePrefixes.png"], () => Interface.current is InterfaceFCMPrefixes, () => InterfaceFCMPrefixes.me.Open(), -2f));
			};
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