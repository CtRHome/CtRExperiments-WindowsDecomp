using CutTheRope.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutTheRope.iframework;

internal partial class FrameworkTypes
{
    public partial class AndroidAPI
    {
        public static partial void openUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Win32Exception ex)
            {
                if (ex.ErrorCode != -2147467259)
                {
                }
            }
            catch (Exception)
            {
            }
        }

        public static partial void exitApp()
        {
            Global.XnaGame.Exit();
        }
    }
}
