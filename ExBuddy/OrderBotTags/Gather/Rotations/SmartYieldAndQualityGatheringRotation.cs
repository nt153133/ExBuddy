namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using ExBuddy.Attributes;
    using ExBuddy.Enumerations;
    using ExBuddy.Interfaces;
    using ff14bot;
    using ff14bot.Managers;
    using System.Threading.Tasks;

    //Name, RequiredTime, RequiredGpBreakpoints
    [GatheringRotation("SmartYieldAndQuality", 25, 900, 850, 800, 750, 700, 650, 600, 550, 500, 0)]
    public class SmartYieldAndQualityGatheringRotation : SmartGatheringRotation, IGetOverridePriority
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

            if (tag.GatherIncrease == GatherIncrease.YieldAndQuality
                || (tag.GatherIncrease == GatherIncrease.Auto && Core.Player.ClassLevel >= 40 && Core.Player.CurrentGP >= 650))
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
                await IncreaseYieldAndQuality(tag);
                return await base.ExecuteRotation(tag);
            }

            return true;
        }
    }
}