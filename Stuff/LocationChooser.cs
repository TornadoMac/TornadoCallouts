﻿using Rage;
using System.Collections.Generic;

namespace TornadoCallouts.Stuff
{
    public static class LocationChooser
    {

        public static Vector3 ChooseNearestLocation(List<Vector3> list)
        {
            Vector3 closestLocation = list[0];
            float closestDistance = Vector3.Distance(Game.LocalPlayer.Character.Position, list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                if (Vector3.Distance(Game.LocalPlayer.Character.Position, list[i]) <= closestDistance)
                {
                    closestDistance = Vector3.Distance(Game.LocalPlayer.Character.Position, list[i]);
                    closestLocation = list[i];
                }
            }
            return closestLocation;
        }
        public static int NearestLocationIndex(List<Vector3> list)
        {
            int closestLocationIndex = 0;
            float closestDistance = Vector3.Distance(Game.LocalPlayer.Character.Position, list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                if (Vector3.Distance(Game.LocalPlayer.Character.Position, list[i]) <= closestDistance)
                {
                    closestDistance = Vector3.Distance(Game.LocalPlayer.Character.Position, list[i]);
                    closestLocationIndex = i;
                }
            }
            return closestLocationIndex;
        }
    }
}
