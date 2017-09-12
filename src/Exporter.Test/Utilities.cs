
using System.IO;
using System.Collections.Generic;
namespace EnvSettingsManager
{
    public static class Utilities
    {
        public static bool AreXmlFilesEqual(string lhs, string rhs)
        {
            string[] LhsLines = File.ReadAllLines(lhs);
            string[] RhsLines = File.ReadAllLines(rhs);
            if (LhsLines.Length != RhsLines.Length)
                return false;

            LhsLines = DiscardTimestamp(LhsLines);
            RhsLines = DiscardTimestamp(RhsLines);
            for (int i = 0; i < LhsLines.Length; i++)
            {
                if (LhsLines[i] != RhsLines[i])
                    return false;
            }

            return true;
        }

        static string[] DiscardTimestamp(string[] lines)
        {
            for (int i = 0; i < 12 && i < lines.Length; i++)
            {
                if (lines[i].Contains("Created"))
                {
                    lines[i] = "";
                    break;
                }
            }
            return lines;
        }
    }
}
