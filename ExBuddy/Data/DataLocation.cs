namespace ExBuddy.Data
{
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class DataLocation
    {
        [System.Runtime.CompilerServices.MethodImpl(MethodImplOptions.NoInlining)]
        public static DirectoryInfo SourceDirectory()
        {
            var frame = new StackFrame(0, true);
            var file = frame.GetFileName();

            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                return new DirectoryInfo(Path.GetDirectoryName(file));
            }

            return null;
        }
    }
}