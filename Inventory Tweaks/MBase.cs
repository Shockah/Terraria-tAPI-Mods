using Shockah.Base;
using TAPI;
using TAPI.UIKit;
using Terraria;
using Shockah.InvTweaks.SlotActions;

namespace Shockah.InvTweaks
{
	public class MBase : ModBase
	{
		public override void OnLoad()
		{
			MInterface minterface = (MInterface)modInterface;

			minterface.actions.Add(new QuickMoveAction());
			minterface.actions.Add(new TrashAction());

			minterface.actions.Add(new TakeHalfAction());
			minterface.actions.Add(new PutOneAction());

			minterface.actions.Add(new OverrideTrashAction());
		}
	}
}