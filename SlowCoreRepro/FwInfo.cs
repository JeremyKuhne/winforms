using Microsoft.Win32;

namespace CreateControlsCSharp
{
    internal class FrameworkAndOsInfo
    {
        public static string GetOsName()
        {
            var os_info = System.Environment.OSVersion;

            var version =
                os_info.Version.Major.ToString() + "." +
                os_info.Version.Minor.ToString();

            string os = $"Unknown ({os_info.Version.Build})";

            switch (version)
            {
                case "10.0": os = $"Win 10 ({os_info.Version.Build})"; break;
                case "6.3": os = $"Win 8.1 ({os_info.Version.Build})"; break;
                case "6.2": os = $"Win 8 ({os_info.Version.Build})"; break;
                case "6.1": os = $"Win 7 ({os_info.Version.Build})"; break;
                case "6.0": os = $"Win Vista ({ os_info.Version.Build})"; break;
                case "5.2": os = $"Win XP 64-Bit Edition ({os_info.Version.Build})"; break;
                case "5.1": os = $"Win XP ({os_info.Version.Build})"; break;
                case "5.0": os = $"Win 2000 ({os_info.Version.Build})"; break;
            }
            return os;
        }

        public static string Get45PlusFromRegistry()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    return $".NET Framework Version: {CheckFor45PlusVersion((int)ndpKey.GetValue("Release"))}";
                }
                else
                {
                    return "A .NET Framework Version >= 4.5 was not detected.";
                }
            }

            // Checking the version using >= enables forward compatibility.
            string CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 528040)
                    return "4.8 or later";
                if (releaseKey >= 461808)
                    return "4.7.2";
                if (releaseKey >= 461308)
                    return "4.7.1";
                if (releaseKey >= 460798)
                    return "4.7";
                if (releaseKey >= 394802)
                    return "4.6.2";
                if (releaseKey >= 394254)
                    return "4.6.1";
                if (releaseKey >= 393295)
                    return "4.6";
                if (releaseKey >= 379893)
                    return "4.5.2";
                if (releaseKey >= 378675)
                    return "4.5.1";
                if (releaseKey >= 378389)
                    return "4.5";
                // This code should never execute. A non-null release key should mean
                // that 4.5 or later is installed.
                return "No 4.5 or later version detected";
            }
        }
    }
}
