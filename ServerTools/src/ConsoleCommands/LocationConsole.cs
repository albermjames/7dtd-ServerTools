﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class LocationConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable location.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. Location off\n" +
                   "  2. Location on\n" +
                   "1. Turn off chat command /loc\n" +
                   "2. Turn on chat command /loc\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-Location", "loc", "st-loc" };
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
                    if (Loc.IsEnabled)
                    {
                        Loc.IsEnabled = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Location has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Location is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!Loc.IsEnabled)
                    {
                        Loc.IsEnabled = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Location has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Location is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in LocationConsole.Execute: {0}", e.Message));
            }
        }
    }
}