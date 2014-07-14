using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class STooltip
	{
		public class Line
		{
			public string textL, textR;
			public Color colorL = Color.White, colorR = Color.White;
			public float scaleL, scaleR;

			public Line(string text) : this(text, "", Color.White, Color.White, 1f, 1f) { }
			public Line(string text, Color c, float scale = 1f) : this(text, "", c, c, scale, scale) { }
			public Line(string textL, string textR) : this(textL, textR, Color.White, Color.White, 1f, 1f) { }
			public Line(string textL, string textR, float scaleL, float scaleR) : this(textL, textR, Color.White, Color.White, scaleL, scaleR) { }
			public Line(string textL, string textR, Color colorL, Color colorR) : this(textL, textR, colorL, colorR, 1f, 1f) { }
			public Line(string textL, string textR, Color colorL, Color colorR, float scaleL, float scaleR)
			{
				this.textL = textL;
				this.textR = textR;
				this.colorL = colorL;
				this.colorR = colorR;
				this.scaleL = scaleL;
				this.scaleR = scaleR;
			}
		}

		public static readonly STooltip global = new STooltip();
		
		public List<Line> lines = new List<Line>();
		public float sideSeparator = 32f;
		public Color background = new Color(0, 0, 0, 0);
		public float? alpha = null;
		public float scale = 1f;

		public void Clear()
		{
			lines.Clear();
		}

		public static STooltip operator +(STooltip tip, Line line)
		{
			tip.lines.Add(line);
			return tip;
		}

		public void Add(Line line)
		{
			lines.Add(line);
		}
		

		public virtual void Draw(SpriteBatch sb, Vector2 pos)
		{
			//if (!Main.toolTip.item.IsBlank() || !string.IsNullOrEmpty(Main.buffString) || !string.IsNullOrEmpty(Main.drawingTooltip)) return;
			if (lines.Count == 0) return;
			Vector2 sizeCalc = ActualDraw(null, default(Vector2));
			if (pos.X + sizeCalc.X > Main.screenWidth) pos.X = Main.screenWidth - sizeCalc.X;
			if (pos.Y + sizeCalc.Y > Main.screenHeight) pos.Y = Main.screenHeight - sizeCalc.Y;
			if (background != new Color(0, 0, 0, 0) || alpha.HasValue)
			{
				if (alpha.HasValue)
				{
					Drawing.DrawBox(sb, pos.X - 6, pos.Y - 6, sizeCalc.X + 12, sizeCalc.Y + 12, alpha.Value);
				}
				else
				{
					Drawing.DrawBox(sb, pos.X - 6, pos.Y - 6, sizeCalc.X + 12, sizeCalc.Y + 12, background);
				}
			}
			foreach (Action<STooltip, Rectangle> h in SBase.EventPreSTooltipDraw) h(this, new Rectangle((int)pos.X, (int)pos.Y, (int)sizeCalc.X, (int)sizeCalc.Y));
			ActualDraw(sb, pos, sizeCalc);
		}

		public virtual Vector2 ActualDraw(SpriteBatch sb, Vector2 pos, Vector2 sizeCalc = default(Vector2))
		{
			if (lines.Count == 0) return default(Vector2);
			Vector2 size = new Vector2();
			Vector2 sizeL, sizeR;

			foreach (Line line in lines)
			{
				float h = Math.Max(line.scaleL, line.scaleR) * Main.fontMouseText.LineSpacing;
				sizeL = SDrawing.MeasureColorCodedString(Main.fontMouseText, line.textL) * line.scaleL * scale;
				sizeR = SDrawing.MeasureColorCodedString(Main.fontMouseText, line.textR) * line.scaleR * scale;
				size.X = Math.Max(size.X, sizeL.X + sizeR.X + (sizeL.X != 0f && sizeR.X != 0f ? sideSeparator : 0f));
				size.Y += Math.Max(sizeL.Y, sizeR.Y);
				if (sb != null)
				{
					SDrawing.DrawColorCodedStringShadow(sb, Main.fontMouseText, line.textL, pos, Color.Black, 0f, default(Vector2), line.scaleL * scale, -1f, 2);
					SDrawing.DrawColorCodedString(sb, Main.fontMouseText, line.textL, pos, line.colorL * (Main.mouseTextColor / 255f), 0f, default(Vector2), line.scaleL * scale);
					SDrawing.DrawColorCodedStringShadow(sb, Main.fontMouseText, line.textR, pos + new Vector2(sizeCalc.X - sizeR.X, 0), Color.Black, 0f, default(Vector2), line.scaleR * scale, -1f, 2);
					SDrawing.DrawColorCodedString(sb, Main.fontMouseText, line.textR, pos + new Vector2(sizeCalc.X - sizeR.X, 0), line.colorR * (Main.mouseTextColor / 255f), 0f, default(Vector2), line.scaleR * scale);
				}
				pos.Y += Math.Max(sizeL.Y, sizeR.Y);
			}
			return size;
		}
	}
}