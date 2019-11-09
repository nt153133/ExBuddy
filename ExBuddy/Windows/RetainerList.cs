namespace ExBuddy.Windows
{
    using ExBuddy.Enumerations;
    using ExBuddy.Helpers;
    using ff14bot.RemoteWindows;
    using System.Threading.Tasks;

    public sealed class RetainerList : Window<RetainerList>
    {
        public RetainerList()
            : base("RetainerList") { }

        public SendActionResult SelectRetainer(uint index)
        {
            return TrySendAction(2, 1, 2, 1, index);
        }

        public async Task<bool> SelectRetainerAndSkipDialog(uint index, byte attempts = 20, ushort interval = 200)
        {
            var result = SendActionResult.None;
            var requestAttempts = 0;
            await Behaviors.Wait(interval, () => RetainerList.IsOpen);

            while (result != SendActionResult.Success && !Talk.DialogOpen && requestAttempts++ < attempts && Behaviors.ShouldContinue)
            {
                result = SelectRetainer(index);
                if (result == SendActionResult.InjectionError) await Behaviors.Sleep(interval);
                if (result == SendActionResult.InvalidWindow) await Refresh(interval);

                await Behaviors.Wait(interval, () => Talk.DialogOpen);
            }

            if (requestAttempts > attempts)
            {
                return false;
            }

            await Behaviors.Sleep(interval);

            await Behaviors.Wait(5000, () => Talk.DialogOpen);

            // Dialog didin't open somehow
            if (!Talk.DialogOpen) return false;

            Talk.Next();

            await Behaviors.Wait(interval, () => SelectString.IsOpen);

            return SelectString.IsOpen;
        }
    }
}