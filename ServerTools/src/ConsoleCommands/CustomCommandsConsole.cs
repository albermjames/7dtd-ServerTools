﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class CustomCommandsConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable custom commands.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. CustomCommands off\n" +
                   "  2. CustomCommands on\n" +
                   "1. Turn off custom commands\n" +
                   "2. Turn on custom commands\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-CustomCommands", "cc", "st-cc" };
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
                    if (CustomCommands.IsEnabled)
                    {
                        CustomCommands.IsEnabled = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Custom commands has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Credentials is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!CustomCommands.IsEnabled)
                    {
                        CustomCommands.IsEnabled = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Custom commands has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Credentials is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in CustomCommandsConsole.Execute: {0}", e));
            }
        }
    }
}