using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class ElSlider : El
	{
		public Vector2 pos = default(Vector2), size = default(Vector2);
		public readonly Func<int> GetScroll, GetHOne, GetHTotal;
		public readonly Action<int> SetScroll;
		public bool dragging = false;

		public int Scroll
		{
			get { return GetScroll(); }
			set { SetScroll(value); }
		}
		public int HOne
		{
			get { return GetHOne(); }
		}
		public int HTotal
		{
			get { return GetHTotal(); }
		}

		public ElSlider(Action<int> SetScroll, Func<int> GetScroll, Func<int> GetHOne, Func<int> GetHTotal)
		{
			this.SetScroll = SetScroll;
			this.GetScroll = GetScroll;
			this.GetHOne = GetHOne;
			this.GetHTotal = GetHTotal;
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			Texture2D blip = Main.colorBlipTexture;
			int ewidth = 0, eheight = 0;
			float alpha = 200f / 255f;

			if (draw)
			{
				ewidth = -4; eheight = 0;
				sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight, (int)size.X + ewidth * 2, (int)size.Y + eheight * 2), Color.Black * alpha);
				ewidth = -2; eheight = -2;
				sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight, (int)size.X + ewidth * 2, (int)size.Y + eheight * 2), Color.Black * alpha);
				ewidth = 0; eheight = -4;
				sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight, (int)size.X + ewidth * 2, (int)size.Y + eheight * 2), Color.Black * alpha);
				ewidth = -2; eheight = -4;
				sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight, (int)size.X + ewidth * 2, (int)size.Y + eheight * 2), new Color(63, 65, 151) * alpha);
				ewidth = -4; eheight = -2;
				sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight, (int)size.X + ewidth * 2, (int)size.Y + eheight * 2), new Color(63, 65, 151) * alpha);
			}

			int height = (int)(size.Y - 8f), posy = 0;
			if (HTotal > HOne)
			{
				height = (int)Math.Ceiling(1f * HOne / HTotal * (size.Y - 8f));
				if (update)
				{
					if (!dragging)
					{
						if (new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y).Contains(Main.mouse))
						{
							Main.localPlayer.mouseInterface = true;
							if (Main.mouseLeft) dragging = true;
						}
					}
					else
					{
						float newDragF = Math.Min(Math.Max((Main.mouseY - (pos.Y + 4f + height / 2)) / (size.Y - 8f), 0f), 1f);
						Scroll = (int)(newDragF * HTotal);

						if (Main.mouseLeftRelease) dragging = false;
						else Main.localPlayer.mouseInterface = true;
					}
				}
				posy = (int)(1f * Scroll / HTotal * (size.Y - 8f));
			}
			if (draw)
			{
				ewidth = -6; eheight = -4;
				sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight + posy, (int)size.X + ewidth * 2, height), Color.LightSteelBlue * alpha);
				if (height > 4)
				{
					ewidth = -4; eheight = -6;
					sb.Draw(blip, new Rectangle((int)pos.X - ewidth, (int)pos.Y - eheight + posy, (int)size.X + ewidth * 2, height - 4), Color.LightSteelBlue * alpha);
				}
			}
			return dragging;
		}
	}
}