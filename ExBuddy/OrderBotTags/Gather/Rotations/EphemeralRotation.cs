namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using ExBuddy.Attributes;
    using ExBuddy.Enumerations;
    using ExBuddy.Helpers;
    using ExBuddy.Interfaces;
    using ff14bot;
    using System.Threading.Tasks;

    //Name, RequiredTime, RequiredGpBreakpoints
    [GatheringRotation("Ephemeral")]
    public sealed class EphemeralGatheringRotation : GatheringRotation, IGetOverridePriority
    {
        #region IGetOverridePriority Members

        int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
        {
            if (tag.Node.IsEphemeral() && tag.CollectableItem == null)
            {
                return 9100;
            }

            return -1;
        }

        #endregion IGetOverridePriority Members

        public override async Task<bool> ExecuteRotation(ExGatherTag tag)
        {
            // Yield And Quality
            if (tag.GatherIncrease == GatherIncrease.YieldAndQuality
                || (tag.GatherIncrease == GatherIncrease.Auto && Core.Player.ClassLevel >= 40 && Core.Player.CurrentGP >= 650))
            {
                if (Core.Player.CurrentGP >= 500 && Core.Player.ClassLevel >= 40)
                {
                    await tag.Cast(Ability.IncreaseGatherYield2);

                    if (Core.Player.CurrentGP >= 100)
                    {
                        await tag.Cast(Ability.IncreaseGatherQuality10);
                    }

                    return await base.ExecuteRotation(tag);
                }

                return true;
            }

            // Yield
            if (tag.GatherIncrease == GatherIncrease.Yield || (tag.GatherIncrease == GatherIncrease.Auto && Core.Player.ClassLevel >= 40))
            {
                if (Core.Player.CurrentGP >= 500 && Core.Player.ClassLevel >= 40)
                {
                    await tag.Cast(Ability.IncreaseGatherYield2);
                    return await base.ExecuteRotation(tag);
                }

                if (Core.Player.CurrentGP >= 400 && Core.Player.ClassLevel >= 30 && (Core.Player.ClassLevel < 40 || Core.Player.MaxGP < 500))
                {
                    await tag.Cast(Ability.IncreaseGatherYield);
                    return await base.ExecuteRotation(tag);
                }

                if (Core.Player.CurrentGP >= 300 && Core.Player.ClassLevel >= 25 && (Core.Player.ClassLevel < 30 || Core.Player.MaxGP < 400))
                {
                    await Wait();

                    if (!tag.GatherItem.TryGatherItem())
                    {
                        return false;
                    }

                    await tag.Cast(Ability.AdditionalAttempt);
                    return await base.ExecuteRotation(tag);
                }

                return true;
            }

            // Quality
            if (tag.GatherIncrease == GatherIncrease.Quality
                || (tag.GatherIncrease == GatherIncrease.Auto && Core.Player.ClassLevel >= 15 && Core.Player.ClassLevel < 40))
            {
                if (Core.Player.CurrentGP >= 300)
                {
                    if (Core.Player.ClassLevel >= 63)
                    {
                        await tag.Cast(Ability.IncreaseGatherQuality30100);
                    }
                    else
                    {
                        await tag.Cast(Ability.IncreaseGatherQuality30);
                    }
                    return await base.ExecuteRotation(tag);
                    ;
                }

                return true;
            }

            return await base.ExecuteRotation(tag);
        }
    }
}