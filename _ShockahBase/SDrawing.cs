using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TAPI;

namespace Shockah.Base
{
	public static class SDrawing
	{
		/*
		 * most of this class copies the tAPI's colorcoded string drawing, fixing some of its issues
		 */
		
		public const string
			REGEX_RESET = "#;",
			REGEX_COLOR1 = "#([0-9a-f]);",
			REGEX_COLOR3 = "#([0-9a-f]{3});",
			REGEX_COLOR6 = "#([0-9a-f]{6});",
			REGEX_COLOR_BRIGHTER = "#>([0-9]{1,3});",
			REGEX_COLOR_DARKER = "#<([0-9]{1,3});",
			REGEX_COLOR_ADD = "#\\+([0-9a-f]{1,2});",
			REGEX_COLOR_SUB = "#\\-([0-9a-f]{1,2});",
			REGEX_COLOR_INVERT = "#\\^;",
			REGEX_COLOR_SWITCH = "#([RGB]{3});",
			REGEX_SCALE = "#%([0-9]{1,3});";
		public static readonly string BUILT_REGEX = "(#[^#]*?;)";
		public static readonly int[][] shadowOffset = new int[][] { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } };
		
		public static void StringShadowed(SpriteBatch sb, SpriteFont font, string text, Vector2 pos)
		{
			StringShadowed(sb, font, text, pos, Color.White);
		}
		public static void StringShadowed(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color c, float scale = 1f, Vector2 origin = default(Vector2))
		{
			Drawing.DrawStringShadow(sb, font, text, pos, new Color(0, 0, 0, c.A), 0f, origin, scale);
			sb.DrawString(font, text, pos, c, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		public static Vector2 DrawColorCodedString(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, float maxW = -1f)
		{
			return DrawColorCodedString(sb, font, text, pos, Color.White, 0f, default(Vector2), 1f, maxW);
		}
		public static Vector2 DrawColorCodedString(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color baseColor, float rotation = 0f, Vector2 origin = default(Vector2), float scale = 1f, float maxW = -1f)
		{
			return DrawColorCodedString(sb, font, text, pos, baseColor, rotation, origin, new Vector2(scale, scale), maxW);
		}
		public static Vector2 DrawColorCodedString(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxW = -1f, bool ignoreColors = false)
		{
			Vector2 xy = new Vector2(pos.X, pos.Y);
			Vector2 xy_size = xy;
			string[] spl1 = text.Split('\n');
			float spaceWidth = font.MeasureString(" ").X;

			Color c = Color.White;
			float scale = 1f, maxScale = 0f;
			foreach (string s1 in spl1)
			{
				string[] spl = Regex.Split(s1, BUILT_REGEX);
				foreach (string s in spl)
				{
					if (Regex.Match(s, BUILT_REGEX).Success)
					{
						Match m;

						m = Regex.Match(s, REGEX_RESET);
						if (m.Success)
						{
							scale = 1f;
							if (ignoreColors) continue;
							c = Color.White;
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR1);
						if (m.Success)
						{
							if (ignoreColors) continue;
							float comp = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber) / 15f;
							c = new Color(comp, comp, comp, c.A / 255f);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR3);
						if (m.Success)
						{
							if (ignoreColors) continue;
							string match = m.Groups[1].Value;
							float
								compR = int.Parse("" + match.ElementAt(0), System.Globalization.NumberStyles.HexNumber) / 15f,
								compG = int.Parse("" + match.ElementAt(1), System.Globalization.NumberStyles.HexNumber) / 15f,
								compB = int.Parse("" + match.ElementAt(2), System.Globalization.NumberStyles.HexNumber) / 15f;
							c = new Color(compR, compG, compB, c.A / 255f);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR6);
						if (m.Success)
						{
							if (ignoreColors) continue;
							string match = m.Groups[1].Value;
							float
								compR = int.Parse(match.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
								compG = int.Parse(match.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f,
								compB = int.Parse(match.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
							c = new Color(compR, compG, compB, c.A / 255f);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR_BRIGHTER);
						if (m.Success)
						{
							if (ignoreColors) continue;
							int match = int.Parse(m.Groups[1].Value);
							c = new Color(c.R / 255f * (1f + match * .01f), c.G / 255f * (1f + match * .01f), c.B / 255f * (1f + match * .01f), c.A / 255f);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR_DARKER);
						if (m.Success)
						{
							if (ignoreColors) continue;
							int match = int.Parse(m.Groups[1].Value);
							c = new Color(c.R / 255f * (1f - match * .01f), c.G / 255f * (1f - match * .01f), c.B / 255f * (1f - match * .01f), c.A / 255f);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR_ADD);
						if (m.Success)
						{
							if (ignoreColors) continue;
							int match = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
							c = new Color((byte)Math.Min(c.R + match, 255), (byte)Math.Min(c.G + match, 255), (byte)Math.Min(c.B + match, 255), c.A);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR_SUB);
						if (m.Success)
						{
							if (ignoreColors) continue;
							int match = int.Parse(m.Groups[1].Value, System.Globalization.NumberStyles.HexNumber);
							c = new Color((byte)Math.Max(c.R - match, 0), (byte)Math.Max(c.G - match, 0), (byte)Math.Max(c.B - match, 0), c.A);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR_INVERT);
						if (m.Success)
						{
							if (ignoreColors) continue;
							c = new Color((byte)(255 - c.R), (byte)(255 - c.G), (byte)(255 - c.B), c.A);
							continue;
						}

						m = Regex.Match(s, REGEX_COLOR_SWITCH);
						if (m.Success)
						{
							if (ignoreColors) continue;
							string match = m.Groups[1].Value;
							int
								compR = match.ElementAt(0) == 'R' ? c.R : (match.ElementAt(0) == 'G' ? c.G : c.B),
								compG = match.ElementAt(1) == 'R' ? c.R : (match.ElementAt(1) == 'G' ? c.G : c.B),
								compB = match.ElementAt(2) == 'R' ? c.R : (match.ElementAt(2) == 'G' ? c.G : c.B);
							c = new Color(compR, compG, compB, c.A);
							continue;
						}

						m = Regex.Match(s, REGEX_SCALE);
						if (m.Success)
						{
							scale = int.Parse(m.Groups[1].Value) * 0.01f;
							continue;
						}
					}
					else
					{
						string[] words = s.Split(' ');
						for (int i = 0; i < words.Length; i++)
						{
							if (i != 0) xy.X += spaceWidth * baseScale.X * scale;

							if (maxW > 0f)
							{
								float textw = font.MeasureString(words[i]).X * baseScale.X * scale;
								if (xy.X - pos.X + textw > maxW)
								{
									xy.X = pos.X;
									xy.Y += font.LineSpacing * maxScale * baseScale.Y;
									xy_size.Y = Math.Max(xy_size.Y, xy.Y);
									maxScale = 0f;
								}
							}

							if (maxScale < scale) maxScale = scale;
							sb.DrawString(font, words[i], xy, c.Multiply(baseColor), rotation, origin, baseScale * scale, SpriteEffects.None, 0f);
							xy.X += font.MeasureString(words[i]).X * baseScale.X * scale;
							xy_size.X = Math.Max(xy_size.X, xy.X);
						}
					}
				}
				xy.X = pos.X;
				xy.Y += font.LineSpacing * maxScale * baseScale.Y;
				xy_size.Y = Math.Max(xy_size.Y, xy.Y);
				maxScale = 0f;
			}
			return xy_size;
		}

		public static void DrawColorCodedStringShadow(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, float maxW = -1f, int offset = 1)
		{
			DrawColorCodedStringShadow(sb, font, text, pos, new Color(0f, 0f, 0f, .5f), 0f, default(Vector2), 1f, maxW, offset);
		}
		public static void DrawColorCodedStringShadow(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color, float rotation = 0f, Vector2 origin = default(Vector2), float scale = 1f, float maxW = -1f, int offset = 1)
		{
			DrawColorCodedStringShadow(sb, font, text, pos, color, rotation, origin, new Vector2(scale, scale), maxW, offset);
		}
		public static void DrawColorCodedStringShadow(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color, float rotation, Vector2 origin, Vector2 scale, float maxW = -1f, int offset = 1)
		{
			color = new Color(color.R, color.G, color.B, (byte)(Math.Pow(color.A / 255f, 2) * 255));
			for (int i = 0; i < shadowOffset.Length; i++)
			{
				DrawColorCodedString(sb, font, text, new Vector2(pos.X + shadowOffset[i][0] * offset/* * scale.X*/, pos.Y + shadowOffset[i][1] * offset/* * scale.Y*/), color, rotation, origin, scale, maxW, true);
			}
		}

		public static string DropColorCodes(string text)
		{
			string[] spl = Regex.Split(text, BUILT_REGEX);
			StringBuilder sb = new StringBuilder();

			foreach (string s in spl)
			{
				if (!Regex.Match(s, BUILT_REGEX).Success) sb.Append(s);
			}
			return sb.ToString();
		}
		public static Vector2 MeasureColorCodedString(SpriteFont font, string text)
		{
			return font.MeasureString(DropColorCodes(text));
		}
	}
}