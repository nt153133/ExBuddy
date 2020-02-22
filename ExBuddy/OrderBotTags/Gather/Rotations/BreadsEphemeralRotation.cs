namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using ExBuddy.Attributes;
    using ExBuddy.Helpers;
    using ExBuddy.Interfaces;
    using ExBuddy.Logging;
    using ExBuddy.OrderBotTags.Gather;
    using ff14bot;
    using ff14bot.Managers;
    using System.Threading.Tasks;

    [GatheringRotation("BreadsEphemeral", 35, 800, 600, 400, 200, 0)]
    public sealed class EndGameEphemeralGatheringRotation : CollectableGatheringRotation, IGetOverridePriority
    {

        #region IGetOverridePriority Members

        int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
        {
            if (tag.CollectableItem != null && tag.CollectableItem.Value >= 450 && Core.Player.ClassLevel >= 71 && tag.Node.IsEphemeral())
            {
                return 601;
            }

            return -1;
        }

        #endregion IGetOverridePriority Members

        public override async Task<bool> ExecuteRotation(ExGatherTag tag)
        {
            if (GatheringManager.SwingsRemaining < 5 && tag.NodesGatheredAtMaxGp < 4)
            {
                for (var steps = 0; steps < 3; steps++) await Instinctual(tag);
                return true;
            }

            if (Core.Player.CurrentGP >= 800) await SingleMind(tag);
            await Instinctual(tag);
            if (Core.Player.CurrentGP >= 600) await SingleMind(tag);
            await Instinctual(tag);
            if (CurrentRarity >= 283)
            {
                if (Core.Player.CurrentGP >= 200) await SingleMind(tag);
                await Instinctual(tag);
                if (CurrentRarity >= 490) goto Collect;
                if (CurrentRarity >= 433)
                {
                    if (Core.Player.CurrentGP >= 200) await SingleMind(tag);
                    await Stickler(tag);
                    goto Collect;
                }
                if (Core.Player.CurrentGP >= 200) await UtmostCaution(tag);
                else goto BadRNG;
                await Methodical(tag);
                goto Collect;
            }
            await Instinctual(tag);
            if (CurrentRarity >= 341)
            {
                if (Core.Player.CurrentGP >= 400) await SingleMind(tag);
                if (Core.Player.CurrentGP >= 200) await UtmostCaution(tag);
                else goto BadRNG;

                if (CurrentRarity >= 375)
                {
                    await Methodical(tag);
                    goto Collect;
                }
                await Instinctual(tag);
                if (CurrentRarity >= 490) goto Collect;
                await Stickler(tag);
                goto Collect;
            }
            if (Core.Player.CurrentGP >= 400) await UtmostDiscerning(tag);
            else if (Core.Player.CurrentGP >= 200) await UtmostCaution(tag);
            else goto BadRNG;
            await Methodical(tag);
            if (CurrentRarity >= 490) goto Collect;
            await Stickler(tag);
            goto Collect;

            BadRNG:
            if (Core.Player.CurrentGP < 200) Logger.Instance.Info("Not enough GP for a bad RNG finisher. Collecting anyways. Current GP: {0}", Core.Player.CurrentGP);
            await Stickler(tag);

            Collect:
            await IncreaseChance(tag);
            return true;
        }
    }
}