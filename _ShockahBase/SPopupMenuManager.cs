using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Shockah.Base
{
	public class SPopupMenuManager
	{
		public List<SPopupMenu> menus = new List<SPopupMenu>();

		public void DrawAll(SpriteBatch sb)
		{
			if (menus.Count == 0) return;
			Main.localPlayer.mouseInterface = true;

			bool mouseHover = false, destroyMe;
			SPopupMenu menuToDestroy = null;
			foreach (SPopupMenu menu in menus)
			{
				mouseHover |= menu.Render(sb, out destroyMe);
				if (destroyMe && menuToDestroy == null) menuToDestroy = menu;
			}
			if (menuToDestroy != null)
				menuToDestroy.Destroy();
			if (!mouseHover && Main.mouseLeft && Main.mouseLeftRelease)
				menus.Clear();
		}
	}
}