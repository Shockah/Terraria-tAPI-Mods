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
		public ILFrames() : base(MBase.me.mod.InternalName + ":Frames") { }

		protected override void OnDraw(SpriteBatch sb)
		{
			MBase.me.frameManager.UpdateAll();
			MBase.me.frameManager.DrawAll(sb);
		}
	}
}
