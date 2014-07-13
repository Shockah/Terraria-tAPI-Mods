using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class BuffSlot : El
	{
		public Vector2 pos = new Vector2(), size = new Vector2();
		public float scale = .85f, alpha = 1f;
		public readonly InterfaceFCMBuffs gui;
		public readonly int index;
		public BuffDef def = null;

		public BuffSlot(InterfaceFCMBuffs gui, int index, Vector2 size)
		{
			this.gui = gui;
			this.index = index;
			this.size = size;
		}

		public BuffDef MyBuffDef
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

				if (MyBuffDef != null)
				{
					Shockah.FCM.MBase.tip += new STooltip.Line((Main.keyState.IsKeyDown(Keys.LeftControl) ? "[" + MyBuffDef.type + "] " : "") + MyBuffDef.noModName, Color.White);
					Shockah.FCM.MBase.tip += new STooltip.Line(Main.buffTip[MyBuffDef.type], Color.White);
				}
			}
		}
		public virtual bool IsMouseOnSlot()
		{
			return Main.mouseX > pos.X && Main.mouseX < pos.X + size.X * scale && Main.mouseY > pos.Y && Main.mouseY < pos.Y + size.Y * scale;
		}

		public virtual void OnLeftClick(ref bool release)
		{
			if (MyBuffDef == null) return;
			if (release)
			{
				Main.localPlayer.AddBuff(MyBuffDef.type, (InterfaceFCMBuffs.me.buffM * 60 + InterfaceFCMBuffs.me.buffS) * 60);
				Main.PlaySound(2, -1, -1, 37);
			}
		}
		public virtual void OnRightClick(ref bool release)
		{
			if (MyBuffDef == null) return;
			if (release)
			{
				int slot = Main.localPlayer.HasBuff(MyBuffDef.type);
				if (slot >= 0)
				{
					Main.localPlayer.DelBuff(slot);
					Main.PlaySound(2, -1, -1, 37);
				}
			}
		}

		public BuffDef ShouldHold()
		{
			if (index >= 0 && index < gui.filtered.Count) return gui.filtered[index];
			return null;
		}

		public virtual void Draw(SpriteBatch sb)
		{
			if (!IsActive()) return;
			DrawSlotBackground(sb);
			DrawSlotBuff(sb);
		}
		public virtual void DrawSlotBackground(SpriteBatch sb)
		{
			if (IsBuffActive()) Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y, Color.White);
			else Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y);
		}
		public virtual void DrawSlotBuff(SpriteBatch sb)
		{
			if (MyBuffDef == null) return;

			Texture2D tex = Main.buffTexture[MyBuffDef.type];
			float iscale = 1f;
			if (tex.Width > InterfaceFCMBuffs.SLOT_H - 8 || tex.Height > InterfaceFCMBuffs.SLOT_H - 8) iscale = tex.Width > tex.Height ? (InterfaceFCMBuffs.SLOT_H - 8) / tex.Width : (InterfaceFCMBuffs.SLOT_H - 8) / tex.Height;

			sb.Draw(tex, pos + new Vector2(InterfaceFCMBuffs.SLOT_H / 2, InterfaceFCMBuffs.SLOT_H / 2), null, Color.White, 0f, tex.Size() / 2, iscale, SpriteEffects.None, 0f);
			Vector2 measure = Main.fontMouseText.MeasureString(MyBuffDef.noModName);
			float tscale = .75f;
			if (measure.X * .75f > InterfaceFCMBuffs.SLOT_W - InterfaceFCMBuffs.SLOT_H - 6) tscale = (InterfaceFCMBuffs.SLOT_W - InterfaceFCMBuffs.SLOT_H - 6) / measure.X;
			SDrawing.StringShadowed(sb, Main.fontMouseText, MyBuffDef.noModName, pos + new Vector2(InterfaceFCMBuffs.SLOT_H, InterfaceFCMBuffs.SLOT_H / 2 + 2), Color.White, tscale, new Vector2(0, measure.Y / 2));
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}

		public bool IsBuffActive()
		{
			return MyBuffDef != null && Main.localPlayer.HasBuff(MyBuffDef.type) >= 0;
		}
	}
}
