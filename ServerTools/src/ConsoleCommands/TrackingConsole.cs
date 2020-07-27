﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ServerTools
{
    class TrackingConsole : ConsoleCmdAbstract
    {
        public override string GetDescription()
        {
            return "[ServerTools] - Enable or disable tracking. Check if players have been tracked with in the specified range.";
        }

        public override string GetHelp()
        {
            return "Usage:\n" +
                   "  1. Tracking off\n" +
                   "  2. Tracking on\n" +
                   "  3. Tracking <Hours> <Block Range>\n" +
                   "  4. Tracking <Hours> <Block Range> <X> <Y> <Z>\n" +
                   "1. Turn off the tracking tool\n" +
                   "2. Turn on the tracking tool\n" +
                   "3. List players tracked with in the specified hours and block range from your current location\n" +
                   "4. List players tracked with in the specified hours and block range from a specific location\n";
        }

        public override string[] GetCommands()
        {
            return new string[] { "st-Tracking", "track", "st-track" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 1 || _params.Count == 3 || _params.Count == 4 || _params.Count > 5)
                {
                    SdtdConsole.Instance.Output(string.Format("Wrong number of arguments, expected 1, 2 or 5, found {0}", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    if (Track.IsEnabled)
                    {
                        Track.IsEnabled = false;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Tracking has been set to off"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Tracking is already off"));
                        return;
                    }
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    if (!Track.IsEnabled)
                    {
                        Track.IsEnabled = true;
                        LoadConfig.WriteXml();
                        SdtdConsole.Instance.Output(string.Format("Tracking has been set to on"));
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output(string.Format("Tracking is already on"));
                        return;
                    }
                }
                else if (_params.Count == 2)
                {
                    if (PersistentContainer.Instance.Track != null && PersistentContainer.Instance.Track.Count > 0)
                    {
                        bool _found = false;
                        int _hours, _range;
                        if (int.TryParse(_params[0], out _hours))
                        {
                            if (_hours > 48)
                            {
                                _hours = 48;
                            }
                            if (int.TryParse(_params[1], out _range))
                            {
                                EntityPlayer _player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
                                if (_player != null)
                                {
                                    List<string[]> _tracking = PersistentContainer.Instance.Track;
                                    for (int i = 0; i < _tracking.Count; i++)
                                    {
                                        string[] _trackData = _tracking[i];
                                        DateTime.TryParse(_trackData[0], out DateTime _date);
                                        if (_date.AddHours(_hours) >= DateTime.Now)
                                        {
                                            string[] _cords = _trackData[1].Split(',');
                                            int.TryParse(_cords[0], out int _x);
                                            int.TryParse(_cords[1], out int _y);
                                            int.TryParse(_cords[2], out int _z);
                                            Vector3 _trackedVecPos = new Vector3(_x, _y, _z);
                                            if (RangeCheck(_player.position, _trackedVecPos, _range))
                                            {
                                                SdtdConsole.Instance.Output(string.Format("Player: {0}, SteamId: {1}, Time: {2}, Position: {3}, Item Held: {4}", _trackData[3], _trackData[2], _trackData[0], _trackData[1], _trackData[4]));
                                            }
                                        }
                                    }
                                }
                                if (!_found)
                                {
                                    SdtdConsole.Instance.Output("Tracking log lists nobody at this time and range from your current position");
                                }
                                return;
                            }
                            else
                            {
                                SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[1]));
                                return;
                            }
                        }
                        else
                        {
                            SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[0]));
                            return;
                        }
                    }
                    else
                    {
                        SdtdConsole.Instance.Output("Tracking log has no data");
                        return;
                    }
                }
                else if (_params.Count == 5)
                {
                    if (PersistentContainer.Instance.Track != null && PersistentContainer.Instance.Track.Count > 0)
                    {
                        bool _found = false;
                        if (int.TryParse(_params[0], out int _hours))
                        {
                            if (int.TryParse(_params[1], out int _range))
                            {
                                if (int.TryParse(_params[2], out int _worldX))
                                {
                                    if (int.TryParse(_params[3], out int _worldY))
                                    {
                                        if (int.TryParse(_params[4], out int _worldZ))
                                        {
                                            EntityPlayer _player = GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
                                            if (_player != null)
                                            {
                                                List<string[]> _tracking = PersistentContainer.Instance.Track;
                                                for (int i = 0; i < _tracking.Count; i++)
                                                {
                                                    string[] _trackData = _tracking[i];
                                                    DateTime.TryParse(_trackData[0], out DateTime _date);
                                                    if (_date.AddHours(_hours) >= DateTime.Now)
                                                    {
                                                        Vector3 _trackedVecPos = new Vector3(_worldX, _worldY, _worldZ);
                                                        if (RangeCheck(_player.position, _trackedVecPos, _range))
                                                        {
                                                            SdtdConsole.Instance.Output(string.Format("Player: {0}, SteamId: {1}, Time: {2}, Position: {3}, Item Held: {4}", _trackData[3], _trackData[2], _trackData[0], _trackData[1], _trackData[4]));
                                                        }
                                                    }
                                                }
                                            }
                                            if (!_found)
                                            {
                                                SdtdConsole.Instance.Output(string.Format("Tracking results found nobody at this time and range inside the specified position"));
                                            }
                                        }
                                        else
                                        {
                                            SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[4]));
                                        }
                                    }
                                    else
                                    {
                                        SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[3]));
                                    }
                                }
                                else
                                {
                                    SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[2]));
                                }
                            }
                            else
                            {
                                SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[1]));
                            }
                        }
                        else
                        {
                            SdtdConsole.Instance.Output(string.Format("Invalid argument {0}", _params[0]));
                        }
                        return;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output("Tracking log has no data");
                        return;
                    }
                }
                else
                {
                    SdtdConsole.Instance.Output("Invalid arguments");
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in TrackingConsole.Execute: {0}", e.Message));
            }
        }

        public static bool RangeCheck(Vector3 _playerX, Vector3 _trackedVecPos, int _range)
        {
            int _distance = (int)Vector3.Distance(_playerX, _trackedVecPos);
            if (_distance <= _range)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
