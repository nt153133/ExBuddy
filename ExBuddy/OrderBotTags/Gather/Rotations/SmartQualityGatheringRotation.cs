namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using ExBuddy.Attributes;
    using ExBuddy.Enumerations;
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
    }
}