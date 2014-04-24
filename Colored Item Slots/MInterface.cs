using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;
using Terraria;

namespace Shockah.ColoredIS
{
	public class MInterface : ModInterface
	{
		private Texture2D texGlow = null, texCracked = null;
		
		public MInterface(ModBase modBase) : base(modBase) { }

		public override void PostDrawItemSlotBackground(SpriteBatch sb, Interface.ItemSlot slot)
		{
			if (!slot.MyItem.IsBlank())
			{
				if (texGlow == null)
				{
					texGlow = modBase.textures["ItemSlotGlow.png"];
					texCracked = modBase.textures["ItemSlotCracked.png"];
				}
				Texture2D tex = slot.MyItem.rare >= 0 ? texGlow : texCracked;
				Color c = slot.MyItem.GetRarityColor();
				if (slot.MyItem.rare == 0) c = Color.LightGray;
				if (slot.MyItem.rare < 0) c = Color.Gray;
				sb.Draw(tex, slot.pos, null, c * slot.alpha, 0f, default(Vector2), slot.scale, SpriteEffects.None, 0f);
			}
		}
	}
}