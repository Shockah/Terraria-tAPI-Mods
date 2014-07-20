using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class ElButton : El
	{
		public Vector2 pos = default(Vector2), size = default(Vector2);
		public readonly Action<ElButton, int> Clicked;
		public readonly Action<ElButton, SpriteBatch, int> DrawCode;
		public bool dragging = false;

		public ElButton(Action<ElButton, int> Clicked, Action<ElButton, SpriteBatch, int> DrawCode)
		{
			this.Clicked = Clicked;
			this.DrawCode = DrawCode;
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			int block = -1;
			if (update)
			{
				if (new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y).Contains(Main.mouse))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft)
					{
						if (Main.mouseLeftRelease) Clicked(this, 0);
						block = 0;
					}
					if (Main.mouseRight)
					{
						if (Main.mouseRightRelease) Clicked(this, 1);
						block = 1;
					}
				}
			}
			
			if (draw)
			{
				if (new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y).Contains(Main.mouse))
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeft) block = 0;
					if (Main.mouseRight) block = 1;
				}
				Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y);
				DrawCode(this, sb, block);
			}
			
			return block != -1;
		}
	}
}