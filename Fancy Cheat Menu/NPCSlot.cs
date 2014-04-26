using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using Terraria;

namespace Shockah.FCM
{
	public class NPCSlot : El
	{
		public Vector2 pos = new Vector2();
		public float scale = .85f, alpha = 1f;
		public readonly InterfaceFCMNPCs gui;
		public readonly int index;
		public NPC npc = new NPC();

		public NPCSlot(InterfaceFCMNPCs gui, int index)
		{
			this.gui = gui;
			this.index = index;
		}

		public NPC MyNPC
		{
			get
			{
				return index < gui.filtered.Count ? gui.filtered[index] : new NPC();
			}
			set
			{
				gui.filtered[index] = value;
				gui.filtered[index].whoAmI = Main.npc.Length - 1;
			}
		}

		public virtual bool IsActive()
		{
			return true;
		}

		public void UpdateAndDraw(SpriteBatch sb)
		{
			Update();
			Draw(sb);
		}
		public void UpdateAndDraw(SpriteBatch sb, Vector2 pos)
		{
			Update(pos);
			Draw(sb);
		}
		public void UpdateAndDraw(SpriteBatch sb, Vector2 pos, float scale)
		{
			Update(pos, scale);
			Draw(sb);
		}

		public virtual void UpdatePos(Vector2 pos)
		{
			this.pos = pos;
		}
		public void Update(Vector2 pos)
		{
			UpdatePos(pos);
			Update();
		}
		public void Update(float scale)
		{
			this.scale = scale;
			Update();
		}
		public void Update(Vector2 pos, float scale)
		{
			this.scale = scale;
			UpdatePos(pos);
			Update();
		}
		public virtual void Update()
		{
			if (!IsActive()) return;
			if (IsMouseOnSlot())
			{
				Main.localPlayer.mouseInterface = true;
				if (Main.mouseLeft) OnLeftClick(ref Main.mouseLeftRelease);
				else if (Main.mouseRight) OnRightClick(ref Main.mouseRightRelease);

				if (MyNPC.type > 0)
				{
					bool isBoss = SBase.IsBoss(MyNPC);
					MBase.tip += new STooltip.Line(string.IsNullOrEmpty(MyNPC.displayName) ? MyNPC.name : MyNPC.displayName, (MyNPC.friendly || MyNPC.damage <= 0 ? "#0f0;Friendly#;" : "#f00;Hostile#;") + (MyNPC.townNPC ? ", #0f0;Town NPC#;" : "") + (isBoss ? ", #f00;Boss#;" : ""), Color.White, Color.White);

					MBase.tip += new STooltip.Line("" + MyNPC.lifeMax + " life");
					if (MyNPC.damage > 0) MBase.tip += new STooltip.Line("" + MyNPC.damage + " damage");
					MBase.tip += new STooltip.Line("" + MyNPC.defense + " defense");
				}
			}
		}
		public virtual bool IsMouseOnSlot()
		{
			return Main.mouseX > pos.X && Main.mouseX < pos.X + Main.inventoryBackTexture.Width * scale - 1 && Main.mouseY > pos.Y && Main.mouseY < pos.Y + Main.inventoryBackTexture.Height * scale - 1;
		}

		public virtual void OnLeftClick(ref bool release)
		{
			InterfaceFCMNPCs.spawning = MyNPC;
		}
		public virtual void OnRightClick(ref bool release)
		{
			
		}

		public NPC ShouldHold()
		{
			if (index >= 0 && index < gui.filtered.Count) return gui.filtered[index];
			return null;
		}

		public virtual void Draw(SpriteBatch sb)
		{
			if (!IsActive()) return;
			DrawSlotBackground(sb);
			DrawSlotNPC(sb);
		}
		public virtual void DrawSlotBackground(SpriteBatch sb)
		{
			Texture2D tex = Main.inventoryBackTexture;
			sb.Draw(tex, pos, null, Main.inventoryBack * alpha, 0f, default(Vector2), scale, SpriteEffects.None, 0f);
		}
		public virtual void DrawSlotNPC(SpriteBatch sb)
		{
			if (MyNPC.type > 0)
			{
				float iscale = 1f;
				if (MyNPC.type <= Main.maxNPCTypes) Main.LoadNPC(MyNPC.type);
				Texture2D texNPC = Main.npcTexture[MyNPC.type];
				Rectangle rect = new Rectangle(0, 0, texNPC.Width, texNPC.Height / Main.npcFrameCount[MyNPC.type]);
				if (rect.Width > 40 || rect.Height > 40) iscale = rect.Width > rect.Height ? 40f / rect.Width : 40f / rect.Height;
				iscale *= scale;

				sb.Draw(
					texNPC,
					new Vector2(pos.X + 26f * scale - texNPC.Width * 0.5f * iscale, pos.Y + 26f * scale - rect.Height * 0.5f * iscale),
					new Rectangle?(rect), MyNPC.GetAlpha(Color.White) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
				);

				if (MyNPC.color != default(Color))
				{
					sb.Draw(
						texNPC,
						new Vector2(pos.X + 26f * scale - texNPC.Width * 0.5f * iscale, pos.Y + 26f * scale - rect.Height * 0.5f * iscale),
						new Rectangle?(rect), MyNPC.GetColor(Color.White) * alpha, 0f, default(Vector2), iscale, SpriteEffects.None, 0f
					);
				}
			}
		}

		public bool Draw(SpriteBatch sb, bool draw, bool update)
		{
			if (update) Update();
			if (draw) Draw(sb);
			return false;
		}
	}
}
