﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace ServerTools
{
    class BloodmoonWarrior
    {
        public static bool IsEnabled = false, IsRunning = false, BloodmoonStarted = false;
        public static int Zombie_Kills = 10;
        public static List<int> WarriorList = new List<int>();
        public static Dictionary<int, int> KilledZombies = new Dictionary<int, int>();
        private const string file = "BloodmoonWarrior.xml";
        private static string filePath = string.Format("{0}/{1}", API.ConfigPath, file);
        private static Dictionary<string, int[]> Dict = new Dictionary<string, int[]>();
        private static Dictionary<string, string> Dict1 = new Dictionary<string, string>();
        private static FileSystemWatcher fileWatcher = new FileSystemWatcher(API.ConfigPath, file);
        private static System.Random random = new System.Random();
        private static bool updateConfig = false;

        private static List<string> list
        {
            get { return new List<string>(Dict.Keys); }
        }

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
            if (!IsEnabled && IsRunning)
            {
                Dict.Clear();
                Dict1.Clear();
                fileWatcher.Dispose();
                IsRunning = false;
            }
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
                if (childNode.Name == "Items")
                {
                    Dict.Clear();
                    Dict1.Clear();
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Unexpected XML node found in 'Bloodmoon_Warrior.xml' section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("Name"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of missing name attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("SecondaryName"))
                        {
                            updateConfig = true;
                        }
                        if (!_line.HasAttribute("MinCount"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of missing minCount attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("MaxCount"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of missing maxCount attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!_line.HasAttribute("MinQuality"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of missing minQuality attribute: {0}", subChild.OuterXml));
                        }
                        if (!_line.HasAttribute("MaxQuality"))
                        {
                            Log.Warning(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of missing maxQuality attribute: {0}", subChild.OuterXml));
                        }
                        if (!int.TryParse(_line.GetAttribute("MinCount"), out int _minCount))
                        {
                            Log.Out(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of invalid (non-numeric) value for 'minCount' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!int.TryParse(_line.GetAttribute("MaxCount"), out int _maxCount))
                        {
                            Log.Out(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of invalid (non-numeric) value for 'maxCount' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!int.TryParse(_line.GetAttribute("MinQuality"), out int _minQuality))
                        {
                            Log.Out(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of invalid (non-numeric) value for 'minQuality' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        if (!int.TryParse(_line.GetAttribute("MaxQuality"), out int _maxQuality))
                        {
                            Log.Out(string.Format("[SERVERTOOLS] Ignoring BloodmoonWarrior entry because of invalid (non-numeric) value for 'maxQuality' attribute: {0}", subChild.OuterXml));
                            continue;
                        }
                        string _name = _line.GetAttribute("Name");
                        ItemClass _class = ItemClass.GetItemClass(_name, true);
                        Block _block = Block.GetBlockByName(_name, true);
                        if (_class == null && _block == null)
                        {
                            Log.Out(string.Format("[SERVERTOOLS] BloodmoonWarrior entry skipped. Item name not found: {0}", _name));
                            continue;
                        }
                        string _secondaryname;
                        if (_line.HasAttribute("SecondaryName"))
                        {
                            _secondaryname = _line.GetAttribute("SecondaryName");
                        }
                        else
                        {
                            _secondaryname = _name;
                        }
                        if (!Dict.ContainsKey(_name))
                        {
                            int[] _c = new int[] { _minCount, _maxCount, _minQuality, _maxQuality };
                            Dict.Add(_name, _c);
                        }
                        if (!Dict1.ContainsKey(_name))
                        {
                            Dict1.Add(_name, _secondaryname);
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
                sw.WriteLine("<BloodmoonWarrior>");
                sw.WriteLine("    <Items>");
                if (Dict.Count > 0)
                {
                    foreach (KeyValuePair<string, int[]> kvp in Dict)
                    {
                        string _name;
                        if (Dict1.TryGetValue(kvp.Key, out _name))
                        {
                            sw.WriteLine(string.Format("        <Item Name=\"{0}\" SecondaryName=\"{1}\" MinCount=\"{2}\" MaxCount=\"{3}\" MinQuality=\"{4}\" MaxQuality=\"{5}\" />", kvp.Key, _name, kvp.Value[0], kvp.Value[1], kvp.Value[2], kvp.Value[3]));
                        }
                    }
                }
                else
                {
                    sw.WriteLine("        <Item Name=\"drinkJarBoiledWater\" SecondaryName=\"Bottled Water\" MinCount=\"20\" MaxCount=\"30\" MinQuality=\"1\" MaxQuality=\"1\" />");
                    sw.WriteLine("        <Item Name=\"drinkJarBeer\" SecondaryName=\"Beer\" MinCount=\"10\" MaxCount=\"15\" MinQuality=\"1\" MaxQuality=\"1\" />");
                    sw.WriteLine("        <Item Name=\"gunHandgunT2Magnum44\" SecondaryName=\"44 Magnum\" MinCount=\"1\" MaxCount=\"1\" MinQuality=\"3\" MaxQuality=\"5\" />");
                    sw.WriteLine("        <Item Name=\"gunShotgunT2PumpShotgun\" SecondaryName=\"Pump Shotgun\" MinCount=\"1\" MaxCount=\"1\" MinQuality=\"3\" MaxQuality=\"5\" />");
                    sw.WriteLine("        <Item Name=\"gunRifleT3SniperRifle\" SecondaryName=\"Sniper Rifle\" MinCount=\"1\" MaxCount=\"1\" MinQuality=\"2\" MaxQuality=\"5\" />");
                    sw.WriteLine("        <Item Name=\"gunExplosivesT3RocketLauncher\" SecondaryName=\"Rocket Launcher\" MinCount=\"1\" MaxCount=\"1\" MinQuality=\"2\" MaxQuality=\"5\" />");
                    sw.WriteLine("        <Item Name=\"ammoRocketHE\" SecondaryName=\"HE Rocket\" MinCount=\"5\" MaxCount=\"10\" MinQuality=\"1\" MaxQuality=\"1\" />");
                    sw.WriteLine("        <Item Name=\"ammo44MagnumBulletHP\" SecondaryName=\"HP 44 Magnum Ammo\" MinCount=\"25\" MaxCount=\"50\" MinQuality=\"1\" MaxQuality=\"1\" />");
                    sw.WriteLine("        <Item Name=\"ammo762mmBulletHP\" SecondaryName=\"HP AK47 Ammo\" MinCount=\"50\" MaxCount=\"100\" MinQuality=\"1\" MaxQuality=\"1\" />");
                }
                sw.WriteLine("    </Items>");
                sw.WriteLine("</BloodmoonWarrior>");
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

        public static void Exec()
        {
            try
            {
                if (!BloodmoonStarted)
                {
                    if (PersistentOperations.BloodMoonSky())
                    {
                        BloodmoonStarted = true;
                        List<ClientInfo> _cInfoList = PersistentOperations.ClientList();
                        if (_cInfoList != null)
                        {
                            for (int i = 0; i < _cInfoList.Count; i++)
                            {
                                ClientInfo _cInfo = _cInfoList[i];
                                if (_cInfo != null && !string.IsNullOrEmpty(_cInfo.playerId) && _cInfo.entityId > 0)
                                {
                                    if (GameManager.Instance.World.Players.dict.ContainsKey(_cInfo.entityId))
                                    {
                                        EntityAlive _player = (EntityAlive)PersistentOperations.GetEntity(_cInfo.entityId);
                                        if (_player != null && _player.IsSpawned() && _player.IsAlive() && _player.Died > 0 && _player.Progression.GetLevel() >= 10 && random.Next(1, 11) < 6)
                                        {
                                            WarriorList.Add(_cInfo.entityId);
                                            KilledZombies.Add(_cInfo.entityId, 0);
                                            string _response = "Hades has called upon you. Survive this night and kill {ZombieCount} zombies to be rewarded by the king of the underworld.";
                                            _response = _response.Replace("{ZombieCount}", Zombie_Kills.ToString());
                                            ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _response + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (!PersistentOperations.BloodMoonSky())
                {
                    BloodmoonStarted = false;
                    RewardWarriors();
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in BloodmoonWarrior.Exec: {0}", e.Message));
            }
        }

        public static void RewardWarriors()
        {
            try
            {
                List<int> _warriors = WarriorList;
                for (int i = 0; i < _warriors.Count; i++)
                {
                    int _warrior = _warriors[i];
                    if (GameManager.Instance.World.Players.dict.ContainsKey(_warrior))
                    {
                        EntityAlive _player = (EntityAlive)PersistentOperations.GetEntity(_warrior);
                        if (_player != null && _player.IsAlive())
                        {
                            int _killedZ;
                            KilledZombies.TryGetValue(_warrior, out _killedZ);
                            if (_killedZ >= Zombie_Kills)
                            {
                                ClientInfo _cInfo = PersistentOperations.GetClientInfoFromEntityId(_warrior);
                                if (_cInfo != null)
                                {
                                    int _deathCount = _player.Died - 1;
                                    _player.Died = _deathCount;
                                    _player.bPlayerStatsChanged = true;
                                    _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(_player));
                                    ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + "You have survived and been rewarded by hades himself. Your death count was reduced by one" + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                                    RandomItem(_cInfo);
                                }
                            }
                        }
                    }
                }
                WarriorList.Clear();
                KilledZombies.Clear();
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in BloodmoonWarrior.RewardWarriors: {0}", e.Message));
            }
        }

        private static void RandomItem(ClientInfo _cInfo)
        {
            try
            {
                string _randomItem = list.RandomObject();
                ItemValue _itemValue = new ItemValue(ItemClass.GetItem(_randomItem, false).type, false);
                int[] _itemData;
                if (Dict.TryGetValue(_randomItem, out _itemData))
                {
                    int _count = 0;
                    if (_itemData[0] > _itemData[1])
                    {
                        _count = random.Next(_itemData[1], _itemData[0] + 1);
                    }
                    else
                    {
                        _count = random.Next(_itemData[0], _itemData[1] + 1);
                    }
                    if (_itemValue.HasQuality && _itemData[2] > 0 && _itemData[3] >= _itemData[2])
                    {
                        _itemValue.Quality = random.Next(_itemData[2], _itemData[3] + 1);
                    }
                    World world = GameManager.Instance.World;
                    var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                    {
                        entityClass = EntityClass.FromString("item"),
                        id = EntityFactory.nextEntityID++,
                        itemStack = new ItemStack(_itemValue, _count),
                        pos = world.Players.dict[_cInfo.entityId].position,
                        rot = new Vector3(20f, 0f, 20f),
                        lifetime = 60f,
                        belongsPlayerId = _cInfo.entityId
                    });
                    world.SpawnEntityInWorld(entityItem);
                    _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _cInfo.entityId));
                    world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
                    string _message = "Received {ItemCount} {ItemName} from Hades.";
                    _message = _message.Replace("{ItemCount}", _count.ToString());
                    string _name;
                    Dict1.TryGetValue(_randomItem, out _name);
                    _message = _message.Replace("{ItemName}", _name);
                    ChatHook.ChatMessage(_cInfo, LoadConfig.Chat_Response_Color + _message + "[-]", -1, LoadConfig.Server_Response_Name, EChatType.Whisper, null);
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in BloodmoonWarrior.RandomItem: {0}", e.Message));
            }
        }
    }
}
