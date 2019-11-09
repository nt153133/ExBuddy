namespace ExBuddy.OrderBotTags.Gather.GatherSpots
{
	using Buddy.Coroutines;
	using Clio.XmlEngine;
	using ExBuddy.Helpers;
	using ff14bot;
	using System.Threading.Tasks;
	using ff14bot.Behavior;
	using ff14bot.Managers;
	using ff14bot.Navigation;

    [XmlElement("StealthGatherSpot")]
	public class StealthGatherSpot : GatherSpot
	{
		[XmlAttribute("UnstealthAfter")]
		public bool UnstealthAfter { get; set; }

		public override async Task<bool> MoveFromSpot(ExGatherTag tag)
		{
			tag.StatusText = "Moving from " + this;

#if RB_CN
            if (UnstealthAfter && Core.Player.HasAura((int)AbilityAura.Stealth))
			{
				return await tag.CastAura(Ability.Stealth);
			}
#else
            if (UnstealthAfter && Core.Player.HasAura((int)AbilityAura.Sneak))
            {
                return await tag.CastAura(Ability.Sneak);
            }
#endif

            return true;
		}

		public override async Task<bool> MoveToSpot(ExGatherTag tag)
		{
			tag.StatusText = "Moving to " + this;

			var result =
				await
					NodeLocation.MoveTo(
						UseMesh,
						radius: tag.Distance,
						name: tag.Node.EnglishName,
						stopCallback: tag.MovementStopCallback,
						dismountAtDestination: true);

			if (result)
			{
			    var landed = MovementManager.IsDiving || await NewNewLandingTask();
			    if (landed && Core.Player.IsMounted)
			        ActionManager.Dismount();

			    Navigator.Stop();
                await Coroutine.Yield();
#if RB_CN
				await tag.CastAura(Ability.Stealth, AbilityAura.Stealth);
#else
                await tag.CastAura(Ability.Sneak, AbilityAura.Sneak);
#endif
            }

            await Coroutine.Yield();

			return result;
        }

        private async Task<bool> NewNewLandingTask()
        {
            if (!MovementManager.IsFlying) { return true; }

            while (MovementManager.IsFlying) { ActionManager.Dismount(); await Coroutine.Sleep(500); }
            return true;
        }
    }
}