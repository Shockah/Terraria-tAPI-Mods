﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class LittleButton
	{
		public readonly string name;
		public readonly Texture2D tex;
		public readonly Func<bool> isPressed;
		public readonly System.Action onClick;

		public LittleButton(string name, Texture2D tex, Func<bool> isPressed, System.Action onClick)
		{
			this.name = name;
			this.tex = tex;
			this.isPressed = isPressed;
			this.onClick = onClick;
		}

		public Vector2 Size { get { return tex.Size(); } }

		public virtual void Draw(SpriteBatch sb, Vector2 pos)
		{
			Color c = isPressed() ? Color.White : Color.Gray;
			sb.Draw(tex, pos, c);

			if (new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y).Contains(Main.mouseX, Main.mouseY))
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