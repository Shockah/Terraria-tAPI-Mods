using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using Shockah.FCM;
using System;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.ItemSuffixes
{
	public class SuffixSlot : El
	{
		public Vector2 pos = new Vector2(), size = new Vector2();
		public float scale = .85f, alpha = 1f;
		public readonly InterfaceFCMSuffixes gui;
		public readonly int index;
		public ItemSuffix Suffix = null;

		public SuffixSlot(InterfaceFCMSuffixes gui, int index, Vector2 size)
		{
			this.gui = gui;
			this.index = index;
			this.size = size;
		}

		public ItemSuffix MySuffix
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

					MItem mitem = tempItem.GetSubClass<MItem>();
					if (mitem != null)
					{
						if (mitem.resetDamage != 0)
						{
							tempItem.damage = mitem.resetDamage;
							tempItem.crit = mitem.resetCrit;
						}
						mitem.resetDamage = tempItem.damage;
						mitem.resetCrit = tempItem.crit;
						mitem.suffix = MySuffix;
					}

					Main.hoverItemName = tempItem.displayName;
					if (tempItem.stack > 1) Main.hoverItemName += " (" + tempItem.stack + ")";
					Main.toolTip = tempItem;
				}
				else if (MySuffix != null)
				{
					Item tempItem = new Item();
					tempItem.SetDefaults("Vanilla:Wood");
					MySuffix.AddTooltips(tempItem);
					StringBuilder sb = new StringBuilder();
					foreach (string tip in tempItem.toolTips)
					{
						if (sb.Length > 0) sb.Append("\n");
						sb.Append(tip);
					}
					SBase.tip = sb.ToString();
				}
			}
		}
		public virtual bool IsMouseOnSlot()
		{
			return Main.mouseX > pos.X && Main.mouseX < pos.X + size.X * scale && Main.mouseY > pos.Y && Main.mouseY < pos.Y + size.Y * scale;
		}

		public virtual void OnLeftClick(ref bool release)
		{
			if (MySuffix == null || gui.slotItem.MyItem.IsBlank()) return;
			if (release)
			{
				MItem mitem = gui.slotItem.MyItem.GetSubClass<MItem>();
				if (mitem != null)
				{
					if (mitem.resetDamage != 0)
					{
						gui.slotItem.MyItem.damage = mitem.resetDamage;
						gui.slotItem.MyItem.crit = mitem.resetCrit;
					}
					mitem.resetDamage = gui.slotItem.MyItem.damage;
					mitem.resetCrit = gui.slotItem.MyItem.crit;
					mitem.suffix = MySuffix;
					Main.PlaySound(2, -1, -1, 37);
				}
			}
		}
		public virtual void OnRightClick(ref bool release)
		{

		}

		public ItemSuffix ShouldHold()
		{
			if (index >= 0 && index < gui.filtered.Count) return gui.filtered[index];
			return null;
		}

		public virtual void Draw(SpriteBatch sb)
		{
			if (!IsActive()) return;
			DrawSlotBackground(sb);
			DrawSlotSuffix(sb);
		}
		public virtual void DrawSlotBackground(SpriteBatch sb)
		{
			if (IsSuffixActive()) Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y, Color.White);
			else Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y);
		}
		public virtual void DrawSlotSuffix(SpriteBatch sb)
		{
			if (MySuffix == null) return;
			SDrawing.StringShadowed(sb, Main.fontMouseText, InterfaceFCMSuffixes.defsNames[InterfaceFCMSuffixes.defs.IndexOf(MySuffix)], pos + new Vector2(6, 6), Color.White, .75f);
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}

		public bool IsSuffixActive()
		{
			if (gui.slotItem.MyItem.IsBlank()) return false;
			return gui.slotItem.MyItem.GetSubClass<MItem>().suffix == MySuffix;
		}
	}
}
