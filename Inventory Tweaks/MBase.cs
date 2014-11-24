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

			minterface.actions.Add(new QuickMoveAction(SlotAction.MButton.Left, SlotAction.KKeys.Shift));
			minterface.actions.Add(new TrashAction(SlotAction.MButton.Left, SlotAction.KKeys.Shift));

			minterface.actions.Add(new TakeHalfAction(SlotAction.MButton.Right, SlotAction.KKeys.None));
			minterface.actions.Add(new PutOneAction(SlotAction.MButton.Right, SlotAction.KKeys.None));
		}
	}
}