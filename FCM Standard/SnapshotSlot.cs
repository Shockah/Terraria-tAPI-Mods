using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class SnapshotSlot : El
	{
		public Vector2 pos = new Vector2(), size = new Vector2();
		public float scale = 1f, alpha = 1f;
		public readonly InterfaceFCMSnapshot gui;
		public readonly int index;
		public NPC npc = new NPC();

		public SnapshotSlot(InterfaceFCMSnapshot gui, int index)
		{
			this.gui = gui;
			this.index = index;
		}

		public PlayerSnapshot MySnapshot
		{
			get
			{
				return index < gui.snapshots.Count ? gui.snapshots[index] : null;
			}
		}

		public virtual bool IsActive()
		{
			return true;
		}

		public void UpdateAndDraw(SpriteBatch sb)
		{
			Update();
			Draw(sb);
		}
		public void UpdateAndDraw(SpriteBatch sb, Vector2 pos)
		{
			Update(pos);
			Draw(sb);
		}
		public void UpdateAndDraw(SpriteBatch sb, Vector2 pos, float scale)
		{
			Update(pos, scale);
			Draw(sb);
		}

		public virtual void UpdatePos(Vector2 pos)
		{
			this.pos = pos;
		}
		public void Update(Vector2 pos)
		{
			UpdatePos(pos);
			Update();
		}
		public void Update(float scale)
		{
			this.scale = scale;
			Update();
		}
		public void Update(Vector2 pos, float scale)
		{
			this.scale = scale;
			UpdatePos(pos);
			Update();
		}
		public virtual void Update()
		{
			if (!IsActive()) return;
			if (IsMouseOnSlot())
			{
				Main.localPlayer.mouseInterface = true;
				if (Main.mouseLeft) OnLeftClick(ref Main.mouseLeftRelease);
				else if (Main.mouseRight) OnRightClick(ref Main.mouseRightRelease);
			}
		}
		public virtual bool IsMouseOnSlot()
		{
			return Main.mouseX >= pos.X && Main.mouseX < pos.X + size.X * scale - 1 && Main.mouseY >= pos.Y && Main.mouseY < pos.Y + size.Y * scale - 1;
		}

		public virtual void OnLeftClick(ref bool release)
		{
			if (InterfaceFCMBase.lockSlotInteraction) return;
			if (MySnapshot == null) return;
			gui.selected = index;
		}
		public virtual void OnRightClick(ref bool release)
		{
			if (InterfaceFCMBase.lockSlotInteraction) return;
			if (MySnapshot == null) return;
			gui.selected = -1;
		}

		public virtual void Draw(SpriteBatch sb)
		{
			if (!IsActive()) return;
			DrawSlotBackground(sb);
			DrawSlotSnapshot(sb);
		}
		public virtual void DrawSlotBackground(SpriteBatch sb)
		{
			if (gui.selected == index)
				Drawing.DrawBox(sb, pos.X, pos.Y, size.X * scale, size.Y * scale, Color.White * .785f);
			else
				Drawing.DrawBox(sb, pos.X, pos.Y, size.X * scale, size.Y * scale, .785f);
		}
		public virtual void DrawSlotSnapshot(SpriteBatch sb)
		{
			PlayerSnapshot snapshot = MySnapshot;
			if (snapshot != null)
			{
				snapshot.Draw(pos + new Vector2(size.Y * scale / 2, size.Y * scale / 2) - snapshot.player.Size / 2);
				Vector2 measure = Main.fontMouseText.MeasureString(snapshot.name);
				Drawing.StringShadowed(sb, Main.fontMouseText, snapshot.name, new Vector2(pos.X + (size.Y + 4) * scale, pos.Y + (size.Y / 3 + 4) * scale), Color.White, scale, new Vector2(0, measure.Y / 2));

				string datetxt = string.Format("{0:yyyy-MM-dd HH:mm}", snapshot.date);
				measure = Main.fontMouseText.MeasureString(datetxt);
				Drawing.StringShadowed(sb, Main.fontMouseText, datetxt, new Vector2(pos.X + (size.Y + 4) * scale, pos.Y + (size.Y / 3 * 2 + 4) * scale), Color.White, .8f * scale, new Vector2(0, measure.Y / 2));

				List<Item> items = new List<Item>();
				for (int i = 0; i < 10; i++)
				{
					if (!snapshot.player.inventory[i].IsBlank())
						items.Add(snapshot.player.inventory[i]);
				}
				for (int i = 0; i < items.Count; i++)
					DrawItem(sb, items[i], new Vector2(pos.X + (size.X - 14 - (items.Count - i - 1) * 24) * scale, pos.Y + (size.Y / 3 - 3) * scale), scale * .75f);

				items.Clear();
				for (int i = 0; i < 8; i++)
				{
					if (!snapshot.player.armor[i].IsBlank())
						items.Add(snapshot.player.armor[i]);
				}
				for (int i = 0; i < items.Count; i++)
					DrawItem(sb, items[i], new Vector2(pos.X + (size.X - 14 - (items.Count - i - 1) * 24) * scale, pos.Y + (size.Y / 3 * 2 + 3) * scale), scale * .75f);
			}
		}

		protected void DrawItem(SpriteBatch sb, Item item, Vector2 pos, float scale)
		{
			pos.X -= 26 * scale;
			pos.Y -= 26 * scale;
			float iscale = 1f;
			Texture2D texItem = item.GetTexture();
			if (texItem.Width > 32 || texItem.Height > 32) iscale = texItem.Width > texItem.Height ? 32f / texItem.Width : 32f / texItem.Height;
			iscale *= scale;

			Color cTexItem = item.GetTextureColor();
			sb.Draw(
				texItem,
				new Vector2(pos.X + 26f * scale - texItem.Width * 0.5f * iscale, pos.Y + 26f * scale - texItem.Height * 0.5f * iscale),
				null, item.GetAlpha(cTexItem) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
			);

			cTexItem = Color.White;
			if (item.color != default(Color))
			{
				sb.Draw(
					texItem,
					new Vector2(pos.X + 26f * scale - texItem.Width * 0.5f * iscale, pos.Y + 26f * scale - texItem.Height * 0.5f * iscale),
					null, item.GetColor(Color.White) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
				);
			}

			if (item.stack > 1)
			{
				sb.DrawString(Main.fontItemStack, "" + item.stack, new Vector2(pos.X + 10f * scale, pos.Y + 26f * scale), Color.White * alpha, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
			}
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}
	}
}
