using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class ElChooser<T> : El
	{
		public const float TEXT_SCALE = .75f;
		
		public Vector2 pos = default(Vector2), size = default(Vector2);
		public readonly List<Tuple<string, T>> items = new List<Tuple<string, T>>();
		public readonly Func<T> GetItem;
		public readonly Action<T> SetItem;
		public readonly Func<Texture2D> GetSelectedTexture;
		public bool open = false;

		public T Item
		{
			get { return GetItem(); }
			set { SetItem(value); }
		}

		public ElChooser(Action<T> SetItem, Func<T> GetItem, Func<Texture2D> GetSelectedTexture)
		{
			this.SetItem = SetItem;
			this.GetItem = GetItem;
			this.GetSelectedTexture = GetSelectedTexture;
		}

		public void Add(params Tuple<string, T>[] items)
		{
			this.items.AddRange(items);
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update)
			{
				if (open)
				{
					Main.localPlayer.mouseInterface = true;
					if (Main.mouseLeftRelease)
					{
						if (new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)(size.Y * (items.Count + 1))).Contains(Main.mouse))
						{
							int index = (int)((Main.mouseY - pos.Y) / size.Y) - 1;
							if (index == -1)
							{
								for (int i = 0; i < items.Count; i++)
								{
									if (object.ReferenceEquals(Item, items[i].Item2))
									{
										index = i;
										break;
									}
								}
							}
							Item = items[index].Item2;
						}
						open = false;
					}
				}
				else
				{
					if (new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y).Contains(Main.mouse))
					{
						Main.localPlayer.mouseInterface = true;
						if (Main.mouseLeft) open = true;
					}
				}
			}

			if (draw)
			{
				if (open)
				{
					for (int i = 0; i < items.Count; i++) DrawItem(sb, i + 1, items[i], false);
				}
				for (int i = 0; i < items.Count; i++)
				{
					if (object.ReferenceEquals(Item, items[i].Item2))
					{
						DrawItem(sb, 0, items[i], true);
						break;
					}
				}
			}
			return open;
		}

		protected void DrawItem(SpriteBatch sb, int index, Tuple<string, T> item, bool selected)
		{
			Drawing.DrawBox(sb, pos.X, pos.Y + index * size.Y, size.X, size.Y);

			float wLeft = size.X;
			if (selected)
			{
				Texture2D tex = GetSelectedTexture();
				float tscale = 1f;
				if (tscale * tex.Width > size.Y - 2f) tscale = (size.Y - 4f) / tex.Width;
				if (tscale * tex.Height > size.Y - 2f) tscale = (size.Y - 4f) / tex.Height;
				sb.Draw(tex, pos + new Vector2(size.Y / 2f + 2, size.Y / 2f + index * size.Y), null, Color.White, 0f, tex.Size() / 2, tscale, SpriteEffects.None, 0f);
				wLeft -= size.Y;
			}

			Vector2 measure = Main.fontMouseText.MeasureString(item.Item1) * TEXT_SCALE;
			SDrawing.StringShadowed(sb, Main.fontMouseText, item.Item1, new Vector2(pos.X + size.X - wLeft + (wLeft - measure.X) / 2, pos.Y + index * size.Y + size.Y / 2), Color.White, TEXT_SCALE, new Vector2(0, measure.Y / 2));
		}
	}
}