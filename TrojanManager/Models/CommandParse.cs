using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrojanManager.Models
{
    public class CommandParse
    {
        public CommandParse(string cmds)
        {
            Commands = cmds;
        }

        public string Parse()
        {
            if(String.IsNullOrEmpty(Commands))
            {
                HackCommands hack = (HackCommands)JsonConvert.DeserializeObject(Commands);


                //if(hack.FlipMouse)
                    //blabla

                //if(hack.FlipScreen)

                //if(hack.Command)

                //if(hack.GetLogs)

                //if(hack.GetPrintScreen)
            }

            return String.Empty;
        }
        public string Commands { get; set; }
    }
}
