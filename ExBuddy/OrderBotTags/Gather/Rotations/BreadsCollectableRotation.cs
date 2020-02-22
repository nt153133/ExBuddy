namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using ExBuddy.Attributes;
    using ExBuddy.Interfaces;
    using ExBuddy.Logging;
    using ExBuddy.OrderBotTags.Gather;
    using ff14bot;
    using Helpers;
    using System.Threading.Tasks;

    // Ideally we'd want to start with at least 400 gp if a 'worst-case' finisher happens, but eh.
    [GatheringRotation("BreadsCollectScrip", 35, 800, 700, 650, 600, 500, 450, 400, 300, 250, 200, 0)]
    public sealed class CollectEndGameGatheringRotation : CollectableGatheringRotation, IGetOverridePriority
    {
        private readonly ushort level = Core.Player.ClassLevel;

        #region IGetOverridePriority Members

        int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
        {
            if (tag.CollectableItem != null && tag.CollectableItem.Value >= 450 && level >= 71 && tag.Node.IsUnspoiled())
            {
                return 601;
            }

            return -1;
        }

        #endregion IGetOverridePriority Members

        public override async Task<bool> ExecuteRotation(ExGatherTag tag)
        {
            var desiredRarity = tag.CollectableItem.Value;

            int steps;
            for (steps = 0; steps < 3; steps++)
            {
                await Impulsive(tag);
                await Wait();
                if (HasDiscerningEye) break;
            }
            while (steps < 2)
            {
                if (Core.Player.CurrentGP >= 200) await SingleMind(tag);
                await Instinctual(tag);
                steps++;
            }

            if (CurrentRarity >= desiredRarity)
            {
                await IncreaseChance(tag);
                return true;
            }
            else if (CurrentRarity >= desiredRarity - 55)
            {
                if (Core.Player.CurrentGP >= 200) await SingleMind(tag);
                await Stickler(tag);
                await IncreaseChance(tag);
                return true;
            }
            else
            {
                if ((Core.Player.CurrentGP < 400 && !HasDiscerningEye) || Core.Player.CurrentGP < 200) Logger.Instance.Info("Not enough GP for our bad RNG finisher. Trying anyways. Current GP: {0}", Core.Player.CurrentGP);
                if (Core.Player.CurrentGP >= 400 && !HasDiscerningEye) await UtmostDiscerning(tag);
                else if (Core.Player.CurrentGP >= 200) await UtmostCaution(tag);
                if (Core.Player.CurrentGP >= 200) await SingleMind(tag);
                await Methodical(tag);
                await IncreaseChance(tag);
                return true;
            }
        }
    }
}