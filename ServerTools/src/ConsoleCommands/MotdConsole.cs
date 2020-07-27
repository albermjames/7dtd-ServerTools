﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class MotdConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable motd(messsage of the day).";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. Motd off\n" +
                   "  2. Motd on\n" +
                   "1. Turn off the motd\n" +
                   "2. Turn on the motd\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-Motd", "motd", "st-motd" };
        }
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 1, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    if (Motd.IsEnabled)
                    {
                        Motd.IsEnabled = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Motd has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Motd is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!Motd.IsEnabled)
                    {
                        Motd.IsEnabled = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Motd has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Motd is already on"));
                        return;
                    }
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in MotdConsole.Execute: {0}", e));
            }
        }
    }
}