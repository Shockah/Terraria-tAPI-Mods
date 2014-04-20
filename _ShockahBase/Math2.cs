using Microsoft.Xna.Framework;
using System;

namespace Shockah.Base
{
	public class Math2
	{
		public static double LdirX(double dist, double angle)
		{
			return -Math.Cos(ToRadians(angle + 180d)) * dist;
		}
		public static double LdirY(double dist, double angle)
		{
			return Math.Sin(ToRadians(angle + 180d)) * dist;
		}
		public static Vector2 LdirVector2(double dist, double angle)
		{
			return new Vector2((float)Math2.LdirX(dist, angle), (float)Math2.LdirY(dist, angle));
		}

		public static double ToRadians(double degrees)
		{
			return degrees * Math.PI / 180;
		}
		public static double ToDegrees(double radians)
		{
			return radians * 180 / Math.PI;
		}
	}
}
