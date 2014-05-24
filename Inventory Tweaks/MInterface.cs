using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using TAPI;
using Terraria;

namespace Shockah.InvTweaks
{
	public class MInterface : ModInterface
	{
		public MInterface(ModBase modBase) : base(modBase) { }

		public override bool PreItemSlotLeftClick(Interface.ItemSlot slot, ref bool release)
		{
			if (slot.modBase == null && Main.localPlayer.chestObj != null && release && Main.keyState.IsKeyDown(Keys.LeftShift))
			{
				if (slot.type == "Inventory")
				{
					Item myItem = slot.MyItem;
					if (SBase.PutItem(ref myItem, Main.localPlayer.chestItems))
					{
						Main.PlaySound(7, -1, -1, 1);
						slot.MyItem = myItem;
					}
				}
				else if (slot.type == "Chest")
				{
					Item myItem = slot.MyItem;
					if (SBase.PutItem(ref myItem, Main.localPlayer.inventory))
					{
						Main.PlaySound(7, -1, -1, 1);
						slot.MyItem = myItem;
					}
				}
				return false;
			}
			return true;
		}
	}
}