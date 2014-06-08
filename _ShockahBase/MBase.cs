using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class MBase : APIModBase
	{
		internal static MBase me = null;
		
		internal bool wasInMenu = true;
		
		public override void OnLoad()
		{
			me = this;
			SFrame.LoadAll();
		}

		public override void OnReload()
		{
			SBase.Clear();
			SFrame.Clear();
			SFrame.LoadAll();
		}

		public override void OnAllModsLoaded()
		{
			SBase.EventMenuStateChange += (menu) =>
				{
					if (menu)
					{
						SFrame.DestroyAll(false);
						SFrame.SaveAll();
						SFrame.DestroyAll(true);
						SPopupMenu.menus.Clear();
					}
				};
		}

		public override void PreGameDraw(SpriteBatch sb)
		{
			if (wasInMenu != Main.gameMenu)
			{
				wasInMenu = Main.gameMenu;
				foreach (Action<bool> h in SBase.EventMenuStateChange) h(wasInMenu);
			}
		}
	}
}