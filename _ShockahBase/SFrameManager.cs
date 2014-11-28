using System;
using System.Collections.Generic;
using LitJson;
using FluentPath;
using Terraria;
using TAPI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Shockah.Base
{
	public class SFrameManager
	{
		public static FPath path = Mods.path.Combine("Shockah.Base.SFrame.json");

		public readonly MBase modBase;
		internal JsonData data = JsonData.Object();
		public List<SFrame> frames = new List<SFrame>();
		internal List<SFrame> framesAdd = new List<SFrame>(), framesRemove = new List<SFrame>();
		public SFrame actionOn = null;
		public bool parentAnchorMode = false;

		public SFrameManager(MBase modBase)
		{
			this.modBase = modBase;
		}

		public void SaveAll()
		{
			path.Write(JsonMapper.ToJson(data));
		}
		public void LoadAll()
		{
			if (path.Exists)
				data = JsonMapper.ToObject(path.Read());
		}

		public void DestroyAll(bool force = false)
		{
			if (force)
			{
				framesAdd.Clear();
				framesRemove.Clear();
				frames.Clear();
			}
			else
				foreach (SFrame sf in frames)
					sf.Destroy();
		}

		public void UpdateAll()
		{
			foreach (SFrame sf in framesRemove) frames.Remove(sf);
			foreach (SFrame sf in framesAdd) frames.Add(sf);
			framesAdd.Clear();
			framesRemove.Clear();
			foreach (SFrame sf in frames) sf.Update();

			if (!Main.hideUI && Main.playerInventory && frames.Count != 0 && Math2.InRegion(Main.mouse, new Vector2(0, Main.screenHeight - 16), new Vector2(16, Main.screenHeight)))
			{
				Main.localPlayer.mouseInterface = true;
				modBase.tip = "Left click to (un)lock all frames\nRight click to switch anchor setting mode";
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					bool allLocked = true;
					foreach (SFrame sf in frames) if (!sf.locked) allLocked = false;
					foreach (SFrame sf in frames) sf.locked = !allLocked;
					parentAnchorMode = false;
					Main.PlaySound(22, -1, -1, 1);
				}
				if (Main.mouseRight && Main.mouseRightRelease)
				{
					parentAnchorMode = !parentAnchorMode;
					Main.PlaySound(22, -1, -1, 1);
				}
			}
		}
		public void DrawAll(SpriteBatch sb)
		{
			foreach (SFrame sf in frames) sf.Render(sb);
			foreach (SFrame sf in frames) sf.Render2(sb);
			foreach (SFrame sf in frames) sf.Render3(sb);

			if (!Main.hideUI && Main.playerInventory && frames.Count != 0)
				sb.Draw(Main.HBLockTexture[frames[0].locked ? 0 : 1], new Vector2(8, Main.screenHeight - 8), null, parentAnchorMode ? Color.Red : Color.White, 0f, Main.HBLockTexture[0].Size() / 2, 1f, SpriteEffects.None, 0f);
		}
	}
}