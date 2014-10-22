using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.MyFavWeapon
{
	[GlobalMod] public class MItem : ModItem
	{
		public String name = null;
		public int statDamage, statCrit, statUseTime, statUseAnimation, statValue, statRare;
		public float statKnockback, statVelocity;

		public override void Save(BinBuffer bb)
		{
			if (name == null) return;

			bb.Write(name);
			bb.Write(statValue);
			bb.Write(statRare);

			bb.Write(statDamage);
			bb.Write(statCrit);
			bb.Write(statUseTime);
			bb.Write(statUseAnimation);
			bb.Write(statKnockback);
			bb.Write(statVelocity);
		}

		public override void Load(BinBuffer bb)
		{
			if (!bb.HasLeft) return;

			name = bb.ReadString();
			statValue = bb.ReadInt();
			statRare = bb.ReadInt();

			statDamage = bb.ReadInt();
			statCrit = bb.ReadInt();
			statUseTime = bb.ReadInt();
			statUseAnimation = bb.ReadInt();
			statKnockback = bb.ReadFloat();
			statVelocity = bb.ReadFloat();

			Reapply();
		}

		public void Backup()
		{
			statValue = item.value;
			statRare = item.rare;
			
			statDamage = item.damage;
			statCrit = item.crit;
			statUseTime = item.useTime;
			statUseAnimation = item.useAnimation;
			statKnockback = item.knockBack;
			statVelocity = item.shootSpeed;
		}
		public void Restore()
		{
			item.value = statValue;
			item.rare = statRare;
			
			item.damage = statDamage;
			item.crit = statCrit;
			item.useTime = statUseTime;
			item.useAnimation = statUseAnimation;
			item.knockBack = statKnockback;
			item.shootSpeed = statVelocity;
		}

		public void Reapply()
		{
			Prefix prefix = item.prefix;
			item.CopyFrom(item.def.item);
			Restore();
			Apply();
			if (prefix != null && prefix != Prefix.None) prefix.ApplyToItem(item);
		}
		public void Apply()
		{
			item.displayName = name;
		}

		[CallPriority(-10000f)] public override string OnAffixName(string currentName, string oldName)
		{
			if (name == null) return currentName;
			currentName = name;

			if (item.prefix != null && item.prefix.id != Terraria.Prefix.None.id)
			{
				currentName = item.prefix.SetItemName(item);
			}
			return currentName;
		}

		public override void PostReforge()
		{
			if (name != null)
			{
				Reapply();
			}
		}
	}
}