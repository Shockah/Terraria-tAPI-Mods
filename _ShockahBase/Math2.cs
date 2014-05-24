using Microsoft.Xna.Framework;
using System;

namespace Shockah.Base
{
	public class Math2
	{
		public static float LdirX(float dist, float angle)
		{
			return (float)(-Math.Cos(ToRadians(angle + 180f)) * dist);
		}
		public static float LdirY(float dist, float angle)
		{
			return (float)(Math.Sin(ToRadians(angle + 180f)) * dist);
		}
		public static Vector2 LdirVector2(float dist, float angle)
		{
			return new Vector2((float)Math2.LdirX(dist, angle), (float)Math2.LdirY(dist, angle));
		}

		public static float ToRadians(float degrees)
		{
			return (float)(degrees * Math.PI / 180f);
		}
		public static float ToDegrees(float radians)
		{
			return (float)(radians * 180f / Math.PI);
		}

		public static bool InRegion(Vector2 pos, Vector2 pos1, float w, float h)
		{
			return pos.X >= pos1.X && pos.Y >= pos1.Y && pos.X < pos1.X + w && pos.Y < pos1.Y + h;
		}
		public static bool InRegion(Vector2 pos, Vector2 pos1, Vector2 pos2)
		{
			return pos.X >= pos1.X && pos.Y >= pos1.Y && pos.X < pos2.X && pos.Y < pos2.Y;
		}
	}
}
