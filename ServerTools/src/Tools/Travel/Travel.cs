﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using UnityEngine;

namespace ServerTools
{
    class Travel
    {
        public static bool IsEnabled = false, IsRunning = false, PvP_Check = false, Zombie_Check = false;
        public static int Delay_Between_Uses = 60, Command_Cost = 0;
        public static string Command49 = "travel";
        private const string file = "TravelLocations.xml";
        private static string filePath = string.Format("{0}/{1}", API.ConfigPath, file);
        private static SortedDictionary<string, string[]> Box = new SortedDictionary<string, string[]>();
        private static SortedDictionary<string, string> Destination = new SortedDictionary<string, string>();
        public static List<int> Flag = new List<int>();
        private static FileSystemWatcher fileWatcher = new FileSystemWatcher(API.ConfigPath, file);
        private static bool updateConfig = false;
        private static int _xMinCheck = 0, _yMinCheck = 0, _zMinCheck = 0, _xMaxCheck = 0, _yMaxCheck = 0, _zMaxCheck = 0;

        public static void Load()
        {
            if (IsEnabled && !IsRunning)
            {
                LoadXml();
                InitFileWatcher();
            }
        }

        public static void Unload()
        {
            fileWatcher.Dispose();
            IsRunning = false;
        }

        public static void LoadXml()
        {
            if (!Utils.FileExists(filePath))
            {
                UpdateXml();
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(filePath);
            }
            catch (XmlException e)
            {
                Log.Error(string.Format("[SERVERTOOLS] Failed loading {0}: {1}", file, e.Message));
                return;
            }
            XmlNode _XmlNode = xmlDoc.DocumentElement;
            foreach (XmlNode childNode in _XmlNode.ChildNodes)
            {
                if (childNode.Name == "Travel")
                {
                    Box.Clear();
                    Destination.Clear();
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'TravelLocations' section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("Name"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of missing Name attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("Corner1"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of missing Corner1 attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("Corner2"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of missing Corner2 attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("Destination"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring Travel entry because of missing Destination attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        string _name = _line.GetAttribute("Name");
                        string[] box = { _line.GetAttribute("Corner1"), _line.GetAttribute("Corner2"), _line.GetAttribute("Destination") };

                        if (!Box.ContainsKey(_name))
                        {
                            Box.Add(_name, box);
                        }
                    }
                }
            }
            if (updateConfig)
            {
                updateConfig = false;
                UpdateXml();
            }
        }

        private static void UpdateXml()
        {
            fileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<TravelLocations>");
                sw.WriteLine("    <Travel>");
                if (Box.Count > 0)
                {
                    foreach (KeyValuePair<string, string[]> kvpBox in Box)
                    {
                        sw.WriteLine(string.Format("        <Location Name=\"{0}\" Corner1=\"{1}\" Corner2=\"{2}\" Destination=\"{3}\" />", kvpBox.Key, kvpBox.Value[0], kvpBox.Value[1], kvpBox.Value[2]));
                    }
                }
                else
                {
                    sw.WriteLine("        <Location Name=\"zone1\" Corner1=\"0,100,0\" Corner2=\"10,100,10\" Destination=\"-100,-1,-100\" />");
                    sw.WriteLine("        <Location Name=\"zone2\" Corner1=\"-1,100,-1\" Corner2=\"-10,100,-10\" Destination=\"100,-1,100\" />");
                }
                sw.WriteLine("    </Travel>");
                sw.WriteLine("</TravelLocations>");
                sw.Flush();
                sw.Close();
            }
            fileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.EnableRaisingEvents = true;
            IsRunning = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!Utils.FileExists(filePath))
            {
                UpdateXml();
            }
            LoadXml();
        }

        public static void Exec(ClientInfo _cInfo)
        {
            if (Delay_Between_Uses < 1)
            {
                if (Wallet.IsEnabled && Command_Cost >= 1)
                {
                    CommandCost(_cInfo);
                }
                else
                {
                    Tele(_cInfo);
                }
            }
            else
            {
                DateTime _lastTravel = PersistentContainer.Instance.Players[_cInfo.playerId].LastTravel;
                TimeSpan varTime = DateTime.Now - _lastTravel;
                double fractionalMinutes = varTime.TotalMinutes;
                int _timepassed = (int)fractionalMinutes;
                if (ReservedSlots.IsEnabled && ReservedSlots.Reduced_Delay)
                {
                    if (ReservedSlots.Dict.ContainsKey(_cInfo.playerId))
                    {
                        DateTime _dt;
                        ReservedSlots.Dict.TryGetValue(_cInfo.playerId, out _dt);
                        if (DateTime.Now < _dt)
                        {
                            int _delay = Delay_Between_Uses / 2;
                            Time(_cInfo, _timepassed, _delay);
                            return;
                        }
                    }
                }
                Time(_cInfo, _timepassed, Delay_Between_Uses);
            }
        }

        public static void Time(ClientInfo _cInfo, int _timepassed, int _delay)
        {
            if (_timepassed >= _delay)
            {
                if (Wallet.IsEnabled && Command_Cost >= 1)
                {
                    CommandCost(_cInfo);
                }
                else
                {
                    Tele(_cInfo);
                }
            }
            else
            {
                int _timeleft = _delay - _timepassed;
                string _phrase605;
                if (!Phrases.Dict.TryGetValue(605, out _phrase605))
                {
                    _phrase605 = "You can only use {CommandPrivate}{Command49} once every {DelayBetweenUses} minutes. Time remaining: {TimeRemaining} minutes.";
                }
                _phrase605 = _phrase605.Replace("{DelayBetweenUses}", _delay.ToString());
                _phrase605 = _phrase605.Replace("{TimeRemaining}", _timeleft.ToString());
                _phrase605 = _phrase605.Replace("{CommandPrivate}", ChatHook.Command_Private);
                _phrase605 = _phrase605.Replace("{Command49}", Command49);
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase605 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void CommandCost(ClientInfo _cInfo)
        {
            if (Wallet.GetCurrentCoins(_cInfo.playerId) >= Command_Cost)
            {
                Tele(_cInfo);
            }
            else
            {
                string _phrase814;
                if (!Phrases.Dict.TryGetValue(814, out _phrase814))
                {
                    _phrase814 = "You do not have enough {WalletCoinName} in your wallet to run this command.";
                }
                _phrase814 = _phrase814.Replace("{WalletCoinName}", Wallet.Coin_Name);
                ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase814 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
            }
        }

        public static void Tele(ClientInfo _cInfo)
        {
            EntityPlayer _player = GameManager.Instance.World.Players.dict[_cInfo.entityId];
            Flag.Clear();
            int _playerX = (int)_player.position.x;
            int _playerY = (int)_player.position.y;
            int _playerZ = (int)_player.position.z;
            if (Box.Count > 0)
            {
                foreach (KeyValuePair<string, string[]> kvpCorners in Box)
                {
                    string[] _xyzCorner1 = { };
                    if (kvpCorners.Value[0].Contains(","))
                    {
                        _xyzCorner1 = kvpCorners.Value[0].Split(',');
                    }
                    else
                    {
                        _xyzCorner1 = kvpCorners.Value[0].Split(' ');
                    }
                    int xMin, yMin, zMin;
                    int.TryParse(_xyzCorner1[0], out xMin);
                    int.TryParse(_xyzCorner1[1], out yMin);
                    int.TryParse(_xyzCorner1[2], out zMin);
                    string[] _xyzCorner2 = { };
                    if (kvpCorners.Value[1].Contains(","))
                    {
                        _xyzCorner2 = kvpCorners.Value[1].Split(',');
                    }
                    else
                    {
                        _xyzCorner2 = kvpCorners.Value[1].Split(' ');
                    }
                    int xMax, yMax, zMax;
                    int.TryParse(_xyzCorner2[0], out xMax);
                    int.TryParse(_xyzCorner2[1], out yMax);
                    int.TryParse(_xyzCorner2[2], out zMax);
                    string[] _xyzDestCords = { };
                    if (kvpCorners.Value[2].Contains(","))
                    {
                        _xyzDestCords = kvpCorners.Value[2].Split(',');
                    }
                    else
                    {
                        _xyzDestCords = kvpCorners.Value[2].Split(' ');
                    }
                    int xDest, yDest, zDest;
                    int.TryParse(_xyzDestCords[0], out xDest);
                    int.TryParse(_xyzDestCords[1], out yDest);
                    int.TryParse(_xyzDestCords[2], out zDest);

                    if (xMin >= 0 & xMax >= 0)
                    {
                        if (xMin < xMax)
                        {
                            if (_playerX >= xMin)
                            {
                                _xMinCheck = 1;
                            }
                            else
                            {
                                _xMinCheck = 0;
                            }
                            if (_playerX <= xMax)
                            {
                                _xMaxCheck = 1;
                            }
                            else
                            {
                                _xMaxCheck = 0;
                            }
                        }
                        else
                        {
                            if (_playerX <= xMin)
                            {
                                _xMinCheck = 1;
                            }
                            else
                            {
                                _xMinCheck = 0;
                            }
                            if (_playerX >= xMax)
                            {
                                _xMaxCheck = 1;
                            }
                            else
                            {
                                _xMaxCheck = 0;
                            }
                        }
                    }
                    else if (xMin <= 0 & xMax <= 0)
                    {
                        if (xMin < xMax)
                        {
                            if (_playerX >= xMin)
                            {
                                _xMinCheck = 1;
                            }
                            else
                            {
                                _xMinCheck = 0;
                            }
                            if (_playerX <= xMax)
                            {
                                _xMaxCheck = 1;
                            }
                            else
                            {
                                _xMaxCheck = 0;
                            }
                        }
                        else
                        {
                            if (_playerX <= xMin)
                            {
                                _xMinCheck = 1;
                            }
                            else
                            {
                                _xMinCheck = 0;
                            }
                            if (_playerX >= xMax)
                            {
                                _xMaxCheck = 1;
                            }
                            else
                            {
                                _xMaxCheck = 0;
                            }
                        }
                    }
                    else if (xMin <= 0 & xMax >= 0)
                    {
                        if (_playerX >= xMin)
                        {
                            _xMinCheck = 1;
                        }
                        else
                        {
                            _xMinCheck = 0;
                        }
                        if (_playerX <= xMax)
                        {
                            _xMaxCheck = 1;
                        }
                        else
                        {
                            _xMaxCheck = 0;
                        }
                    }
                    else if (xMin >= 0 & xMax <= 0)
                    {
                        if (_playerX <= xMin)
                        {
                            _xMinCheck = 1;
                        }
                        else
                        {
                            _xMinCheck = 0;
                        }
                        if (_playerX >= xMax)
                        {
                            _xMaxCheck = 1;
                        }
                        else
                        {
                            _xMaxCheck = 0;
                        }
                    }

                    if (yMin >= 0 & yMax >= 0)
                    {
                        if (yMin < yMax)
                        {
                            if (_playerY >= yMin)
                            {
                                _yMinCheck = 1;
                            }
                            else
                            {
                                _yMinCheck = 0;
                            }
                            if (_playerY <= yMax)
                            {
                                _yMaxCheck = 1;
                            }
                            else
                            {
                                _yMaxCheck = 0;
                            }
                        }
                        else
                        {
                            if (_playerY <= yMin)
                            {
                                _yMinCheck = 1;
                            }
                            else
                            {
                                _yMinCheck = 0;
                            }
                            if (_playerY >= yMax)
                            {
                                _yMaxCheck = 1;
                            }
                            else
                            {
                                _yMaxCheck = 0;
                            }
                        }
                    }
                    else if (yMin <= 0 & yMax <= 0)
                    {
                        if (yMin < yMax)
                        {
                            if (_playerY >= yMin)
                            {
                                _yMinCheck = 1;
                            }
                            else
                            {
                                _yMinCheck = 0;
                            }
                            if (_playerY <= yMax)
                            {
                                _yMaxCheck = 1;
                            }
                            else
                            {
                                _yMaxCheck = 0;
                            }
                        }
                        else
                        {
                            if (_playerY <= yMin)
                            {
                                _yMinCheck = 1;
                            }
                            else
                            {
                                _yMinCheck = 0;
                            }
                            if (_playerY >= yMax)
                            {
                                _yMaxCheck = 1;
                            }
                            else
                            {
                                _yMaxCheck = 0;
                            }
                        }
                    }
                    else if (yMin <= 0 & yMax >= 0)
                    {
                        if (_playerY >= yMin)
                        {
                            _yMinCheck = 1;
                        }
                        else
                        {
                            _yMinCheck = 0;
                        }
                        if (_playerY <= yMax)
                        {
                            _yMaxCheck = 1;
                        }
                        else
                        {
                            _yMaxCheck = 0;
                        }
                    }
                    else if (yMin >= 0 & yMax <= 0)
                    {
                        if (_playerY <= yMin)
                        {
                            _yMinCheck = 1;
                        }
                        else
                        {
                            _yMinCheck = 0;
                        }
                        if (_playerY >= yMax)
                        {
                            _yMaxCheck = 1;
                        }
                        else
                        {
                            _yMaxCheck = 0;
                        }
                    }

                    if (zMin >= 0 & zMax >= 0)
                    {
                        if (zMin < zMax)
                        {
                            if (_playerZ >= zMin)
                            {
                                _zMinCheck = 1;
                            }
                            else
                            {
                                _zMinCheck = 0;
                            }
                            if (_playerZ <= zMax)
                            {
                                _zMaxCheck = 1;
                            }
                            else
                            {
                                _zMaxCheck = 0;
                            }
                        }
                        else
                        {
                            if (_playerZ <= zMin)
                            {
                                _zMinCheck = 1;
                            }
                            else
                            {
                                _zMinCheck = 0;
                            }
                            if (_playerZ >= zMax)
                            {
                                _zMaxCheck = 1;
                            }
                            else
                            {
                                _zMaxCheck = 0;
                            }
                        }
                    }
                    else if (zMin <= 0 & zMax <= 0)
                    {
                        if (zMin < zMax)
                        {
                            if (_playerZ >= zMin)
                            {
                                _zMinCheck = 1;
                            }
                            else
                            {
                                _zMinCheck = 0;
                            }
                            if (_playerZ <= zMax)
                            {
                                _zMaxCheck = 1;
                            }
                            else
                            {
                                _zMaxCheck = 0;
                            }
                        }
                        else
                        {
                            if (_playerZ <= zMin)
                            {
                                _zMinCheck = 1;
                            }
                            else
                            {
                                _zMinCheck = 0;
                            }
                            if (_playerZ >= zMax)
                            {
                                _zMaxCheck = 1;
                            }
                            else
                            {
                                _zMaxCheck = 0;
                            }
                        }
                    }
                    else if (zMin <= 0 & zMax >= 0)
                    {
                        if (_playerZ >= zMin)
                        {
                            _zMinCheck = 1;
                        }
                        else
                        {
                            _zMinCheck = 0;
                        }
                        if (_playerZ <= zMax)
                        {
                            _zMaxCheck = 1;
                        }
                        else
                        {
                            _zMaxCheck = 0;
                        }
                    }
                    else if (zMin >= 0 & zMax <= 0)
                    {
                        if (_playerY <= zMin)
                        {
                            _zMinCheck = 1;
                        }
                        else
                        {
                            _zMinCheck = 0;
                        }
                        if (_playerY >= zMax)
                        {
                            _zMaxCheck = 1;
                        }
                        else
                        {
                            _zMaxCheck = 0;
                        }
                    }
                    if (_xMinCheck == 1 & _yMinCheck == 1 & _zMinCheck == 1 & _xMaxCheck == 1 & _yMaxCheck == 1 & _zMaxCheck == 1)
                    {
                        if (PvP_Check)
                        {
                            if (Teleportation.PCheck(_cInfo, _player))
                            {
                                return;
                            }
                        }
                        if (Zombie_Check)
                        {
                            if (Teleportation.ZCheck(_cInfo, _player))
                            {
                                return;
                            }
                        }
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(new Vector3(xDest, yDest, zDest), null, false));
                        if (Wallet.IsEnabled && Command_Cost >= 1)
                        {
                            Wallet.SubtractCoinsFromWallet(_cInfo.playerId, Command_Cost);
                        }
                        PersistentContainer.Instance.Players[_cInfo.playerId].LastTravel = DateTime.Now;
                        PersistentContainer.Instance.Save();
                        string _phrase603;
                        if (!Phrases.Dict.TryGetValue(603, out _phrase603))
                        {
                            _phrase603 = "You have traveled to {Destination}.";
                        }
                        _phrase603 = _phrase603.Replace("{Destination}", kvpCorners.Key);
                        ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase603 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                    }
                    else
                    {
                        Flag.Add(_cInfo.entityId + 1);
                        if (Flag.Count == Box.Count)
                        {
                            string _phrase604;
                            if (!Phrases.Dict.TryGetValue(604, out _phrase604))
                            {
                                _phrase604 = "You are not in a travel location.";
                            }
                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _phrase604 + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                        }
                    }
                }
            }
        }
    }
}