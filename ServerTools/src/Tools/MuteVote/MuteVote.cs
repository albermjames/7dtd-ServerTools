﻿using System;
using System.Collections.Generic;

namespace ServerTools
{
    class MuteVote
    {
        public static bool IsEnabled = false, VoteOpen = false;
        public static int Players_Online = 5, Votes_Needed = 3;
        public static string Command67 = "mutevote";
        public static List<int> Votes = new List<int>();
        private static ClientInfo _playerMute;

        public static void Vote(ClientInfo _cInfo, string _player)
        {
            if (!VoteOpen)
            {
                if (ConnectionManager.Instance.ClientCount() >= Players_Online)
                {
                    int _entityId;
                    if (int.TryParse(_player, out _entityId))
                    {
                        ClientInfo _playerToMute = ConnectionManager.Instance.Clients.ForEntityId(_entityId);
                        if (_playerToMute != null)
                        {
                            if (Mute.Mutes.Contains(_playerToMute.playerId))
                            {
                                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + "This player is already muted.[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                                return;
                            }
                            _playerMute = _playerToMute;
                            string _phrase775;
                            if (!Phrases.Dict.TryGetValue(775, out _phrase775))
                            {
                                _phrase775 = "A vote to mute {PlayerName} in chat has begun and will close in 60 seconds.";
                            }
                            _phrase775 = _phrase775.Replace("{PlayerName}", _playerToMute.playerName);
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase775 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                            string _phrase776;
                            if (!Phrases.Dict.TryGetValue(776, out _phrase776))
                            {
                                _phrase776 = "Type {CommandPrivate}{Command70} to cast your vote.";
                            }
                            _phrase776 = _phrase776.Replace("{CommandPrivate}", ChatHook.Command_Private);
                            _phrase776 = _phrase776.Replace("{Command70}", RestartVote.Command70);
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase776 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
                            VoteOpen = true;
                        }
                        else
                        {
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + "This player id was not found.[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                }
                else
                {
                    ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + "Not enough players are online to start a vote to mute.[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
        }

        public static void ProcessMuteVote()
        {
            if (MuteVote.Votes.Count >= Votes_Needed)
            {
                Mute.Mutes.Add(_playerMute.playerId);
                PersistentContainer.Instance.Players[_playerMute.playerId].MuteTime = 60;
                PersistentContainer.Instance.Players[_playerMute.playerId].MuteName = _playerMute.playerName;
                PersistentContainer.Instance.Players[_playerMute.playerId].MuteDate = DateTime.Now;
                PersistentContainer.Instance.Save();
                string _phrase777;
                if (!Phrases.Dict.TryGetValue(777, out _phrase777))
                {
                    _phrase777 = "{PlayerName} has been muted for 60 minutes.";
                }
                _phrase777 = _phrase777.Replace("{PlayerName}", _playerMute.playerName);
                ChatHook.ChatMessage(null, LoadConfig.Chat_Response_Color + _phrase777 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Global, null);
            }
            _playerMute = null;
            MuteVote.Votes.Clear();
        }

        public static void List(ClientInfo _cInfo)
        {
            bool _otherUser = false;
            List<ClientInfo> ClientInfoList = PersistentOperations.ClientList();
            for (int i = 0; i < ClientInfoList.Count; i++)
            {
                ClientInfo _cInfo2 = ClientInfoList[i];
                if (_cInfo2 != _cInfo)
                {
                    _otherUser = true;
                    string _phrase958;
                    if (!Phrases.Dict.TryGetValue(958, out _phrase958))
                    {
                        _phrase958 = "PlayerName = {PlayerName}, # = {Id}.";
                    }
                    _phrase958 = _phrase958.Replace("{PlayerName}", _cInfo2.playerName);
                    _phrase958 = _phrase958.Replace("{Id}", _cInfo2.entityId.ToString());
                    ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase958 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
            if (_otherUser)
            {
                string _phrase778;
                if (!Phrases.Dict.TryGetValue(778, out _phrase778))
                {
                    _phrase778 = "Type {CommandPrivate}{Command67} # to start a vote to mute that player from chat.";
                }
                _phrase778 = _phrase778.Replace("{CommandPrivate}", ChatHook.Command_Private);
                _phrase778 = _phrase778.Replace("{Command67}", Command67);
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase778 + "[-]", _cInfo.entityId, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
            else
            {
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + "No other users were found online" + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }
    }
}
