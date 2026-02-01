
using System.Collections.Generic;
using System.Linq;
using IShowSeed.Random;

namespace IShowSeed.Prediction;



public class VendoPredictor
{
    public static Dictionary<string, IEnumerable<string>> Generate(RouteType routeType)
    {
        if (routeType ==  RouteType.DEFAULT)
        {
            return new()
            {
                {"vendo_M1_Silos_SafeArea_01_Prop_VendingMachine.01", GenerateInternal("vendo_M1_Silos_SafeArea_01_Prop_VendingMachine.01", 3, VendoTier.T1)},
                {"vendo_M1_Silos_SafeArea_01_Prop_VendingMachine", GenerateInternal("vendo_M1_Silos_SafeArea_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine", GenerateInternal("vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine.01", GenerateInternal("vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine.01", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Pipeworks_To_Habitation_01_Prop_VendingMachine", GenerateInternal("vendo_Campaign_Interlude_Pipeworks_To_Habitation_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine.01", GenerateInternal("vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine.01", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Pier_Entrance_01_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Pier_Entrance_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Lab_Lobby_Prop_VendingMachine_T2", GenerateInternal("vendo_M3_Habitation_Lab_Lobby_Prop_VendingMachine_T2", 3, VendoTier.T2)},
                {"vendo_M3_Habitation_Lab_Ending_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Lab_Ending_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Habitation_To_Abyss_01_Prop_VendingMachine_Big", GenerateInternal("vendo_Campaign_Interlude_Habitation_To_Abyss_01_Prop_VendingMachine_Big", 8, VendoTier.T1)},
            };
        }
        if (routeType == RouteType.SHORTCUT_SINK)
        {
            return new()
            {
                {"vendo_Campaign_Interlude_Sink_To_Pipeworks_01_Prop_VendingMachine", GenerateInternal("vendo_Campaign_Interlude_Sink_To_Pipeworks_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Pipeworks_To_Habitation_01_Prop_VendingMachine", GenerateInternal("vendo_Campaign_Interlude_Pipeworks_To_Habitation_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine.01", GenerateInternal("vendo_M3_Habitation_Shaft_Intro_Prop_VendingMachine.01", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Pier_Entrance_01_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Pier_Entrance_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Lab_Lobby_Prop_VendingMachine_T2", GenerateInternal("vendo_M3_Habitation_Lab_Lobby_Prop_VendingMachine_T2", 3, VendoTier.T2)},
                {"vendo_M3_Habitation_Lab_Ending_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Lab_Ending_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Habitation_To_Abyss_01_Prop_VendingMachine_Big", GenerateInternal("vendo_Campaign_Interlude_Habitation_To_Abyss_01_Prop_VendingMachine_Big", 8, VendoTier.T1)},
            };
        }
        if (routeType == RouteType.SHORTCUT_BURNER)
        {
            return new()
            {
                {"vendo_M1_Silos_SafeArea_01_Prop_VendingMachine.01", GenerateInternal("vendo_M1_Silos_SafeArea_01_Prop_VendingMachine.01", 3, VendoTier.T1)},
                {"vendo_M1_Silos_SafeArea_01_Prop_VendingMachine", GenerateInternal("vendo_M1_Silos_SafeArea_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine", GenerateInternal("vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine.01", GenerateInternal("vendo_Campaign_Interlude_Silo_To_Pipeworks_01_Prop_VendingMachine.01", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Chute_To_Habitation_Prop_VendingMachine", GenerateInternal("vendo_Campaign_Interlude_Chute_To_Habitation_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Pier_Entrance_01_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Pier_Entrance_01_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_M3_Habitation_Lab_Lobby_Prop_VendingMachine_T2", GenerateInternal("vendo_M3_Habitation_Lab_Lobby_Prop_VendingMachine_T2", 3, VendoTier.T2)},
                {"vendo_M3_Habitation_Lab_Ending_Prop_VendingMachine", GenerateInternal("vendo_M3_Habitation_Lab_Ending_Prop_VendingMachine", 3, VendoTier.T1)},
                {"vendo_Campaign_Interlude_Habitation_To_Abyss_01_Prop_VendingMachine_Big", GenerateInternal("vendo_Campaign_Interlude_Habitation_To_Abyss_01_Prop_VendingMachine_Big", 8, VendoTier.T1)},
            };
        }
        return [];
    }


    private enum VendoTier
    {
        T1,
        T2
    }
    private struct VendoPurchase
    {
        public string Name;
        public float Chance;
    };


    private static readonly List<VendoPurchase> PurchaseListT1 = [
        new() {Name = "piton", Chance = 1.0f},
        new() {Name = "rebar", Chance = 1.0f},
        new() {Name = "rebar rope", Chance = 1.0f},
        new() {Name = "rubble", Chance = 1.0f},
        new() {Name = "beans", Chance = 1.0f},
        new() {Name = "injector", Chance = 0.8f},
        new() {Name = "pills", Chance = 0.8f},
        new() {Name = "rebar explosive", Chance = 0.8f},
        new() {Name = "sluggrub", Chance = 0.8f},
        new() {Name = "auto piton", Chance = 1.0f},
        new() {Name = "foodbar", Chance = 0.8f},
        new() {Name = "flaregun", Chance = 0.5f},
        new() {Name = "blinkeye", Chance = 0.8f},
    ];
    private static readonly List<VendoPurchase> PurchaseListT2 = [
        new() {Name = "injector", Chance = 0.8f},
        new() {Name = "pills", Chance = 0.8f},
        new() {Name = "rebar explosive", Chance = 0.8f},
        new() {Name = "sluggrub", Chance = 0.8f},
        new() {Name = "auto piton", Chance = 1.0f},
        new() {Name = "foodbar", Chance = 0.8f},
        new() {Name = "blinkeye", Chance = 0.8f},
    ];

    private static List<string> GenerateInternal(string callSite, int itemsToGenerate, VendoTier vendoTier)
    {
        Rod.SwitchToMode(Rod.ERandomMode.Prediction);
        List<string> predictedItems = [];

        Rod.Context _ = new();
        Rod.Enter(ref _, callSite);

        // generation
        List<VendoPurchase> purchaseList;
        if (vendoTier == VendoTier.T1) purchaseList = PurchaseListT1;
        else purchaseList = PurchaseListT2;

        while (predictedItems.Count < itemsToGenerate)
        {
            VendoPurchase purchase = purchaseList[UnityEngine.Random.Range(0, purchaseList.Count)];
            
            if (UnityEngine.Random.value < purchase.Chance)
            {
                predictedItems.Add(purchase.Name);
            }
        }

        Rod.Exit(in _);

        Rod.SwitchToMode(Rod.ERandomMode.Disabled);

        return predictedItems;
    }
}
