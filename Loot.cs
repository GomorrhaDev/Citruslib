using System.Collections.Generic;
using System.IO;
using System.Linq;
using Landfall.Network;

namespace WobbleBridge
{
    /// <summary>
    /// A class for creating groups of items, to simplify item-giving
    /// </summary>
    public class LootPack
    {
        List<int> types;
        List<byte> amounts;

        /// <summary>
        /// makes a new, empty lootpack.
        /// </summary>
        public LootPack()
        {
            types = new List<int>();
            amounts = new List<byte>();
            
        }

        /// <summary>
        /// creates a lootpack using TABG's Loot array as an input
        /// </summary>
        /// <param name="loot">An array of Loot structs.</param>
        public LootPack(Loot[] loot)
        {
            types = new List<int>();
            amounts = new List<byte>();

            foreach(Loot l in loot)
            {
                types.Add(l.loot.GetComponent<Pickup>().m_itemIndex);
                amounts.Add((byte)l.quanitity);
            }
        }

        /// <summary>
        /// adds a stack of an item to the loot pack.
        /// </summary>
        /// <param name="id">the id of the item. use the WobbleBridge Item enum!</param>
        /// <param name="amount">the amount to add.</param>
        public void AddLoot(int id, int amount)
        {
            if (amount > (int)byte.MaxValue)
            {
                AddLoot(id, amount - 254);
                amount = 254;
            }
            types.Add(id);
            amounts.Add((byte)amount);
        }
        
        public void AddLoot(string itemNameOrId, int amount)
        {
            if (WobbleBridge.Utils.ItemHelper.TryParseItemId(itemNameOrId, out int id))
            {
                AddLoot(id, amount);
            }
            else
            {
                UnityEngine.Debug.LogError($"Item '{itemNameOrId}' konnte nicht gefunden werden.");
            }
        }


        /// <summary>
        /// creates a lootpack that is a copy of the provided list of playerlootitems, which can be read from a TABGPlayerServer.
        /// </summary>
        /// <param name="loot"></param>
        /// <returns></returns>
        public static LootPack CopyPlayerLoot(List<TABGPlayerLootItem> loot)
        {
            LootPack ret = new LootPack();

            foreach(TABGPlayerLootItem item in loot)
            {
                ret.AddLoot(item.ItemIdentifier, item.ItemCount);
            }


            return ret;
        }

        /// <summary>
        /// gives the items to the player. the player should be alive
        /// </summary>
        /// <param name="player">the player to recieve the items</param>
        public void GiveTo(TABGPlayerServer player)
        {
            byte[] buffer = new byte[2 + types.Count() * 9];

            //List<int> ids = new List<int>();



            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {

                    binaryWriter.Write(player.PlayerIndex);
                    binaryWriter.Write((byte)types.Count());
                    for (int k = 0; k < types.Count(); k++)
                    {
                        int id = Wobble.World.GameRoomReference.GetNewWeaponIndex();
                        NetworkGun networkGun = new NetworkGun("DEV GUN", (int)amounts[k], id, types[k], null, false);
                        player.AddLoot(networkGun);
                        //does nothing
                        //Wobble.World.GameRoomReference.CurrentGameMode.HandlePlayerPickup(player, networkGun);

                        binaryWriter.Write(id);
                        binaryWriter.Write(types[k]);
                        binaryWriter.Write(amounts[k]);
                    }
                }
            }

