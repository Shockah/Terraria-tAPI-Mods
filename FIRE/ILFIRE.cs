using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using TAPI;
using Terraria;

namespace Shockah.FIRE
{
	public class ILFIRE : InterfaceLayer
	{
		public ILFIRE() : base(MWorld.me.modBase.modName) { }

		protected override void OnDraw(SpriteBatch sb)
		{
			Texture2D texLaser = MWorld.me.modBase.textures["Images/FIRELaser.png"], texLaserBase = MWorld.me.modBase.textures["Images/FIREBase.png"];
			float scale = ((FIREInstance.radius * 2 + 1) * 32f) / texLaserBase.Width;

			foreach (FIREInstance inst in MWorld.me.instances)
			{
				if (inst.y < inst.targetY)
				{
					string text = "Impact in " + (inst.targetY - inst.y) / FIREInstance.speedV / 60 + "s";
					Vector2 measure = Main.fontCombatText[0].MeasureString(text);
					Vector2 pos = new Vector2(inst.x * 16 + 8 - Main.screenPosition.X - measure.X / 2, inst.targetY * 16 + 8 - Main.screenPosition.Y - measure.Y / 2);
					Drawing.DrawBox(sb, pos.X - 8, pos.Y - 8, measure.X + 16, measure.Y + 16);
					SDrawing.StringShadowed(sb, Main.fontCombatText[0], text, pos, Color.Red);
				}
			}
			
			sb.End();
			sb.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null);
			foreach (FIREInstance inst in MWorld.me.instances)
			{
				Vector2 pos = new Vector2(inst.x * 16 + 8, (inst.y + 1) * 16) - Main.screenPosition;
				sb.Draw(texLaserBase, new Rectangle((int)pos.X, (int)(pos.Y - texLaserBase.Height), (int)(texLaserBase.Width * scale * inst.alpha), (int)(texLaserBase.Height * scale)), null, Color.White * inst.alpha, 0f, new Vector2(texLaserBase.Width / 2, 0), SpriteEffects.None, 0f);
				int repeat = (int)Math.Ceiling((Main.maxTilesY + FIREInstance.speedV * 16 / FIREInstance.decaySpeed) * 16f / texLaser.Height);
				for (int i = 0; i < repeat; i++)
				{
					sb.Draw(texLaser, new Rectangle((int)pos.X, (int)(pos.Y - texLaserBase.Height - texLaser.Height * (i + 1)), (int)(texLaserBase.Width * scale * inst.alpha), (int)(texLaserBase.Height * scale)), null, Color.White * inst.alpha, 0f, new Vector2(texLaser.Width / 2, 0), SpriteEffects.None, 0f);
				}
			}
			sb.End();
			sb.Begin();
		}
	}
}
