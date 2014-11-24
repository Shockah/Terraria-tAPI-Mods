using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using TAPI;
using TAPI.UIKit;
using Terraria;

namespace Shockah.InvTweaks
{
	public class MInterface : ModInterface
	{
		private Texture2D texGlow = null, texCracked = null, texQuest = null;

		public override void PostDrawItemSlotBackground(SpriteBatch sb, ItemSlot slot)
		{
			if ((bool)modBase.options["colorRarity"].Value)
			{
				if (!slot.MyItem.IsBlank())
				{
					if (texGlow == null)
					{
						texGlow = modBase.textures["Images/ItemSlotGlow"];
						texCracked = modBase.textures["Images/ItemSlotCracked"];
						texQuest = modBase.textures["Images/ItemSlotQuest"];
					}
					Texture2D tex = slot.MyItem.questItem ? texQuest : (slot.MyItem.rare >= 0 ? texGlow : texCracked);
					Color c = slot.MyItem.GetRarityColor();
					if (slot.MyItem.rare == 0) c = Color.LightGray;
					if (slot.MyItem.rare < 0) c = Color.Gray;
					if (slot.MyItem.questItem) c = Color.White;
					sb.Draw(tex, slot.pos, null, c * slot.alpha, 0f, default(Vector2), slot.scale, SpriteEffects.None, 0f);
				}
			}
		}

		public override bool PreItemSlotLeftClick(ItemSlot slot, ref bool release)
		{
			if (!(bool)modBase.options["bindShiftMove"].Value) return true;
			if (slot.modBase == null && Main.localPlayer.chestObj != null && release && KState.Special.Shift.Down())
			{
				if (slot.type == "Inventory" || slot.type == "Coin" || slot.type == "Ammo")
				{
					Item myItem = slot.MyItem;
					SBase.PutItem(ref myItem, Main.localPlayer.chestItems);
					Main.PlaySound(7, -1, -1, 1);
					slot.MyItem = myItem;
				}
				else if (slot.type == "Chest")
				{
					Item myItem = slot.MyItem;
					if (Main.localPlayer.chest >= 0)
					{
						if (!myItem.IsBlank() && myItem.type >= 71 && myItem.type <= 74) SBase.PutItem(ref myItem, Main.localPlayer.inventory, 50, 53);
						if (!myItem.IsBlank() && myItem.ammo > 0 && !myItem.notAmmo) SBase.PutItem(ref myItem, Main.localPlayer.inventory, 54, 57);
						if (!myItem.IsBlank()) SBase.PutItem(ref myItem, Main.localPlayer.inventory, 0, 49);
					}
					else
					{
						Item[] container = null;
						switch (Main.localPlayer.chest)
						{
							case -2: container = Main.localPlayer.bank.item; break;
							case -3: container = Main.localPlayer.bank2.item; break;
						}
						if (container != null)
							SBase.PutItem(ref myItem, container);
					}
					Main.PlaySound(7, -1, -1, 1);
					slot.MyItem = myItem;
				}
				return false;
			}
			return true;
		}
	}
}