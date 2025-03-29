using System;
using AltV.Net;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;
using System.Numerics;

public class FishingResource : Resource
{
    private IColShape? fishingZone;
        public override void OnStart()
    {
        Alt.Log("🎣 SimpleFishing gestartet!");
        Alt.Log("🎣 Creating Fishing Zone!");
        fishingZone = Alt.CreateColShapeSphere(new Vector3(318, -2237, 5), 50f);
        Alt.Log("🎣 Fishing Zone Created!");
        fishingZone.SetData("FishingZone", true);
    }

    public override void OnStop()
    {
        Alt.Log("❌ SimpleFishing gestoppt!");
    }

}
