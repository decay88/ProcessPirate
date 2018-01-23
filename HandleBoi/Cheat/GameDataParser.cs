using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace HandleBoi.Cheat
{
    class GameDataParser
    {
        private readonly NativeRemoteCall remoteCall;
        private const int ACTOR_COUNT = 64;

        private IntPtr m_UWorld;
        private IntPtr m_gameInstance;
        private IntPtr m_ULocalPlayer;
        private IntPtr m_localPlayer;
        private IntPtr m_viewportclient;
        private IntPtr m_localPawn;
        private IntPtr m_localPlayerState;
        private IntPtr m_PWorld;
        private IntPtr m_ULevel;
        private Int32 m_actorCount;
        private Vector3 m_localPlayerPosition;
        private IntPtr m_localPlayerBasePointer;
        private Int32 m_localTeam;
        private IntPtr m_AActorPtr;

        List<Int32> allIDs = new List<int>();
        List<Int32> playerIDs = new List<int>();
        List<Int32> vehicleIDs = new List<int>();

        private readonly List<string> vehicleGNameVec = new List<string> { "Uaz", "Buggy", "Dacia", "ABP_Motorbike", "BP_Motorbike", "Boat_PG117" };
        private readonly Dictionary<string, string> dropGNameMap = new Dictionary<string, string>
        {
            { "Item_Head_G_01_Lv3_C", "Helm3" },
            { "Item_Head_G_01_Lv3_", "Helm3" },
            { "Item_Armor_C_01_Lv3", "Vest3" },
            { "Item_Armor_C_01_Lv3_C", "Vest3" },
            { "Item_Equip_Armor_Lv3_C", "Vest3" },
            { "Item_Equip_Armor_Lv3", "Vest3" }, 
            { "Item_Attach_Weapon_Muzzle_Suppressor_SniperRifle", "Supp(SR)" },
            { "Item_Attach_Weapon_Muzzle_Suppressor_Large", "Supp(AR)" },
            { "Item_Attach_Weapon_Muzzle_Suppressor_Large_C", "Supp(SR)" },
            { "Item_Heal_MedKit", "Meds" },
            { "Item_Heal_FirstAid", "Meds" },
            { "Item_Weapon_Kar98k", "kar98" },
            { "Item_Weapon_Mini14", "mini" },
            { "Item_Weapon_M16A4", "M16" },
            { "Item_Weapon_HK416", "m416" },
            { "Item_Weapon_SCAR-L", "SCAR" },
            { "Item_Weapon_SKS", "sks" },
            { "Item_Attach_Weapon_Upper_ACOG_01", "4x" },
            { "Item_Attach_Weapon_Upper_CQBSS", "8x" },
            { "Item_Attach_Weapon_Upper_CQBSS_C", "8x" }
        };
        private readonly List<string> playerGNameVec = new List<string> { "PlayerMale", "PlayerFemale" };



        GameDataParser(SocketServer server, NativeRemoteCall remoteCall)
        {
            this.remoteCall = remoteCall;
        }

        
        //void readLoop()
        //{
        //    data["players"] = json::array();
        //    data["vehicles"] = json::array();
        //    data["items"] = json::array();
        //
        //    readLocals();
        //    readPlayers(data);
        //}
        //
        //IntPtr getPUBase()
        //{
        //    return m_kReader->getPUBase();
        //}
        //
        //IntPtr readPUBase()
        //{
        //    return m_kReader->readPUBase();
        //}
        //
        ///*
        // * PRIVATE CLASS FUNCTIONS
        // */
        //void readPlayers()
        //{
        //    for (int i = 0; i < ACTOR_COUNT; i++)
        //    {
        //        // read the position of Player
        //        IntPtr curActor = remoteCall.ReadProcessMemory<IntPtr>(m_AActorPtr + (i * 0x8));
        //        Int32 curActorID = remoteCall.ReadProcessMemory<Int32>(curActor + 0x0018);
        //        string actorGName = m_kReader->getGNameFromId(curActorID).Trim();
        //
        //        // Here we check if the name is found from the wanted GNames list (PlayerMale etc...)
        //        if (playerIDs.Contains(curActorID))
        //        {
        //            IntPtr rootCmpPtr = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x180);
        //            IntPtr playerState = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x3C0);
        //            Vector3 actorLocation = remoteCall.ReadProcessMemory<Vector3>(rootCmpPtr + 0x1A0);
        //
        //            Int32 actorTeam = remoteCall.ReadProcessMemory<Int32>(playerState + 0x0444);
        //
        //            //actorLocation.X += remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x918);
        //            //actorLocation.Y += remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x91C);
        //            // actorLocation.Z += ReadAny<int>(PWorld + 0x920);
        //
        //            //w_data["players"].emplace_back(json::object({
        //            //    {
        //            //        "t", actorTeam
        //            //    },{
        //            //        "x", actorLocation.X
        //            //    },{
        //            //        "y", actorLocation.Y
        //            //    } /*,{ "z", actorLocation.Z }*/
        //            //}));
        //        }
        //        
        //        //if (actorGName == "DroppedItemGroup" || actorGName == "DroppedItemInteractionComponent")
        //        //{
        //        //    IntPtr rootCmpPtr = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x180);
        //        //    IntPtr playerState = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x3C0);
        //        //    Vector3 actorLocation = remoteCall.ReadProcessMemory<Vector3>(rootCmpPtr + 0x1A0);
        //        //    IntPtr DroppedItemArray = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x2D8);
        //        //    Int32 DroppedItemCount = remoteCall.ReadProcessMemory<Int32>(curActor + 0x2E0);
        //        //
        //        //    for (int j = 0; j < DroppedItemCount; j++)
        //        //    {
        //        //        IntPtr ADroppedItem = remoteCall.ReadProcessMemory<IntPtr>(DroppedItemArray + j * 0x10);
        //        //        Vector3 droppedLocation = remoteCall.ReadProcessMemory<Vector3>(ADroppedItem + 0x1E0);
        //        //        droppedLocation.X = droppedLocation.X + actorLocation.X + remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x918);
        //        //        droppedLocation.Y = droppedLocation.Y + actorLocation.Y + remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x91C);
        //        //        IntPtr UItem = remoteCall.ReadProcessMemory<IntPtr>(ADroppedItem + 0x448);
        //        //        Int32 UItemID = remoteCall.ReadProcessMemory<Int32>(UItem + 0x18);
        //        //        string itemName = m_kReader->getGNameFromId(UItemID);
        //        //
        //        //        // check if inside the map / array of wanted items
        //        //        foreach (string name in dropGNameMap.Keys)
        //        //        {
        //        //            if (itemName.Substring(0, name.Length) == name)
        //        //            {
        //        //                IntPtr rootCmpPtr = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x180);
        //        //                Vector3 actorLocation = remoteCall.ReadProcessMemory<Vector3>(rootCmpPtr + 0x1A0);
        //        //
        //        //                actorLocation.X += remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x918);
        //        //                actorLocation.Y += remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x91C);
        //        //
        //        //                w_data["items"].emplace_back(json::object({
        //        //                    {
        //        //                        "n", it->second
        //        //                    },{
        //        //                        "x", droppedLocation.X
        //        //                    },{
        //        //                        "y", droppedLocation.Y
        //        //                    }
        //        //                }));
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //
        //        //else if (actorGName.substr(0, strlen("CarePackage")) == "CarePackage" ||
        //        //         actorGName.substr(0, strlen("AircraftCarePackage")) == "AircraftCarePackage" ||
        //        //         actorGName.substr(0, strlen("Carapackage_RedBox")) == "Carapackage_RedBox")
        //        //{
        //        //    IntPtr rootCmpPtr = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x180);
        //        //    IntPtr playerState = remoteCall.ReadProcessMemory<IntPtr>(curActor + 0x3C0);
        //        //    Vector3 actorLocation = remoteCall.ReadProcessMemory<Vector3>(rootCmpPtr + 0x1A0);
        //        //
        //        //    actorLocation.X += remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x918);
        //        //    actorLocation.Y += remoteCall.ReadProcessMemory<Int32>(m_PWorld + 0x91C);
        //        //
        //        //    w_data["vehicles"].emplace_back(json::object({
        //        //        {
        //        //            "v", "Drop"
        //        //        },{
        //        //            "x", actorLocation.X
        //        //        },{
        //        //            "y", actorLocation.Y
        //        //        }
        //        //    }));
        //        //
        //        //}
        //
        //        else
        //        {
        //            allIDs.Add(curActorID);
        //
        //            if (actorGName == "FAIL")
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                if (playerGNameVec.Contains(actorGName))
        //                {
        //                    playerIDs.Add(curActorID);
        //                }
        //                else if (vehicleGNameVec.Contains(actorGName))
        //                {
        //                    vehicleIDs.Add(curActorID);
        //                }
        //            }
        //        }
        //    }
        //}
        //
        //void readLocals()
        //{
        //    m_UWorld = remoteCall.ReadProcessMemory<IntPtr>(m_kReader->getPUBase() + 0x3CA94A8);
        //    m_gameInstance = remoteCall.ReadProcessMemory<IntPtr>(m_UWorld + 0x140);
        //    m_ULocalPlayer = remoteCall.ReadProcessMemory<IntPtr>(m_gameInstance + 0x38);
        //    m_localPlayer = remoteCall.ReadProcessMemory<IntPtr>(m_ULocalPlayer);
        //    m_viewportclient = remoteCall.ReadProcessMemory<IntPtr>(m_localPlayer + 0x58);
        //    m_localPawn = remoteCall.ReadProcessMemory<IntPtr>(m_localPlayer + 0x3A8);
        //    m_localPlayerState = remoteCall.ReadProcessMemory<IntPtr>(m_localPawn + 0x3C0);
        //    m_PWorld = remoteCall.ReadProcessMemory<IntPtr>(m_viewportclient + 0x80);
        //    m_ULevel = remoteCall.ReadProcessMemory<IntPtr>(m_PWorld + 0x30);
        //    m_actorCount = remoteCall.ReadProcessMemory<Int32>(m_ULevel + 0xA8);
        //
        //    m_localPlayerPosition = remoteCall.ReadProcessMemory<Vector3>(m_localPlayer + 0x70);
        //    m_localPlayerBasePointer = remoteCall.ReadProcessMemory<IntPtr>(m_localPlayer);
        //
        //    m_localTeam = remoteCall.ReadProcessMemory<Int32>(m_localPlayerState + 0x0444);
        //
        //    m_AActorPtr = remoteCall.ReadProcessMemory<IntPtr>(m_ULevel + 0xA0);
        //}



/*
 * CLASS VARIABLES
 */
/*
 * Local variables
 * These are updated once every read loop.
 */


/*
 * Global IDs that are found from the game
 * These containers are used to help the 
 * maintaining of systematic ID handling and 
 * storing.
 */
 
    }
}
