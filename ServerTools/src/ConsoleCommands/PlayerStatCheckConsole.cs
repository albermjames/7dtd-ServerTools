﻿using System;
using System.Collections.Generic;
using ServerTools.AntiCheat;

namespace ServerTools
{
    class PlayerStatCheckConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable player stat check.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. PlayerStatCheck off\n" +
                   "  2. PlayerStatCheck on\n" +
                   "1. Turn off player stat check\n" +
                   "2. Turn on player stat check\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-PlayerStatCheck", "psc", "st-psc" };
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
                    if (PlayerStats.IsEnabled)
                    {
                        PlayerStats.IsEnabled = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Player stat check has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Player stat check is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!PlayerStats.IsEnabled)
                    {
                        PlayerStats.IsEnabled = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Player stat check has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Player stat check is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in PlayerStatCheckConsole.Execute: {0}", e.Message));
            }
        }
    }
}