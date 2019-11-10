namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using Buddy.Coroutines;
    using ExBuddy.Attributes;
    using ExBuddy.Enumerations;
    using ExBuddy.Helpers;
    using ExBuddy.Interfaces;
    using ff14bot;
    using ff14bot.Managers;
    using System.Threading.Tasks;

    //Name, RequiredTime, RequiredGpBreakpoints
    [GatheringRotation("SmartQuality", 18, 900, 800, 700, 600, 500, 400, 300, 200, 100, 0)]
    public class SmartQualityGatheringRotation : SmartGatheringRotation, IGetOverridePriority
    {
        #region IGetOverridePriority Members

        int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
        {
            if (tag.CollectableItem != null)
            {
                return -1;
            }

            if (tag.GatherItem != null && tag.GatherItem.HqChance < 1)
            {
                return -1;
            }

            if (tag.GatherIncrease == GatherIncrease.Quality
                || (tag.GatherIncrease == GatherIncrease.Auto && Core.Player.ClassLevel >= 15 && Core.Player.ClassLevel < 40))
            {
                return 9001;
            }

            return -1;
        }

        #endregion IGetOverridePriority Members

        public override async Task<bool> ExecuteRotation(ExGatherTag tag)
        {
            if (GatheringManager.SwingsRemaining > 4 || ShouldForceUseRotation(tag, Core.Player.ClassLevel))
            {
                await IncreaseQuality(tag);
                return await base.ExecuteRotation(tag);
            }

            return true;
        }

        public override async Task<bool> Gather(ExGatherTag tag)
        {
            tag.StatusText = "Gathering items";

            while (tag.Node.CanGather && GatheringManager.SwingsRemaining > tag.SwingsRemaining && Behaviors.ShouldContinue)
            {
                await Wait();

                if (Core.Player.CurrentGP >= 100 && Core.Player.ClassLevel >= 58 && GatheringManager.MaxSwings > 4)
                {
                    await tag.Cast(Ability.IncreaseGatherQualityOnce10);
                    await Wait();
                }

                if (GatheringManager.GatheringCombo == 4 && GatheringManager.SwingsRemaining > tag.SwingsRemaining)
                {
                    await tag.Cast(Ability.IncreaseGatherChanceQuality100);
                    await Wait();
                }

                if (!await tag.ResolveGatherItem())
                {
                    return false;
                }

                var swingsRemaining = GatheringManager.SwingsRemaining - 1;

                if (!tag.GatherItem.TryGatherItem())
                {
                    return false;
                }

                var ticks = 0;
                while (swingsRemaining != GatheringManager.SwingsRemaining && ticks++ < 60 && Behaviors.ShouldContinue)
                {
                    await Coroutine.Yield();
                }
            }

            tag.StatusText = "Gathering items complete";

            return true;
        }
    }
}