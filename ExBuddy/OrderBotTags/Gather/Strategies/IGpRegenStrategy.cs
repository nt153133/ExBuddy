namespace ExBuddy.OrderBotTags.Gather.Strategies
{
    using Enumerations;
    using ff14bot.Objects;
    using Interfaces;
    using System.Threading.Tasks;

    internal interface IGpRegenStrategy
    {
        Task<GpRegenStrategyResult> RegenerateGp(GatheringPointObject node, IGatheringRotation gatherRotation, GatherStrategy gatherStrategy, CordialTime cordialTime, CordialType cordialType);
    }
}