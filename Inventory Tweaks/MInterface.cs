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
		private Texture2D texGlow = null, texCracked = null;
		
		public MInterface(ModBase modBase) : base(modBase) { }

		public override void PostDrawItemSlotBackground(SpriteBatch sb, Interface.ItemSlot slot)
		{
			if ((bool)modBase.options["colorRarity"].Value)
			{
				if (!slot.MyItem.IsBlank())
				{
					if (texGlow == null)
					{
						texGlow = modBase.textures["Images/ItemSlotGlow.png"];
						texCracked = modBase.textures["Images/ItemSlotCracked.png"];
					}
					Texture2D tex = slot.MyItem.rare >= 0 ? texGlow : texCracked;
					Color c = slot.MyItem.GetRarityColor();
					if (slot.MyItem.rare == 0) c = Color.LightGray;
					if (slot.MyItem.rare < 0) c = Color.Gray;
					sb.Draw(tex, slot.pos, null, c * slot.alpha, 0f, default(Vector2), slot.scale, SpriteEffects.None, 0f);
				}
			}
		}

		public override bool PreItemSlotLeftClick(Interface.ItemSlot slot, ref bool release)
		{
			if (!(bool)modBase.options["bindShiftMove"].Value) return true;
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