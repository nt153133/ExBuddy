namespace ExBuddy.OrderBotTags.Gather.Rotations
{
    using Buddy.Coroutines;
    using ExBuddy.Attributes;
    using ExBuddy.Enumerations;
    using ExBuddy.Helpers;
    using ExBuddy.Interfaces;
    using ff14bot;
    using ff14bot.Managers;
    using System;
    using System.Threading.Tasks;

    //Name, RequiredTime, RequiredGpBreakpoints
    [GatheringRotation("BreadsYield", 25, 820, 780, 730, 680, 630, 580, 530, 480, 430, 380, 330, 0)]
    public class EndGameYieldRotation : SmartGatheringRotation, IGetOverridePriority
    {
        private readonly ushort level = Core.Player.ClassLevel;

        #region IGetOverridePriority Members

        int IGetOverridePriority.GetOverridePriority(ExGatherTag tag)
        {
            if (tag.CollectableItem != null || !tag.Node.IsUnspoiled()) return -1;
            if (level >= 40)
            {
                switch (tag.GatherIncrease)
                {
                    case GatherIncrease.Auto:
                        return 9200;
                    case GatherIncrease.Yield:
                        return 9200;
                }
            }

            return -1;
        }

        #endregion IGetOverridePriority Members

        public override async Task<bool> ExecuteRotation(ExGatherTag tag)
        {
            double YieldsLeft()
            {
                return Math.Min(GatheringManager.SwingsRemaining, Math.Max(0, (Core.Player.CurrentGP + GatheringManager.SwingsRemaining * 5) / 100));
            }

            var bountifulYield = tag.GatherItem.Level <= 70 ? 3 : 2;

            int pickCleanYield;
            if (tag.GatherItem.Level <= 50) pickCleanYield = 3;
            else pickCleanYield = tag.GatherItem.Level <= 70 ? 2 : 1;

            await IncreaseChance(tag);
            await Wait();

            if (Core.Player.CurrentGP >= 250 && level >= 77)
            {
                var yieldCalc = Math.Max(0, (Core.Player.CurrentGP + (GatheringManager.MaxSwings - 1) * 5) / 100.0);
                var adjustedGp = yieldCalc - Math.Floor(yieldCalc);
                var pickCleanBonus = adjustedGp >= 0.50 && adjustedGp <= 0.999 ? 1 : 0;

                if (pickCleanYield * GatheringManager.MaxSwings > bountifulYield * (3 - pickCleanBonus))
                {
                    await tag.Cast(Ability.PickClean);
                    await Wait();
                }
            }

            double uncappedYields = Math.Floor((Core.Player.CurrentGP + (GatheringManager.MaxSwings - 1) * 5) / 100d);

            if (Core.Player.CurrentGP >= 500 && level >= 40)
            {
                if ((GatheringManager.MaxSwings * 2) + (uncappedYields - 5) >= bountifulYield * Math.Floor(YieldsLeft()))
                {
                    await tag.Cast(Ability.IncreaseGatherYield2);
                    await Wait();
                }
            }

            bool usedVigorForce = false;

            while (tag.Node.CanGather && GatheringManager.SwingsRemaining > tag.SwingsRemaining && Behaviors.ShouldContinue)
            {
                await Wait();

                if (!usedVigorForce && GatheringManager.GatheringCombo == 4 && GatheringManager.SwingsRemaining > 0 && ((tag.GatherItem.HqChance > 0 && tag.GatherItem.HqChance < 100) || tag.GatherItem.Chance < 100))
                {
                    await tag.Cast(Ability.IncreaseGatherChanceQuality100);
                    usedVigorForce = true;
                }

                if (Core.Player.CurrentGP >= 100 && level >= 68 && (tag.GatherItem.HqChance == 100 || GatheringManager.SwingsRemaining <= YieldsLeft()) && !Core.Player.HasAura(1286))
                {
                    await tag.Cast(Ability.IncreaseGatherYieldOnce2);
                    await Coroutine.Wait(2000, () => Core.Player.HasAura(1286));
                }

                //double counsels = Math.Floor(YieldsLeft() + 0.1);

                //if (Core.Player.CurrentGP >= 10 && level >= 67 && tag.GatherItem.HqChance > 0 && tag.GatherItem.HqChance < 100 && YieldsLeft() >= counsels && !Core.Player.HasAura(1262))
                //{
                //    await tag.Cast(Ability.IncreaseGatherQualityRandomOnce);
                //    await Coroutine.Wait(2000, () => Core.Player.HasAura(1262));
                //}

                if (!await tag.ResolveGatherItem()) return false;

                if (!tag.GatherItem.TryGatherItem()) return false;

                var swingsRemaining = GatheringManager.SwingsRemaining;

                await Coroutine.Yield();
                await Coroutine.Wait(3000, () => swingsRemaining != GatheringManager.SwingsRemaining);
            }

            return true;
        }
    }
}