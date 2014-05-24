using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class MBase : ModBase
	{
		public override void OnAllModsLoaded()
		{
			SBase.EventMenuStateChange += (menu) =>
				{
					if (!menu)
					{
						new FrameCompanionBagButton(this).Create();
						new FrameCompanionBagInterface(this).Create();
					}
				};
		}
	}
}