using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class PrefixSlot : El
	{
		public Vector2 pos = new Vector2(), size = new Vector2();
		public float scale = .85f, alpha = 1f;
		public readonly InterfaceFCMPrefixes gui;
		public readonly int index;
		public Prefix prefix = null;

		public PrefixSlot(InterfaceFCMPrefixes gui, int index, Vector2 size)
		{
			this.gui = gui;
			this.index = index;
			this.size = size;
		}

		public Prefix MyPrefix
		{
			get
			{
				return index < gui.filtered.Count ? gui.filtered[index] : null;
			}
			set
			{
				gui.filtered[index] = value;
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

				if (!gui.slotItem.MyItem.IsBlank())
				{
					BinBuffer bb = new BinBuffer();
					gui.slotItem.MyItem.Write(bb);
					bb.Pos = 0;
					Item tempItem = new Item();
					tempItem.Read(bb);

					if (tempItem.PreReforge())
					{
						if (MyPrefix != null && MyPrefix.type > 0)
						{
							tempItem.Prefix(MyPrefix.name);
						}
						else
						{
							tempItem.prefix = Prefix.None;
							tempItem.ResetStats();
						}
					}
					tempItem.PostReforge();

					Main.hoverItemName = tempItem.displayName;
					if (tempItem.stack > 1) Main.hoverItemName += " (" + tempItem.stack + ")";
					Main.toolTip = tempItem;
				}
			}
		}
		public virtual bool IsMouseOnSlot()
		{
			return Main.mouseX > pos.X && Main.mouseX < pos.X + size.X * scale && Main.mouseY > pos.Y && Main.mouseY < pos.Y + size.Y * scale;
		}

		public virtual void OnLeftClick(ref bool release)
		{
			if (MyPrefix == null || gui.slotItem.MyItem.IsBlank()) return;
			if (release)
			{
				if (gui.slotItem.MyItem.PreReforge())
				{
					if (MyPrefix == null || MyPrefix.type == 0)
					{
						gui.slotItem.MyItem.prefix = Prefix.None;
						gui.slotItem.MyItem.ResetStats();
					}
					else
					{
						gui.slotItem.MyItem.Prefix(MyPrefix.name);
					}
					Main.PlaySound(2, -1, -1, 37);
				}
				gui.slotItem.MyItem.PostReforge();
			}
		}
		public virtual void OnRightClick(ref bool release)
		{

		}

		public Prefix ShouldHold()
		{
			if (index >= 0 && index < gui.filtered.Count) return gui.filtered[index];
			return null;
		}

		public virtual void Draw(SpriteBatch sb)
		{
			if (!IsActive()) return;
			DrawSlotBackground(sb);
			DrawSlotPrefix(sb);
		}
		public virtual void DrawSlotBackground(SpriteBatch sb)
		{
			if (MyPrefix != null && IsPrefixActive()) Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y, Color.White);
			else Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y);
		}
		public virtual void DrawSlotPrefix(SpriteBatch sb)
		{
			if (MyPrefix == null) return;
			SDrawing.StringShadowed(sb, Main.fontMouseText, InterfaceFCMPrefixes.defsNames[InterfaceFCMPrefixes.defs.IndexOf(MyPrefix)], pos + new Vector2(6, 6), Item.GetRarityColor(MyPrefix.tier), .75f);
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}

		public bool IsPrefixActive()
		{
			if (gui.slotItem.MyItem.IsBlank()) return false;
			if ((MyPrefix == null || MyPrefix.type == 0) && (gui.slotItem.MyItem.prefix == null || gui.slotItem.MyItem.prefix.type == 0)) return true;
			return gui.slotItem.MyItem.prefix == MyPrefix;
		}
	}
}
