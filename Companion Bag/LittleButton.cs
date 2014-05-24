using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class LittleButton
	{
		public readonly string name;
		public readonly Texture2D tex;
		public readonly Func<bool> isPressed;
		public readonly Action onClick;

		public LittleButton(string name, Texture2D tex, Func<bool> isPressed, Action onClick)
		{
			this.name = name;
			this.tex = tex;
			this.isPressed = isPressed;
			this.onClick = onClick;
		}

		public Vector2 Size { get { return tex.Size(); } }

		public virtual void Draw(SFrame frame, SpriteBatch sb, Vector2 pos)
		{
			Color c = isPressed() ? Color.White : Color.Gray;
			sb.Draw(tex, pos, c);

			if (frame.dragging.HasValue) return;
			if (Math2.InRegion(Main.mouse, pos, pos + Size))
			{
				Main.localPlayer.mouseInterface = true;
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					if (isPressed()) Interface.current.Close();
					else onClick();
				}
			}
		}
	}
}
