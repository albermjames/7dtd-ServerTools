﻿using System.Collections.Generic;

namespace ServerTools
{
    class RestartVote
    {
        public static bool IsEnabled = false, VoteOpen = false, Startup = false;
        public static int Players_Online = 5, Votes_Needed = 3, Admin_Level = 0;
        public static string Command66 = "restartvote", Command70 = "yes";
        public static List<int> Restart = new List<int>();

        public static void CallForVote1(ClientInfo _cInfo)
        {
            if (!Startup)
            {
                if (!VoteOpen)
                {
                    bool _adminOnline = false;
                    List<ClientInfo> ClientInfoList = PersistentOperations.ClientList();
                    for (int i = 0; i < ClientInfoList.Count; i++)
                    {
                        ClientInfo _cInfoAdmins = ClientInfoList[i];
                        if (_cInfo != _cInfoAdmins)
                        {
                            if (GameManager.Instance.adminTools.GetUserPermissionLevel(_cInfo) <= Admin_Level)
                            {
                                _adminOnline = true;
                                string _phrase748;
                                if (!Phrases.Dict.TryGetValue(748, out _phrase748))
                                {
                                    _phrase748 = "{Player} has requested a restart vote.";
                                }
                                _phrase748 = _phrase748.Replace("{Player}", _cInfo.playerName);
                                ChatHook.ChatMessage(_cInfoAdmins, LoadConfig.Chat_Response_Color + _phrase748 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                            }
                        }
                    }
                    if (!_adminOnline)
                    {
                        if (ConnectionManager.Instance.ClientCount() >= Players_Online)
                        {
                            string _phrase740;
                            if (!Phrases.Dict.TryGetValue(740, out _phrase740))
                            {
                                _phrase740 = "A vote to restart the server has opened and will close in 60 seconds. Type {CommandPrivate}{Command70} to cast your vote.";
                            }
                            _phrase740 = _phrase740.Replace("{CommandPrivate}", ChatHook.Command_Private);
                            _phrase740 = _phrase740.Replace("{Command70}", Command70);
                            ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase740 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                            VoteOpen = true;
                        }
                        else
                        {
                            string _phrase741;
                            if (!Phrases.Dict.TryGetValue(741, out _phrase741))
                            {
                                _phrase741 = "There are not enough players online to start a restart vote.";
                            }
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase741 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                    else
                    {
                        string _phrase749;
                        if (!Phrases.Dict.TryGetValue(749, out _phrase749))
                        {
                            _phrase749 = "A administrator is currently online. They have been alerted.";
                        }
                        ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase749 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                }
                else
                {
                    string _phrase824;
                    if (!Phrases.Dict.TryGetValue(824, out _phrase824))
                    {
                        _phrase824 = "There is a vote already open.";
                    }
                    ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase824 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
            else
            {
                string _phrase816;
                if (!Phrases.Dict.TryGetValue(816, out _phrase816))
                {
                    _phrase816 = "You must wait thirty minutes after the server starts before opening a restart vote.";
                }
               ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase816 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void ProcessRestartVote()
        {
            if (Restart.Count > 0)
            {
                if (Restart.Count >= Votes_Needed)
                {
                    string _phrase742;
                    if (!Phrases.Dict.TryGetValue(742, out _phrase742))
                    {
                        _phrase742 = "Players voted yes to a server restart. Shutdown has been initiated.";
                    }
                    ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase742 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                    SdtdConsole.Instance.ExecuteSync(string.Format("st-StopServer 2"), (ClientInfo)null);
                }
                else
                {
                    string _phrase743;
                    if (!Phrases.Dict.TryGetValue(743, out _phrase743))
                    {
                        _phrase743 = "Players voted yes but not enough votes were cast to restart.";
                    }
                    ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase743 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                }
            }
            else
            {
                string _phrase746;
                if (!Phrases.Dict.TryGetValue(746, out _phrase746))
                {
                    _phrase746 = "No votes were cast to restart the server.";
                }
                ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase746 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
            }
            Restart.Clear();;
        }
    }
}
