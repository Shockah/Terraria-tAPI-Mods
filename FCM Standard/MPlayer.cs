using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Shockah.Base;
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
		public const float
			NOCLIP_SPEED = 8f, NOCLIP_SPEED_MULTIPLIER = 4f,
			CAMERA_SPEED = 16f, CAMERA_SPEED_MULTIPLIER = 4f;
		
		public bool cheatGod, cheatNoclip, cheatUsage, cheatRange, cheatTileSpeed, cheatTileUsage;
		public Vector2 oldPos = new Vector2(-1, -1), lastCameraPos = new Vector2(-1, -1);
		
		public override void MidUpdate()
		{
			if (player.whoAmI == Main.myPlayer && InterfaceFCMMisc.freeCamera)
			{
				if (lastCameraPos.X == -1 && lastCameraPos.Y == -1) lastCameraPos = Main.screenPosition;

				float speed = CAMERA_SPEED;
				if (player.controlTorch) speed *= CAMERA_SPEED_MULTIPLIER;
				if (player.controlLeft) lastCameraPos.X -= speed;
				if (player.controlRight) lastCameraPos.X += speed;
				if (player.controlUp) lastCameraPos.Y -= speed;
				if (player.controlDown) lastCameraPos.Y += speed;
				player.controlTorch = player.controlLeft = player.controlRight = player.controlUp = player.controlDown = false;
			}
			else
			{
				lastCameraPos = new Vector2(-1, -1);
			}
			
			if (cheatGod)
			{
				player.immune = true;
				player.immuneAlpha = 0;
				player.breath = player.breathMax;
				if (player.lifeRegen < 0) player.lifeRegen = 0;
				player.poisoned = player.venom = player.onFire = player.onFire2 = player.onFrostBurn = player.burned = player.suffocating = 0;
				player.noFallDmg = true;
			}

			if (cheatNoclip)
			{
				if (oldPos.X != -1 || oldPos.Y != -1) player.position = oldPos;
				player.velocity = default(Vector2);
				player.gravControl = false;
				player.sloping = false;
				player.stepSpeed = 160f;
				player.step = 0;
				player.noFallDmg = true;

				player.oldPosition = player.position;
				if (!InterfaceFCMMisc.freeCamera || player.whoAmI != Main.myPlayer)
				{
					float speed = NOCLIP_SPEED;
					if (player.controlTorch) speed *= NOCLIP_SPEED_MULTIPLIER;
					if (player.controlLeft) player.position.X -= speed;
					if (player.controlRight) player.position.X += speed;
					if (player.controlUp) player.position.Y -= speed;
					if (player.controlDown) player.position.Y += speed;
				}
				oldPos = player.position;
			}

			if (cheatRange)
			{
				player.tileRangeX = Main.maxTilesX;
				player.tileRangeY = Main.maxTilesY;
			}

			if (cheatUsage)
			{
				Item held = player.heldItem;
				MItem mi = held.GetSubClass<MItem>();
				if (mi.lastMana != -1)
				{
					held.mana = mi.lastMana;
					mi.lastMana = -1;
				}
			}

			if (cheatTileSpeed)
			{
				Item held = player.heldItem;
				if (!held.IsBlank() && (held.createTile >= 0 || held.createWall > 0 || held.pick > 0 || held.axe > 0 || held.hammer > 0))
				{
					player.itemTime = 0;
				}
			}
		}
	}
}