            //world peace
            Wobble.World.SendMessageToClients(EventCode.LootPackGiven, buffer, player.PlayerIndex, true);



        }

    }

    /// <summary>
    /// an enum list of every vanilla item. helps hard-coding item-give commands and lootpacks easier
    /// </summary>

    public enum Item : int
    {
#pragma warning disable CS1591 //im not writing docs on every item
        _45_ACP = 0,
        BigAmmo = 1,
        Bolts = 2,
        MoneyAmmo = 3,
        MusketAmmo = 4,
        NoAmmo = 5,
        NormalAmmo = 6,
        RocketAmmo = 7,
        ShotgunAmmo = 8,
        SmallAmmo = 9,
        Soul = 10,
        TaserAmmo = 11,
        WaterAmmo = 12,
        Lv0JetpackArmor = 13,
        Lv1SafetyVest = 14,
        Lv2KevlarVest = 15,
        Lv3BigBoyKevlarVest = 16,
        Lv4HeavyArmor = 17,
        Lv5BananaArmor = 18,
        Lv5PickleArmor = 19,
        _05xScope = 20,
        _2xScope = 21,
        _4xScope = 22,
        _8xScope = 23,
        Compensator = 24,
        DamageAnalyzer = 25,
        HealingBarrel = 26,
        HealthAnalyzer = 27,
        HeavyBarrel = 28,
        LaserSight = 29,
        DoubleBarrel = 30,
        FastBarrel = 31,
        AccuracyBarrel = 32,
        FireRateBarrel = 33,
        BigSlowBulletBarrel = 34,
        PeriscopeBarrel = 35,
        Periscope = 36,
        Recycler = 37,
        RedDot = 38,
        Suppressor = 39,
        Suppressor002 = 40,
        TazerBarrel = 41,
        CommonBloodlust = 42,
        CommonCardio = 43,
        CommonDash = 44,
        CommonHealth = 45,
        CommonIce = 46,
        CommonJump = 47,
        CommonPoison = 48,
        CommonRecycling = 49,
        CommonRegeneration = 50,
        CommonRelax = 51,
        CommonShield = 52,
        CommonSpeed = 53,
        CommonSpray = 54,
        CommonStorm = 55,
        CommonTheHunt = 56,
        CommonVampire = 57,
        CommonWeaponMastery = 58,
        EpicBattlecry = 59,
        EpicBloodlust = 60,
        EpicCardio = 61,
        EpicCharge = 62,
        EpicDash = 63,
        EpicHealingWords = 64,
        EpicHealth = 65,
        EpicIce = 66,
        EpicJump = 67,
        EpicLitBeats = 68,
        EpicPoison = 69,
        EpicRecycling = 70,
        EpicRegeneration = 71,
        EpicRelax = 72,
        EpicShield = 73,
        EpicSmall = 74,
        EpicSpeed = 75,
        EpicSpray = 76,
        EpicStormCall = 77,
        EpicStorm = 78,
        EpicTheHunt = 79,
        EpicVampire = 80,
        EpicWeaponMastery = 81,
        EpicWordsOfJustice = 82,
        LegendaryBattlecry = 83,
        LegendaryBloodlust = 84,
        LegendaryCardio = 85,
        LegendaryCharge = 86,
        LegendaryDash = 87,
        LegendaryHealingWords = 88,
        LegendaryHealth = 89,
        LegendaryIce = 90,
        LegendaryJump = 91,
        LegendaryLitBeats = 92,
        LegendaryPoison = 93,
        LegendaryRecycling = 94,
        LegendaryRegeneration = 95,
        LegendaryRelax = 96,
        LegendaryShield = 97,
        LegendarySpeed = 98,
        LegendarySpray = 99,
        LegendaryStormCall = 100,
        LegendaryStorm = 101,
        LegendaryTheHunt = 102,
        LegendaryVampire = 103,
        LegendaryWeaponMastery = 104,
        LegendaryWordsOfJustice = 105,
        RareAirstrike = 106,
        RareBloodlust = 107,
        RareCardio = 108,
        RareDash = 109,
        RareHealth = 110,
        RareIce = 111,
        RareInsight = 112,
        RareJump = 113,
        RareLitBeats = 114,
        RarePoison = 115,
        RarePull = 116,
        RareRecycling = 117,
        RareRegeneration = 118,
        RareRelax = 119,
        RareShield = 120,
        RareSpeed = 121,
        RareSpray = 122,
        RareStorm = 123,
        RareTheHunt = 124,
        RareVampire = 125,
        RareWeaponMastery = 126,
        TheAssassin = 127,
        TheMadMechanic = 128,
        BlessingPickup = 129,
        TranscentionOrb = 130,
        Bandage = 131,
        MedKit = 132,
        Lv1BikeHelmet = 133,
        Lv2FastMotorcycleHelmet = 134,
        Lv2FastestMotorcycleHelmet = 135,
        Lv2MotorcycleHelmetOpen = 136,
        Lv2MotorcycleHelmet = 137,
        Lv2OldSchoolMotorcycleHelmet = 138,
        Lv3GreyKevlarHelmetWithGoogles = 139,
        Lv3GreyKevlarHelmet = 140,
        Lv3KevlarHelmetWithGoogles = 141,
        Lv3KevlarHelmet = 142,
        Lv4HeavyHelmetOpen1 = 143,
        Lv4CowboyHat = 144,
        Lv4ExplosiveguyHat = 145,
        Lv4HeavyHelmetOpen = 146,
        Lv4HeavyHelmet = 147,
        Lv4KnightHelmet = 148,
        Lv4RamboBandana = 149,
        Lv4TricorneHat = 150,
        Ak2K47 = 151,
        Ak47=152,
        Aug = 153,
        BeamAR = 154,
        Burstgun = 155,
        Famas = 156,
        CursedFamas = 157,
        H1 = 158,
        LiberatingM16 = 159,
        M16 = 160,
        Mp44=161,
        Scarh = 162,
        AutomaticCrossbow = 163,
        BalloonCrossbow = 164,
        Ak472=165,
        Ak473=166,
        Ak474=167,
        Bomb = 168,
        Crossbow = 169,
        TaserCrossbow = 170,
        FireoworkCrossbow = 171,
        Gaussbow = 172,
        GrapplingHook = 173,
        Harpoon = 174,
        MiniGun = 175,
        LiberatingMiniGun = 176,
        MegaGun = 177,
        MiniGun2 = 178,
        MissileLauncher = 179,
        MoneyStack = 180,
        SmokeRocketLauncher = 181,
        RocketLauncher = 182,
        TaserMiniGun = 183,
        ThePromise = 184,
        WaterGun = 185,
        Grenade = 186,
        BigHealingGrenade = 187,
        BlackHoleGrenade = 188,
        BombardementGrenade = 189,
        BouncyGrenade = 190,
        CageGrenade = 191,
        TaserCageGrenade = 192,
        ClusterGrenade = 193,
        ClusterDummyGrenade = 194,
        DummyGrenade = 195,
        FireGrenade = 196,
        Grenade2 = 197,
        HealingGrenade = 198,
        ImplosionGrenade = 199,
        KnockbackGrenade = 200,
        BigKnockbackGrenade = 201,
        LaunchPadGrenade = 202,
        Mgl = 203,
        OrbitalTaseGrenade = 204,
        OrbitalStrikeGrenade = 205,
        PoofGrenade = 206,
        ShieldGrenade = 207,
        SmokeGrenade = 208,
        SnowStormGrenade = 209,
        SplinterGrenade = 210,
        TaserSplinterGrenade = 211,
        StunGrenade = 212,
        SnowStormGrenade2 = 213,
        Dynamite = 214,
        VolleyGrenade = 215,
        WallGrenade = 216,
        BrowningM2 = 217,
        M1918Bar = 218,
        M8 = 219,
        Mg42=220,
        SpellBlindingLight = 221,
        SpellGravityField = 222,
        SpellGust = 223,
        SpellHealingAura = 224,
        SpellSpeedAura = 225,
        SpellSummonRock = 226,
        SpellTeleport = 227,
        SpellTrack = 228,
        SpellFireBall = 229,
        SpellIceBolt = 230,
        SpellMagicMissile = 231,
        SpellMirage = 232,
        SpellOrbOfSight = 233,
        SpellReveal = 234,
        SpellShockwave = 235,
        SpellSummonTree = 236,
        SpellTrack2 = 237,
        BallisticShield = 238,
        BallisticShields = 239,
        TaserBallisticShield = 240,
        Baton = 241,
        BlackKatana = 242,
        BoxingGlove = 243,
        Cleaver = 244,
        Crowbar = 245,
        CrusaderSword = 246,
        TaserCrusaderSword = 247,
        Fish = 248,
        TaserFish = 249,
        HolySword = 250,
        InflatableHammer = 251,
        JarlAxe = 252,
        TaserJarlAxe = 253,
        Katana = 254,
        Knife = 255,
        Rapier = 256,
        RiotShield = 257,
        Sabre = 258,
        ShallowPotWithLongHandle = 259,
        Shield = 260,
        Shovel = 261,
        VikingAxe = 262,
        Weights = 263,
        Beretta93R = 264,
        CrossbowPistol = 265,
        DesertEagle = 266,
        Flintlock = 267,
        TaserFlintlock = 268,
        AutoRevolver = 269,
        WindUpPistol = 270,
        G18C = 271,
        GlueGun = 272,
        HandGun = 273,
        HandCannon = 274,
        LiberatingM1911 = 275,
        LugerP08 = 276,
        M1911 = 277,
        RealGun = 278,
        ReallyBigDeagle = 279,
        Revolver = 280,
        HolyRevolver = 281,
        Revolver2 = 282,
        Hardballer = 283,
        Taser = 284,
        BeamDmr = 285,
        Fal = 286,
        Garand = 287,
        LiberatingGarand = 288,
        M14 = 289,
        S7 = 290,
        WinchesterModel1886 = 291,
        AA12=292,
        Blunderbuss = 293,
        SawedOffShotgun = 294,
        FlyingBlunderbuss = 295,
        LiberatingAA12=296,
        Mossberg500=297,
        Mossberg5000=298,
        TaserMossberg500=299,
        Rainmaker = 300,
        TheArnold = 301,
        Aks74U=302,
        Awps74U=303,
        MoneyMakerMac = 304,
        Glockinator = 305,
        LiberatingM1A1Thompson = 306,
        M1A1Thompson = 307,
        Mac10=308,
        Mp40=309,
        Mp5K = 310,
        P90 = 311,
        Ppsh41=312,
        Tec9=313,
        Ump45=314,
        Vector = 315,
        Z4 = 316,
        Awp = 317,
        TaserAwp = 318,
        Barrett = 319,
        BeamSniper = 320,
        Kar98K = 321,
        LiberatingBarrett = 322,
        Musket = 323,
        TaserMusket = 324,
        ReallyBigBarrett = 325,
        SniperShotgun=326,
        DoubleShot = 327,
        Vss = 328
#pragma warning restore CS1591
    }

}
