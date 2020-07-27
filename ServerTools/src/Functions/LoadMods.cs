﻿using ServerTools.AntiCheat;
using ServerTools.Website;

namespace ServerTools
{
    public class Mods
    {
        public static bool Startup = false;

        public static void Load()
        {
            Confirm.Exec();
            PatchTools.ApplyPatches();
            if (!Timers.IsRunning)
            {
                Timers.TimerStart();
            }
            if (!LoadTriggers.IsRunning)
            {
                LoadTriggers.Load();
            }
            if (Poll.IsEnabled && PersistentContainer.Instance.PollOpen)
            {
                Poll.CheckTime();
            }
            if (!ClanManager.IsEnabled)
            {
                ClanManager.Clans.Clear();
                ClanManager.ClanMember.Clear();
            }
            if (!InfoTicker.IsEnabled && InfoTicker.IsRunning)
            {
                InfoTicker.Unload();
            }
            if (InfoTicker.IsEnabled && !InfoTicker.IsRunning)
            {
                InfoTicker.Load();
            }
            if (Gimme.IsRunning && !Gimme.IsEnabled)
            {
                Gimme.Unload();
            }
            if (!Gimme.IsRunning && Gimme.IsEnabled)
            {
                Gimme.Load();
            }
            if (Badwords.IsRunning && !Badwords.IsEnabled)
            {
                Badwords.Unload();
            }
            if (!Badwords.IsRunning && Badwords.IsEnabled)
            {
                Badwords.Load();
            }
            if (!LoginNotice.IsRunning && LoginNotice.IsEnabled)
            {
                LoginNotice.Load();
            }
            if (LoginNotice.IsRunning && !LoginNotice.IsEnabled)
            {
                LoginNotice.Unload();
            }
            if (!Zones.IsRunning && Zones.IsEnabled)
            {
                Zones.Load();
            }
            if (Zones.IsRunning && !Zones.IsEnabled)
            {
                Zones.Unload();
            }
            if (!VoteReward.IsRunning && VoteReward.IsEnabled)
            {
                VoteReward.Load();
            }
            if (VoteReward.IsRunning && !VoteReward.IsEnabled)
            {
                VoteReward.Unload();
            }
            if (!Watchlist.IsRunning && Watchlist.IsEnabled)
            {
                Watchlist.Load();
            }
            if (Watchlist.IsRunning && !Watchlist.IsEnabled)
            {
                Watchlist.Unload();
            }
            if (!ReservedSlots.IsRunning && ReservedSlots.IsEnabled)
            {
                ReservedSlots.Load();
            }
            if (ReservedSlots.IsRunning && !ReservedSlots.IsEnabled)
            {
                ReservedSlots.Unload();
            }
            if (!StartingItems.IsRunning && StartingItems.IsEnabled)
            {
                StartingItems.Load();
            }
            if (StartingItems.IsRunning && !StartingItems.IsEnabled)
            {
                StartingItems.Unload();
            }
            if (!Travel.IsRunning && Travel.IsEnabled)
            {
                Travel.Load();
            }
            if (Travel.IsRunning && !Travel.IsEnabled)
            {
                Travel.Unload();
            }
            if (!Shop.IsRunning && Shop.IsEnabled)
            {
                Shop.Load();
            }
            if (Shop.IsRunning && !Shop.IsEnabled)
            {
                Shop.Unload();
            }
            if (!Motd.IsRunning && Motd.IsEnabled)
            {
                Motd.Load();
            }
            if (Motd.IsRunning && !Motd.IsEnabled)
            {
                Motd.Unload();
            }
            if (InvalidItems.IsRunning && !InvalidItems.IsEnabled)
            {
                InvalidItems.Unload();
            }
            if (!InvalidItems.IsRunning && InvalidItems.IsEnabled)
            {
                InvalidItems.Load();
            }
            if (HighPingKicker.IsRunning && !HighPingKicker.IsEnabled)
            {
                HighPingKicker.Unload();
            }
            if (!HighPingKicker.IsRunning && HighPingKicker.IsEnabled)
            {
                HighPingKicker.Load();
            }
            if (CredentialCheck.IsRunning && !CredentialCheck.IsEnabled)
            {
                CredentialCheck.Unload();
            }
            if (!CredentialCheck.IsRunning && CredentialCheck.IsEnabled)
            {
                CredentialCheck.Load();
            }
            if (CustomCommands.IsRunning && !CustomCommands.IsEnabled)
            {
                CustomCommands.Unload();
            }
            if (!CustomCommands.IsRunning && CustomCommands.IsEnabled)
            {
                CustomCommands.Load();
            }
            if (DupeLog.IsRunning && !DupeLog.IsEnabled)
            {
                DupeLog.Unload();
            }
            if (!DupeLog.IsRunning && DupeLog.IsEnabled)
            {
                DupeLog.Load();
            }
            if (ChatColorPrefix.IsRunning && !ChatColorPrefix.IsEnabled)
            {
                ChatColorPrefix.Unload();
            }
            if (!ChatColorPrefix.IsRunning && ChatColorPrefix.IsEnabled)
            {
                ChatColorPrefix.Load();
            }
            if (KillNotice.IsRunning && !KillNotice.IsEnabled)
            {
                KillNotice.Unload();
            }
            if (!KillNotice.IsRunning && KillNotice.IsEnabled)
            {
                KillNotice.Load();
            }
            if (Prayer.IsRunning && !Prayer.IsEnabled)
            {
                Prayer.Unload();
            }
            if (!Prayer.IsRunning && Prayer.IsEnabled)
            {
                Prayer.Load();
            }
            if (BloodmoonWarrior.IsRunning && !BloodmoonWarrior.IsEnabled)
            {
                BloodmoonWarrior.Unload();
            }
            if (!BloodmoonWarrior.IsRunning && BloodmoonWarrior.IsEnabled)
            {
                BloodmoonWarrior.Load();
            }
            if (ProtectedSpaces.IsRunning && !ProtectedSpaces.IsEnabled)
            {
                ProtectedSpaces.Unload();
            }
            if (!ProtectedSpaces.IsRunning && ProtectedSpaces.IsEnabled)
            {
                ProtectedSpaces.Load();
            }
            if (ClanManager.IsEnabled)
            {
                ClanManager.ClanList();
            }
            if (AuctionBox.IsEnabled)
            {
                AuctionBox.AuctionList();
            }
            if (Mute.IsEnabled)
            {
                Mute.ClientMuteList();
                Mute.MuteList();
            }
            if (Jail.IsEnabled)
            {
                Jail.JailList();
            }
            //always load the website last
            if (WebsiteServer.IsEnabled && !WebsiteServer.DirFound)
            {
                WebsiteServer.CheckDir();
            }
            if (WebsiteServer.IsRunning && !WebsiteServer.IsEnabled)
            {
                WebsiteServer.Unload();
            }
            if (!WebsiteServer.IsRunning && WebsiteServer.IsEnabled && WebsiteServer.DirFound)
            {
                WebsiteServer.Load();
            }
        }
    }
}