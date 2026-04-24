using System.Collections.Generic;

namespace sailadex
{
    internal sealed class Region
    {
        private readonly int _index;
        private readonly string _name;
        private readonly string _badgeName;
        private readonly string _code;
        private readonly List<string> _ports;
        private readonly List<string> _islands;

        public int Index => _index;
        public string Name => _name;
        public string BadgeName => _badgeName;
        public string Code => _code;
        public IReadOnlyList<string> Ports => _ports.AsReadOnly();
        public IReadOnlyList<string> Islands => _islands.AsReadOnly();

        private Region(int index, string name, string badgeName, string code, List<string> ports, List<string> islands)
        {
            _index = index;
            _name = name;
            _badgeName = badgeName;
            _code = code;
            _ports = ports;
            _islands = islands;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        public static readonly Region AlAnkh =
            new Region
            (
                0,
                "Al'Ankh",
                "alankhBadge",
                "Aa",
                new List<string>
                {
                    "Gold Rock City",
                    "Al'Nilem",
                    "Neverdin",
                    "Albacore Town",
                    "Alchemist's Island",
                    "Al'Ankh Academy",
                    "Oasis",
                    "Old Ankh Town",
                    "Mirage Mountain",
                    "Saffron Island",
                },
                new List<string>
                {
                    "island 1 A (gold rock)",
                    "island 2 A (AlNilem)",
                    "island 3 A (Neverdin)",
                    "island 4 A (fish island)",
                    "island 5 A (clear mind)",
                    "island 6 A (lion fang)",
                    "island 7 A (alchemist's island)",
                    "island 8 A (academy)",
                    "island 20 A (Oasis)",
                    "island 40 (coffee)",
                    "island 41 (mirage mountain)",
                    "island 42 ()",
                }
            );

        public static readonly Region EmeraldArchipelago =
            new Region
            (
                1,
                "Emerald Archipelago",
                "emeraldBadge",
                "Ea",
                new List<string>
                {
                    "Dragon Cliffs",
                    "Sanctuary",
                    "Crab Beach",
                    "New Port",
                    "Sage Hills",
                    "Serpent Isle",
                    "Dead Cove",
                    "Turtle Island",
                    //"Pondering Peak" not a port
                },
                new List<string>
                {
                    "island 9 E (dragon cliffs)",
                    "island 10 E (sanctuary)",
                    "island 11 E (crab beach)",
                    "island 12 E (New Port)",
                    "island 13 E (Sage Hills)",
                    "island 22 E (serpent isle)",
                    "island 37 E (swamp)",
                    "island 38 E (jungle)",
                    "island 39 E (onsen)",
                }
            );

        public static readonly Region Aestrin =
            new Region
            (
                2,
                "Aestrin",
                "mediBadge",
                "Ae",
                new List<string>
                {
                    "Fort Aestrin",
                    "Sunspire",
                    "Mount Malefic",
                    "Siren Song",
                    "Eastwind",
                    "Firefly Grotto",
                    "Aestra Abbey",
                    "Fey Valley",
                    "Happy Bay",
                    "Chronos",
                },
                new List<string>
                {
                    "island 15 M (Fort)",
                    "island 16 M (Sunspire)",
                    "island 17 M (Mount Malefic)",                    
                    "island 19 M (Eastwind)",
                    "island 21 M (Siren Song)",
                    "island 33 M cave church",
                    "island 34 M monastery",
                    "island 35 M valley",
                    "island 23 M oracle",
                    "island 18 M (Oasis)",
                    "island 25 (chronos)",
                }
            );

        public static readonly Region FireFishLagoon =
            new Region
            (
                3,
                "Fire Fish Lagoon",
                "lagoonBadge",
                "Ffl",
                new List<string>
                {
                    "Kicia Bay",
                    "Fire Fish Town",
                    "On'na",
                    "Sen'na",
                },
                new List<string>
                {
                    "island 26 Lagoon Temple",
                    "island 27 Lagoon Shipyard",
                    "island 28 Lagoon Senna",
                    "island 29 Lagoon Onna",
                    "island 31 Lagoon Fisherman",
                }
            );

        public static readonly IReadOnlyList<Region> AllRegions = new List<Region>
        {
            AlAnkh,
            EmeraldArchipelago,
            Aestrin,
            FireFishLagoon
        }.AsReadOnly();

        public static readonly IReadOnlyList<string> AllBadgeNames = new List<string>
        {
            AlAnkh.BadgeName,
            EmeraldArchipelago.BadgeName,
            Aestrin.BadgeName,
            FireFishLagoon.BadgeName,
            "allPortsBadge"
        }.AsReadOnly();

        public static IReadOnlyList<string> AllPorts
        {
            get
            {
                var allPorts = new List<string>();
                foreach (var region in AllRegions)
                {
                    allPorts.AddRange(region.Ports);
                }
                return allPorts.AsReadOnly();
            }
        }

        public static IReadOnlyList<string> TransitCodes
        {
            get
            {
                var transitCodes = new List<string>();
                foreach (var region in AllRegions)
                {
                    foreach (var otherRegion in AllRegions)
                    {
                        if (region.Index != otherRegion.Index)
                        {
                            transitCodes.Add($"{region.Code}{otherRegion.Code}");
                        }
                    }
                }
                return transitCodes.AsReadOnly();
            }
        }
    }
}
