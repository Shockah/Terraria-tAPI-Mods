using TAPI;

namespace Shockah.Base
{
	public class MBase : ModBase
	{
		internal static MBase me = null;
		
		public override void OnLoad()
		{
			me = this;
		}
	}
}