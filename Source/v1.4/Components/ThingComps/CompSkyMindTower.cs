﻿using Verse;
using RimWorld;
using System.Text;

namespace SkyMind
{
    public class CompSkyMindTower : ThingComp
    {
        public CompProperties_SkyMindTower Props
        {
            get
            {
                return (CompProperties_SkyMindTower)props;
            }
        }

        // After despawning remove the tower.
        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            // The tower capacity should only be removed for towers that do not have a power supply (and are thus always on) or for towers that are currently on.
            CompPowerTrader cpt = parent.GetComp<CompPowerTrader>();
            if (parent is Building && cpt?.PowerOn != false)
            {
                SMN_Utils.gameComp.RemoveTower(this);
            }
        }

        // If the map the tower is on is lost, lose the tower's capacity if it was online.
        public override void Notify_MapRemoved()
        {
            base.Notify_MapRemoved();
            CompPowerTrader cpt = parent.GetComp<CompPowerTrader>();
            if (parent is Building && cpt?.PowerOn != false)
            {
                SMN_Utils.gameComp.RemoveTower(this);
            }
        }

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);

            switch (signal)
            {
                case "PowerTurnedOn":
                    SMN_Utils.gameComp.AddTower(this);
                    break;
                case "PowerTurnedOff":
                    SMN_Utils.gameComp.RemoveTower(this);
                    break;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder ret = new StringBuilder();

            if (parent.Map == null)
                return base.CompInspectStringExtra();

            ret.Append("SMN_SkyMindNetworkSummary".Translate(SMN_Utils.gameComp.GetSkyMindDevices().Count, SMN_Utils.gameComp.GetSkyMindNetworkSlots()));

            return ret.Append(base.CompInspectStringExtra()).ToString();
        }


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            // No need to handle anything upon loading a save - capacity is saved in the GameComponent and we should avoid adding extra capacity.
            if (respawningAfterLoad)
                return;

            // If there is no power supply to this server, it can't be turned on/off normally. Just add it in and handle removing it separately.
            if (parent.GetComp<CompPowerTrader>() == null)
            {
                SMN_Utils.gameComp.AddTower(this);
            }
        }
    }
}