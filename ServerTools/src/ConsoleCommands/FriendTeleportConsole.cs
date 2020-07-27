﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class FriendTeleportConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable friend teleport.";
        }
        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. FriendTeleport off\n" +
                   "  2. FriendTeleport on\n" +
                   "1. Turn off friend teleport\n" +
                   "2. Turn on friend teleport\n";
        }
        public override string[] GetCommands()
        {
            return new string[] { "st-FriendTeleport", "ft", "st-ft" };
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
                    if (FriendTeleport.IsEnabled)
                    {
                        FriendTeleport.IsEnabled = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Friend teleport has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Friend teleport is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!FriendTeleport.IsEnabled)
                    {
                        FriendTeleport.IsEnabled = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Friend teleport has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Friend teleport is already on"));
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
                Log.Out(string.Format("[SERVERTOOLS] Error in FriendTeleportConsole.Execute: {0}", e));
            }
        }
    }
}