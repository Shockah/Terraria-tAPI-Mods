using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;
using Shockah.Base;

namespace Shockah.Achievements
{
	public class MBase : ModBase
	{
		public static ModBase me = null;
		internal static SBase sbase = null;
		internal static Achievements ca

		public override void OnLoad()
		{
			me = this;
			Achievements.me = new Achievements();

			if (!Main.dedServ)
			{
				Texture2DData texd;

				texd = new Texture2DData(Notifier.WIDTH, Notifier.HEIGHT);
				for (int x = 0; x < texd.width; x++) for (int y = 0; y < texd.height; y++)
				{
					float xx = Math.Min(x >= texd.width / 2 ? texd.width - x : x, 6) / 6f;
					float yy = Math.Min(y >= texd.height / 2 ? texd.height - y : y, 6) / 6f;
					texd[x, y] = Color.Lerp(Notifier.COLOR_TOP, Notifier.COLOR_BOTTOM, 1f * y / texd.height) * (xx * yy);
				}
				textures["Notifier"] = texd.CreateTexture();

				MInterface.LayerNotifiers = new ILNotifiers(modName + ":Notifiers");
			}
		}
		
		public override void OnAllModsLoaded()
		{
			sbase = (SBase)CallInMod("Shockah.Base");
			MyAchievements.Init();
		}
	}
}