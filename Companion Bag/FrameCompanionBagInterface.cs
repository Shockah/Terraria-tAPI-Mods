using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.CompanionBag
{
	public class FrameCompanionBagInterface : SFrame
	{
		public static SFrame me = null;
		
		public FrameCompanionBagInterface(ModBase modBase) : base(modBase, "Bag", Anchor.TopLeft, new Vector2(InterfaceCompanionBag.POS_X, InterfaceCompanionBag.POS_Y)) { }

		protected override void OnCreate()
		{
			me = this;
		}

		protected override void OnUpdate()
		{
			if (InterfaceCompanionBag.me == null) return;
			size.X = Math.Min(Main.localPlayer.GetSubClass<MPlayer>().companions.Count, InterfaceCompanionBag.COLS) * InterfaceCompanionBag.OFF_X * .75f;
			size.Y = Math.Min((int)Math.Ceiling(Main.localPlayer.GetSubClass<MPlayer>().companions.Count * 1f / InterfaceCompanionBag.COLS), InterfaceCompanionBag.ROWS) * InterfaceCompanionBag.OFF_Y * .75f;
		}

		protected override bool IsVisible()
		{
			if (InterfaceCompanionBag.me == null) return false;
			return Interface.current is InterfaceCompanionBag;
		}

		protected override void OnRender(SpriteBatch sb)
		{
			if (InterfaceCompanionBag.me == null) return;
			if (Interface.current is InterfaceCompanionBag) InterfaceCompanionBag.me.DrawFrame(sb);
		}
	}
}
