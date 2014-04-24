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
	}
}
