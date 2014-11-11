using FluentPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TAPI;
using Terraria;
using Terraria.DataStructures;

namespace Shockah.FCM.Standard
{
	public class InterfaceFCMSnapshot : InterfaceFCMBase
	{
		public const int POS_X = 20, POS_Y = 300;
		public const int OFF_X = (int)((9 * 56 - 4) * 0.85) - 20, OFF_Y = 48;
		public const int ROWS = 5;

		public static InterfaceFCMSnapshot me = null;
		public static readonly FPath path = Main.SavePath + "/Mods/Shockah.FCM.Standard.Snapshots.dat";

		protected readonly ElSlider slider;
		protected readonly ElButton bSave, bLoad, bDelete;
		public bool resetSnapshots = true;
		protected SnapshotSlot[] slots = new SnapshotSlot[ROWS];
		public int selected = -1;
		private int _Scroll = 0;
		public List<PlayerSnapshot> snapshots = new List<PlayerSnapshot>();

		protected int Scroll
		{
			get
			{
				return _Scroll;
			}
			set
			{
				_Scroll = Math.Min(Math.Max(value, 0), ScrollMax);
			}
		}
		protected int ScrollMax
		{
			get
			{
				return Math.Max((int)Math.Ceiling(1f * (snapshots.Count - ROWS)), 0);
			}
		}

		public static void Reset()
		{
		}

		public InterfaceFCMSnapshot()
		{
			me = this;
			if (Main.dedServ) return;
			Refresh(true);

			slider = new ElSlider(
				(scroll) => { if (Scroll != scroll) { Scroll = scroll; Refresh(false); } },
				() => { return Scroll; },
				() => { return ROWS; },
				() => { return (int)Math.Ceiling(1f * snapshots.Count); }
			);

			bSave = new ElButton(
				(b, mb) =>
				{
					selected = -1;
					snapshots.Add(PlayerSnapshot.Create(Main.localPlayer.name));
					Refresh(false);
					new Thread(new ThreadStart(Save)).Start();
				},
				(b, sb, mb) =>
				{
					Vector2 measure = Main.fontMouseText.MeasureString("Save");
					Drawing.StringShadowed(sb, Main.fontMouseText, "Save", b.pos + b.size / 2, Color.White, 0.85f, measure / 2);
				}
			);

			bLoad = new ElButton(
				(b, mb) =>
				{
					if (selected == -1) return;
					PlayerSnapshot.Restore(snapshots[selected]);
				},
				(b, sb, mb) =>
				{
					Vector2 measure = Main.fontMouseText.MeasureString("Load");
					Drawing.StringShadowed(sb, Main.fontMouseText, "Load", b.pos + b.size / 2, Color.White, 0.85f, measure / 2);
				}
			);

			bDelete = new ElButton(
				(b, mb) =>
				{
					if (selected == -1) return;
					snapshots.RemoveAt(selected);
					selected = -1;
					Refresh(false);
					new Thread(new ThreadStart(Save)).Start();
				},
				(b, sb, mb) =>
				{
					Vector2 measure = Main.fontMouseText.MeasureString("Delete");
					Drawing.StringShadowed(sb, Main.fontMouseText, "Delete", b.pos + b.size / 2, Color.White, 0.85f, measure / 2);
				}
			);
		}

		public override void OnOpen()
		{
			base.OnOpen();

			if (resetSnapshots)
			{
				resetSnapshots = false;
				if (path.Exists)
					new Thread(new ThreadStart(Load)).Start();
			}

			selected = -1;
			Scroll = 0;
		}

		public void Load()
		{
			using (FileStream fs = File.OpenRead(path))
			{
				BinBuffer bb = new BinBuffer(fs);
				int count = bb.ReadInt();
				while (count-- > 0)
				{
					PlayerSnapshot snapshot = new PlayerSnapshot(bb.ReadString(), DateTime.FromBinary(bb.ReadLong()));
					snapshot.Load(bb);
					snapshots.Add(snapshot);
				}
			}
		}

		public void Save()
		{
			if (path.Exists)
				path.Delete();
			using (FileStream fs = File.OpenWrite(path))
			{
				BinBuffer bb = new BinBuffer(fs);
				bb.Write(snapshots.Count);
				foreach (PlayerSnapshot snapshot in snapshots)
				{
					bb.Write(snapshot.name);
					bb.Write(snapshot.date.ToBinary());
					snapshot.Save(bb);
				}
			}
		}

		public override void Draw(InterfaceLayer layer, SpriteBatch sb)
		{
			base.Draw(layer, sb);
			bool blocked = false;

			int scrollBy = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 120;
			int oldScroll = Scroll;
			Scroll -= scrollBy;
			if (Scroll != oldScroll) Refresh(false);

			Drawing.StringShadowed(sb, Main.fontMouseText, "Snapshots: " + snapshots.Count, new Vector2(POS_X, POS_Y - 26));

			Main.inventoryScale = 1f;
			int offX = (int)Math.Ceiling(OFF_X * Main.inventoryScale), offY = (int)Math.Ceiling(OFF_Y * Main.inventoryScale);
			for (int y = 0; y < ROWS; y++)
			{
				slots[y].scale = Main.inventoryScale;
				slots[y].UpdatePos(new Vector2(POS_X, POS_Y + y * offY));
				slots[y].Draw(sb, true, !blocked);
			}

			Main.inventoryScale = 1f;
			slider.pos = new Vector2(POS_X + 4 + OFF_X * Main.inventoryScale, POS_Y);
			slider.size = new Vector2(16, ROWS * OFF_Y * Main.inventoryScale);
			blocked = slider.Draw(sb, true, !blocked) || blocked;

			bSave.pos = new Vector2(POS_X + OFF_X + 28, POS_Y + OFF_Y - 34);
			bSave.size = new Vector2(96, 32);
			blocked = bSave.Draw(sb, true, !blocked) || blocked;

			if (selected != -1)
			{
				bLoad.pos = new Vector2(POS_X + OFF_X + 28, POS_Y + OFF_Y + 2);
				bLoad.size = new Vector2(96, 32);
				blocked = bLoad.Draw(sb, true, !blocked) || blocked;

				bDelete.pos = new Vector2(POS_X + OFF_X + 28, POS_Y + OFF_Y + 54);
				bDelete.size = new Vector2(96, 32);
				blocked = bDelete.Draw(sb, true, !blocked) || blocked;
			}
		}

		public void Refresh(bool resetScroll)
		{
			Scroll = resetScroll ? 0 : Scroll;
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = new SnapshotSlot(this, i + Scroll);
				slots[i].size = new Vector2(OFF_X, OFF_Y);
			}
		}
	}
}