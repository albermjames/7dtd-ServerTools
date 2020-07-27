﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class NormalPlayerColoringConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable normal player coloring.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. NormalPlayerColoring off\n" +
                   "  2. NormalPlayerColoring on\n" +
                   "1. Turn off normal player coloring\n" +
                   "2. Turn on normal player coloring\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-NormalPlayerColoring", "npc", "st-npc" };
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
                    if (ChatHook.Normal_Player_Color_Prefix)
                    {
                        ChatHook.Normal_Player_Color_Prefix = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Normal player coloring has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Normal player coloring is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (ChatHook.Normal_Player_Color_Prefix)
                    {
                        ChatHook.Normal_Player_Color_Prefix = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Normal player coloring has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Normal player coloring is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in NormalPlayerColoringConsole.Execute: {0}", e.Message));
            }
        }
    }
}