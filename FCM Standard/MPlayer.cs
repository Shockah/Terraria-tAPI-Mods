using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.FCM.Standard
{
	public class MPlayer : ModPlayer
	{
		public const float NOCLIP_SPEED = 8f, NOCLIP_SPEED_MULTIPLIER = 4f;
		
		public bool cheatGod, cheatNoclip;
		public Vector2 oldPos = new Vector2(-1, -1);
		
		public MPlayer(ModBase modBase, Player player) : base(modBase, player) { }

		public override void OnUpdate()
		{
			if (cheatGod)
			{
				player.immune = true;
				player.immuneAlpha = 0;
				player.breath = player.breathMax;
				if (player.lifeRegen < 0) player.lifeRegen = 0;
				player.poisoned = player.venom = player.onFire = player.onFire2 = player.onFrostBurn = player.burned = player.suffocating = false;
			}

			if (cheatNoclip)
			{
				if (oldPos.X != -1 || oldPos.Y != -1) player.position = oldPos;
				player.velocity = default(Vector2);
				player.gravControl = false;
				player.sloping = false;
				player.stepSpeed = 160f;
				player.step = 0;

				player.oldPosition = player.position;
				float speed = NOCLIP_SPEED;
				if (Main.keyState.IsKeyDown(Keys.LeftShift)) speed *= NOCLIP_SPEED_MULTIPLIER;
				if (player.controlLeft) player.position.X -= speed;
				if (player.controlRight) player.position.X += speed;
				if (player.controlUp) player.position.Y -= speed;
				if (player.controlDown) player.position.Y += speed;
				oldPos = player.position;
			}
		}
	}
}