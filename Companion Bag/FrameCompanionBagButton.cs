using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class FrameCompanionBagButton : SFrame
	{
		protected LittleButton button = null;

		public FrameCompanionBagButton(ModBase modBase) : base(modBase, "Button", Anchor.TopLeft, new Vector2(512, 20), new Vector2(32, 32)) { }

		protected override void OnCreate()
		{
			button = new LittleButton("Companions", modBase.textures["CompanionBag.png"], () => { return Interface.current is InterfaceCompanionBag; }, () => { InterfaceCompanionBag.me.Open(); });
		}

		protected override void OnRender(SpriteBatch sb)
		{
			if (!Main.playerInventory) return;
			if (InterfaceCompanionBag.me == null) new InterfaceCompanionBag(modBase);
			button.Draw(this, sb, pos);
		}
	}
}
