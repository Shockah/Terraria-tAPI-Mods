using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ChestContents
{
	public class ILChestContents : InterfaceLayer
	{
		public const int TILE_CHEST = 21;

		public static Texture2D itemBack;

		public static void DrawItem(SpriteBatch sb, Vector2 pos, Item item, float alpha, float maxSize = 32)
		{
			sb.Draw(itemBack, pos, null, Color.White * alpha, 0f, new Vector2(itemBack.Width, itemBack.Height) / 2, 1f, SpriteEffects.None, 0f);

			float iscale = 1f;
			Texture2D texItem = item.GetTexture();
			if (texItem.Width > 32 || texItem.Height > 32) iscale = texItem.Width > texItem.Height ? 32f / texItem.Width : 32f / texItem.Height;
			iscale *= .75f;
			Vector2 origin = new Vector2(texItem.Width, texItem.Height) / 2;

			sb.Draw(texItem, pos, null, item.GetAlpha(item.GetTextureColor()) * alpha, 0f, origin, iscale, SpriteEffects.None, 0f);
			if (item.color != default(Color)) sb.Draw(texItem, pos, null, item.GetColor(Color.White) * alpha, 0f, origin, iscale, SpriteEffects.None, 0f);
		}

		public int cX = -1, cY = -1, cId = -1, life = 0;

		public ILChestContents(string name) : base(name) { }

		protected override void OnDraw(SpriteBatch sb)
		{
			if (Main.netMode != 0)
			{
				life = 0;
				cX = cY = cId = -1;
				return;
			}

			Vector2 cpos = new Vector2(Main.mouseX, Main.mouseY);
			Vector2 mouseWorld = Main.screenPosition + cpos;
			int mtileX = (int)(mouseWorld.X / 16), mtileY = (int)(mouseWorld.Y / 16);

			if (Main.playerInventory)
			{
				Update(-1, -1, -1);
			}
			else
			{
				Tile t = Main.tile[mtileX, mtileY];
				if (t.active() && t.type == TILE_CHEST)
				{
					for (int i = 0; i < Main.chest.Length; i++)
					{
						Chest c = Main.chest[i];
						if (c == null) continue;
						if ((mtileX == c.x || mtileX == c.x+1) && (mtileY == c.y || mtileY == c.y+1) && Main.localPlayer.GetSubClass<MPlayer>().IsVisited(c))
						{
							Update(c.x, c.y, i);
							break;
						}
					}
				}
				else Update(-1, -1, -1);
			}

			if (life > 0)
			{
				Chest c = Main.chest[cId];
				if (c == null)
				{
					life = 0;
					cX = cY = cId = -1;
					return;
				}

				List<string> itemNames = new List<string>();
				List<Item> items = new List<Item>();

				for (int i = 0; i < c.item.Length; i++)
				{
					Item item = c.item[i];
					if (item.IsBlank()) continue;

					if (!itemNames.Contains(item.name))
					{
						itemNames.Add(item.name);
						items.Add(item);
					}
				}
				if (items.Count == 0)
				{
					life = 0;
					cX = cY = cId = -1;
					return;
				}

				DisplayStyle.TwoCircles.Draw(sb, this, items);
			}
		}

		protected void Update(int ncX, int ncY, int ncId)
		{
			if (UpdateMatches(ncX, ncY, ncId) && ncId != -1)
			{
				life++;
			}
			else
			{
				life--;
				if (ncId != -1) life--;
				if (life <= 0)
				{
					life = 0;
					cX = ncX;
					cY = ncY;
					cId = ncId;
				}
			}
		}
		protected bool UpdateMatches(int ncX, int ncY, int ncId)
		{
			return ncX == cX && ncY == cY && ncId == cId;
		}
	}
}
