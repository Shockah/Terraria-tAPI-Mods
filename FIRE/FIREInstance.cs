using Microsoft.Xna.Framework;
using System;
using System.Threading;
using Terraria;

namespace Shockah.FIRE
{
	public class FIREInstance
	{
		public const int startupTicks = 60;
		public const int speedV = 4;
		public const int radius = 6;
		public const float decaySpeed = .05f;
		
		public int x, y, targetY;
		public float alpha = 1f;
		public bool firstTick = true;
		public int toGo = 0;

		public FIREInstance(int x, int y)
		{
			this.x = x;
			this.y = -startupTicks * speedV;
			targetY = y;
		}

		public bool Update()
		{
			toGo++;
			if (firstTick)
			{
				Thread thread = new Thread(new ThreadStart(() =>
					{
						if (Main.rand == null) Main.rand = new Random((int)DateTime.Now.Ticks);
						if (WorldGen.genRand == null) WorldGen.genRand = new Random(((int)DateTime.Now.Ticks)^Main.rand.Next());
						while (alpha > 0f)
						{
							if (Main.gameMenu) break;
							while (toGo > 0)
							{
								toGo--;
								y += speedV;

								int x1 = Math.Min(Math.Max(x - radius, 0), Main.maxTilesX - 1);
								int x2 = Math.Min(Math.Max(x + radius, 0), Main.maxTilesX - 1);
								int y1 = Math.Min(Math.Max(y - speedV, 0), Main.maxTilesY - 1);
								int y2 = Math.Min(Math.Max(y, 0), Main.maxTilesY - 1);

								if (y2 != 0 && y1 != Main.maxTilesY - 1)
								{
									for (int yy = y2; yy >= y1; yy--) for (int xx = x1; xx <= x2; xx++)
										{
											if (Main.tile[xx, yy] == null) Main.tile[xx, yy] = new Tile();
											WorldGen.KillTile(xx, yy, false, false, true);
											WorldGen.KillWall(xx, yy, false);
											Main.tile[xx, yy].wire(false);
											Main.tile[xx, yy].wire2(false);
											Main.tile[xx, yy].wire3(false);
											Main.tile[xx, yy].actuator(false);
											Main.tile[xx, yy].liquid = 0;
										}

									foreach (Player player in Main.player)
									{
										if (!player.active) continue;

										Vector2 c = player.Center;
										if (y * 16 >= c.Y)
										{
											float dist = Math.Abs(x * 16 + 8 - c.X) - radius * 16 * alpha - (player.width + player.height) / 2;
											if (dist <= 0) player.Hurt(1000, Math.Sign(x * 16 + 8 - c.X), true, false, " got incinerated by FIRE.");
										}
									}

									foreach (NPC npc in Main.npc)
									{
										if (!npc.active) continue;
										if (npc.dontTakeDamage) continue;

										Vector2 c = npc.Center;
										if (y * 16 >= c.Y)
										{
											float dist = Math.Abs(x * 16 + 8 - c.X) - radius * 16 * alpha - (npc.width + npc.height) / 2;
											if (dist <= 0) npc.StrikeNPC(1000, 0, Math.Sign(x * 16 + 8 - c.X));
										}
									}
								}

								if (y >= Main.maxTilesY)
								{
									alpha -= decaySpeed;
									if (alpha <= 0f) break;
								}
							}
							Thread.Sleep(5);
						}
					}
				));
				thread.IsBackground = true;
				thread.Priority = ThreadPriority.Lowest;
				thread.Start();
			}

			if (alpha <= 0f) return true;
			return false;
		}
	}
}
