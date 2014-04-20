using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;

namespace Shockah.Achievements
{
	public class ILNotifiers : InterfaceLayer
	{
		internal List<Notifier> notifiers = new List<Notifier>();
		
		public ILNotifiers(string name) : base(name) { }

		protected override void OnDraw(SpriteBatch sb)
		{
			for (int i = 0; i < notifiers.Count; i++)
			{
				if (notifiers[i].Draw(sb)) notifiers.RemoveAt(i--);
			}
		}

		public void Add(Achievement achievement, object icon = null)
		{
			bool[] taken = new bool[notifiers.Count];
			for (int i = 0; i < notifiers.Count; i++) taken[notifiers[i].index] = true;

			for (int i = 0; i < taken.Length; i++)
			{
				if (!taken[i])
				{
					Add(new Notifier(i, achievement, icon));
					return;
				}
			}

			Add(new Notifier(taken.Length, achievement, icon));
		}
		private void Add(Notifier notifier)
		{
			notifiers.Add(notifier);
			notifiers.Sort((n1, n2) => { return n1.index.CompareTo(n2.index); });
		}
	}
}