using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace Shockah.MoveAnything
{
	public class FrameHiding : SFrame
	{
		public bool hidden = false;

		public FrameHiding(string tag, Anchor anchor = Anchor.TopLeft, Vector2 pos = default(Vector2), Vector2 size = default(Vector2)) : base(MBase.me, tag, anchor, pos, size) { }
		public FrameHiding(string tag, Anchor anchor, Vector2 pos, Vector2 size, Resizable resizable) : base(MBase.me, tag, anchor, pos, size, resizable) { }

		protected sealed override void OnRender(SpriteBatch sb)
		{
			if (hidden) sb.DrawString(Main.fontMouseText, modBase.modName + ":" + tag, FramePos1() + new Vector2(8, 8), Color.White, 0f, default(Vector2), .5f, SpriteEffects.None, 0f);
			else OnRender2(sb);
		}
		protected virtual void OnRender2(SpriteBatch sb) { }

		protected override void OnUnlockedRightClick()
		{
			 new SPopupMenu().Add(
				new SPopupMenu.Element(hidden ? "Show" : "Hide", () => { hidden = !hidden; })
			 ).Create();
		}

		protected override void OnSave(JsonData j)
		{
			j["hidden"] = hidden;
		}
		protected override void OnLoad(JsonData j)
		{
			hidden = (bool)j["hidden"];
		}
	}
}