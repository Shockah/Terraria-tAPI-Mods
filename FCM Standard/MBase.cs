using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using TAPI;
using Terraria;
using Terraria.DataStructures;

namespace Shockah.FCM.Standard
{
	public class MBase : ModBase
	{
		public const int
			MSG_SPAWN_NPCS = 1,
			MSG_CHEAT = 2,
			MSG_TIME = 3,
			MSG_SNAPSHOT = 4;
		
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
			InterfaceFCMBuffs.Reset();
			InterfaceFCMMisc.Reset();
			InterfaceFCMSnapshot.Reset();

			FrameFCMButtons.EventCreatingButtonList += (list) =>
			{
				list.Add(new LittleButton("Items", textures["Images/ModuleItems"], () => UICore.currentInterface is InterfaceFCMItems, () => InterfaceFCMItems.me.Open(), 0f));
				list.Add(new LittleButton("NPCs", textures["Images/ModuleNPCs"], () => UICore.currentInterface is InterfaceFCMNPCs, () => InterfaceFCMNPCs.me.Open(), -1f));
				list.Add(new LittleButton("Prefixes", textures["Images/ModulePrefixes"], () => UICore.currentInterface is InterfaceFCMPrefixes, () => InterfaceFCMPrefixes.me.Open(), -2f));
				list.Add(new LittleButton("Buffs", textures["Images/ModuleBuffs"], () => UICore.currentInterface is InterfaceFCMBuffs, () => InterfaceFCMBuffs.me.Open(), -3f));
				list.Add(new LittleButton("Misc", textures["Images/ModuleMisc"], () => UICore.currentInterface is InterfaceFCMMisc, () => InterfaceFCMMisc.me.Open(), -4f));
				list.Add(new LittleButton("Snapshots", textures["Images/ModuleSnapshot"], () => UICore.currentInterface is InterfaceFCMSnapshot, () => InterfaceFCMSnapshot.me.Open(), -5f));
			};

			SBase.EventMenuStateChange += (menu) =>
			{
				if (!menu)
				{
					new InterfaceFCMItems();
					new InterfaceFCMNPCs();
					new InterfaceFCMPrefixes();
					new InterfaceFCMBuffs();
					new InterfaceFCMMisc();
					new InterfaceFCMSnapshot();
				}
			};
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (!Main.gameMenu)
			{
				MPlayer mp = (MPlayer)Main.localPlayer.GetSubClass<MPlayer>();
				if (mp.lastCameraPos.X != -1 || mp.lastCameraPos.Y != -1)
					Main.screenPosition = mp.lastCameraPos;

				if (InterfaceFCMMisc.fullBright)
					Lighting.fullBright = true;
			}
		}
	}
}