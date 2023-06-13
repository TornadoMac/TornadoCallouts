﻿using System;
using System.Collections.Generic;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using CalloutInterfaceAPI;
using LSPD_First_Response.Engine;

namespace TornadoCallouts.Callouts
{
    [CalloutInterface("Active Stabbing", CalloutProbability.High, "Someone is actively stabbing people.", "Code 3", "LSPD")]
    public class ActiveStabbing : Callout
    {
        private Ped Suspect1, Suspect2;
        private Blip SuspectBlip1, SuspectBlip2;
        private Vector3 SpawnPoint;
        private bool FightCreated;
        private const float MaxDistance = 6500f; // Approx. 6.5km (4mi) in-game distance
        private Random rand = new Random();

        private List<Vector3> spawnLocations = new List<Vector3>()
        {
            new Vector3(127.44f, -1306.12f, 29.23f), // Vanilla Unicorn Strip Club (Strawberry)
            new Vector3(-1391.837f, -585.1603f, 30.23638f), // Bahama Mamas Bar (Del Perro)
            new Vector3(487.0231f, -1545.364f, 29.19354f), // Hi-Men Bar (Rancho)
            new Vector3(250.0017f, -1011.299f, 29.26846f), // Shenanigan's Bar (Legion Square)
            new Vector3(226.7715f, 301.7152f, 105.5336f), // Singleton's Nightclub (Downtown Vinewood)
            new Vector3(919.417f, 50.9967f, 80.89855f), // Diamond Casino (Los Santos)
        };

        public override bool OnBeforeCalloutDisplayed()
        {
            // Generate a spawn point around the player within a radius of...
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(500f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 50f);
            AddMinimumDistanceCheck(100f, SpawnPoint);

            CalloutMessage = "Active Stabbing";
            CalloutPosition = SpawnPoint;
            LSPD_First_Response.Mod.API.Functions.PlayScannerAudioUsingPosition("CITIZENS_REPORT_04 CRIME_ASSAULT_01 IN_OR_ON_POSITION UNITS_RESPOND_CODE_03_02", SpawnPoint);
            return base.OnBeforeCalloutDisplayed();
        }


        public override bool OnCalloutAccepted()
        {
            CalloutInterfaceAPI.Functions.SendMessage(this, "Citizens are reporting someone is actively stabbing people. EMERGENCY RESPONSE.");

            // List of ped model names
            List<string> pedModels = new List<string>()
            {
                // Male Gang Peds
                "a_m_y_mexthug_01",
                "g_m_importexport_01",
                "g_m_m_korboss_01",
                "g_m_y_ballaorig_01",
                "g_m_y_famdnf_01",
                "g_m_y_mexgoon_01",
                "g_m_y_salvaboss_01",
                "g_m_y_mexgoon_01",
                "g_m_m_chigoon_02",

                // Female Gang Peds
                "g_f_y_ballas_01",
                "g_f_y_lost_01",
                "g_f_y_families_01",
                "g_f_y_vagos_01",
            };

            // Select random models for the suspects
            string model1 = pedModels[rand.Next(pedModels.Count)];
            string model2 = pedModels[rand.Next(pedModels.Count)];

            // Create suspect 1 at the selected location
            Suspect1 = new Ped(model1, SpawnPoint, 180f);
            Suspect1.IsPersistent = true;
            Suspect1.BlockPermanentEvents = true;
            Suspect1.Alertness += 50;
            Suspect1.CanOnlyBeDamagedByPlayer = true;
            Suspect1.Health += 50;

            SuspectBlip1 = Suspect1.AttachBlip();
            SuspectBlip1.Color = System.Drawing.Color.Yellow;
            SuspectBlip1.IsRouteEnabled = true;

            // Create suspect 2 at the selected location
            Suspect2 = new Ped(model2, SpawnPoint, 180f);
            Suspect2.IsPersistent = true;
            Suspect2.BlockPermanentEvents = true;
            Suspect2.Alertness += 50;
            Suspect2.CanOnlyBeDamagedByPlayer = true;
            Suspect2.Health += 50;

            SuspectBlip2 = Suspect2.AttachBlip();
            SuspectBlip2.Color = System.Drawing.Color.Yellow;

            FightCreated = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!FightCreated && Game.LocalPlayer.Character.DistanceTo(Suspect1) <= 60f)
            {
                Suspect1.Tasks.FightAgainst(Suspect2);
                Suspect2.Tasks.FightAgainst(Suspect1);
                FightCreated = true;
            }

            bool v = Suspect1.IsCuffed || Suspect2.IsCuffed; // Suspect 1 or 2 is cuffed.
            bool n = Suspect1.IsCuffed && Suspect2.IsCuffed; // Suspect 1 and 2 are cuffed.
            if (Suspect1.IsDead && Suspect2.IsDead || Game.LocalPlayer.Character.IsDead || !Suspect1.Exists() || !Suspect2.Exists() || v || n)
            {
                Game.DisplayNotification("Callout Ended. ~g~We Are Code 4.");
                End();
            }
        }

        public override void End()
        {
            base.End();

            if (Suspect1.Exists())
            {
                Suspect1.Dismiss();
            }
            if (Suspect2.Exists())
            {
                Suspect2.Dismiss();
            }

            if (SuspectBlip1.Exists())
            {
                SuspectBlip1.Delete();
            }
            if (SuspectBlip2.Exists())
            {
                SuspectBlip2.Delete();
            }

            Game.LogTrivial("TornadoCallouts | Active Stabbing | Has Cleaned Up.");
        }
    }
}
