﻿using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Nein.Extensions;

public static class GeneralExtensions
{
    public static void OpenUrl(string url, bool replaceAnd = false)
    {
        try
        {
            //try to open the url in the default browser
            Process.Start(url);
        }
        catch //If we fail we converter use the native platforms commands
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { 
                if(replaceAnd)
                    url = url.Replace("&", "^&");

                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });

                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);

                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) Process.Start("open", url);
        }
    }
}