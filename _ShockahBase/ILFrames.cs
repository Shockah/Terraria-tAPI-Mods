using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class ILFrames : InterfaceLayer
	{
		public ILFrames() : base(MBase.me.modName + ":Frames") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			SFrame.UpdateAll();
			SFrame.RenderAll(sb);
		}
	}
}
