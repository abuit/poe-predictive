using System.Linq;

namespace POEStash
{
    public enum ItemType
    {
        Unknown,
        Amulet,
        Talisman,
        Ring,
        Belt,
        Quiver,
        BodyArmour,
        Boots,
        Gloves,
        Helmet,
        Shield,
        Bow,
        Claw,
        Dagger,
        OneHandedAxe,
        OneHandedMace,
        OneHandedSword,
        Sceptre,
        Staff,
        OneHandedThrustringSword,
        TwoHandedAxe,
        TwoHandedMace,
        TwoHandedSword,
        Wand
    }

    public static class BaseTypes
    {
        public static ItemType GetItemType(string typeString)
        {
            if (BaseTypesAmulets.BaseTypes.Contains(typeString))                  return ItemType.Amulet;
            if (BaseTypesRings.BaseTypes.Contains(typeString))                    return ItemType.Ring;
            if (BaseTypesBelt.BaseTypes.Contains(typeString))                     return ItemType.Belt;
            if (BaseTypesQuivers.BaseTypes.Contains(typeString))                  return ItemType.Quiver;
            if (BaseTypesTalismans.BaseTypes.Contains(typeString))                return ItemType.Talisman;
            if (BaseTypesBodyArmour.BaseTypes.Contains(typeString))               return ItemType.BodyArmour;
            if (BaseTypesBoots.BaseTypes.Contains(typeString))                    return ItemType.Boots;
            if (BaseTypesGloves.BaseTypes.Contains(typeString))                   return ItemType.Gloves;
            if (BaseTypesHelmets.BaseTypes.Contains(typeString))                  return ItemType.Helmet;
            if (BaseTypesShields.BaseTypes.Contains(typeString))                  return ItemType.Shield;
            if (BaseTypesBows.BaseTypes.Contains(typeString))                     return ItemType.Bow;
            if (BaseTypesClaws.BaseTypes.Contains(typeString))                    return ItemType.Claw;
            if (BaseTypesDaggers.BaseTypes.Contains(typeString))                  return ItemType.Dagger;
            if (BaseTypesOneHandedAxes.BaseTypes.Contains(typeString))            return ItemType.OneHandedAxe;
            if (BaseTypesOneHandedMaces.BaseTypes.Contains(typeString))           return ItemType.OneHandedMace;
            if (BaseTypesOneHandedSwords.BaseTypes.Contains(typeString))          return ItemType.OneHandedSword;
            if (BaseTypesSceptres.BaseTypes.Contains(typeString))                 return ItemType.Sceptre;
            if (BaseTypesStaffs.BaseTypes.Contains(typeString))                   return ItemType.Staff;
            if (BaseTypesThrustingOneHandedSwords.BaseTypes.Contains(typeString)) return ItemType.OneHandedThrustringSword;
            if (BaseTypesTwoHandedAxes.BaseTypes.Contains(typeString))            return ItemType.TwoHandedAxe;
            if (BaseTypesTwoHandedMaces.BaseTypes.Contains(typeString))           return ItemType.TwoHandedMace;
            if (BaseTypesTwoHandedSwords.BaseTypes.Contains(typeString))          return ItemType.TwoHandedSword;
            if (BaseTypesWands.BaseTypes.Contains(typeString))                    return ItemType.Wand;

            return ItemType.Unknown;
        }
    }

    public static class BaseTypesAmulets
    {
        public static string[] BaseTypes =
        {
            "Coral Amulet",
            "Paua Amulet",
            "Amber Amulet",
            "Jade Amulet",
            "Lapis Amulet",
            "Gold Amulet",
            "Agate Amulet",
            "Citrine Amulet",
            "Turquoise Amulet",
            "Onyx Amulet",
            "Marble Amulet",
            "Blue Pearl Amulet"
        };
    }

    public static class BaseTypesRings
    {
        public static string[] BaseTypes =
        {
            "Breach Ring",
            "Iron Ring",
            "Coral Ring",
            "Paua Ring",
            "Sapphire Ring",
            "Topaz Ring",
            "Golden Hoop",
            "Jet Ring",
            "Ruby Ring",
            "Gold Ring",
            "Two-Stone Ring",
            "Diamond Ring",
            "Moonstone Ring",
            "Prismatic Ring",
            "Amethyst Ring",
            "Unset Ring",
            "Steel Ring",
            "Opal Ring",
        };
    }

    public static class BaseTypesBelt
    {
        public static string[] BaseTypes =
        {
            "Chain Belt",
            "Rustic Sash",
            "Heavy Belt",
            "Leather Belt",
            "Cloth Belt",
            "Studded Belt",
            "Vanguard Belt",
            "Crystal Belt"
        };
    }

    public static class BaseTypesQuivers
    {
        public static string[] BaseTypes =
        {
            "Two-Point Arrow Quiver",
            "Serrated Arrow Quiver",
            "Sharktooth Arrow Quiver",
            "Blunt Arrow Quiver",
            "Fire Arrow Quiver",
            "Broadhead Arrow Quiver",
            "Penetrating Arrow Quiver",
            "Spike-Point Arrow Quiver",
        };
    }

    public static class BaseTypesTalismans
    {
        public static string[] BaseTypes =
        {
            "Spinefuse Talisman",
            "Mandible Talisman",
            "Hexclaw Talisman",
            "Primal Skull Talisman",
            "Wereclaw Talisman",
            "Splitnewt Talisman",
            "Clutching Talisman",
            "Avian Twins Talisman",
            "Fangjaw Talisman",
            "Horned Talisman",
            "Three Rat Talisman",
            "Monkey Twins Talisman",
            "Longtooth Talisman",
            "Rotfeather Talisman",
            "Monkey Paw Talisman",
            "Three Hands Talisman",
            "Jet Amulet",
            "Black Maw Talisman",
            "Bonespire Talisman",
            "Ashscale Talisman",
            "Lone Antler Talisman",
            "Deep One Talisman",
            "Breakrib Talisman",
            "Deadhand Talisman",
            "Undying Flesh Talisman",
            "Rot Head Talisman"
        };
    }

    public static class BaseTypesBodyArmour
    {
        public static string[] BaseTypes =
        {
            "Plate Vest",
            "Shabby Jerkin",
            "Simple Robe",
            "Chainmail Vest",
            "Scale Vest",
            "Padded Vest",
            "Chestplate",
            "Light Brigandine",
            "Chainmail Tunic",
            "Oiled Vest",
            "Strapped Leather",
            "Silken Vest",
            "Scale Doublet",
            "Ringmail Coat",
            "Copper Plate",
            "Buckskin Tunic",
            "Padded Jacket",
            "Scholar's Robe",
            "Infantry Brigandine",
            "Chainmail Doublet",
            "War Plate",
            "Oiled Coat",
            "Wild Leather",
            "Silken Garb",
            "Full Plate",
            "Mage's Vestment",
            "Scarlet Raiment",
            "Full Ringmail",
            "Full Scale Armour",
            "Full Leather",
            "Soldier's Brigandine",
            "Silk Robe",
            "Waxed Garb",
            "Arena Plate",
            "Sun Leather",
            "Full Chainmail",
            "Holy Chainmail",
            "Lordly Plate",
            "Thief's Garb",
            "Cabalist Regalia",
            "Field Lamellar",
            "Bone Armour",
            "Eelskin Tunic",
            "Bronze Plate",
            "Sage's Robe",
            "Wyrmscale Doublet",
            "Latticed Ringmail",
            "Quilted Jacket",
            "Silken Wrap",
            "Battle Plate",
            "Frontier Leather",
            "Hussar Brigandine",
            "Crusader Chainmail",
            "Sleek Coat",
            "Sun Plate",
            "Conjurer's Vestment",
            "Glorious Leather",
            "Full Wyrmscale",
            "Ornate Ringmail",
            "Crimson Raiment",
            "Colosseum Plate",
            "Coronal Leather",
            "Spidersilk Robe",
            "Commander's Brigandine",
            "Chain Hauberk",
            "Lacquered Garb",
            "Majestic Plate",
            "Cutthroat's Garb",
            "Destroyer Regalia",
            "Battle Lamellar",
            "Devout Chainmail",
            "Crypt Armour",
            "Savant's Robe",
            "Sharkskin Tunic",
            "Golden Plate",
            "Dragonscale Doublet",
            "Loricated Ringmail",
            "Sentinel Jacket",
            "Necromancer Silks",
            "Crusader Plate",
            "Destiny Leather",
            "Desert Brigandine",
            "Conquest Chainmail",
            "Occultist's Vestment",
            "Exquisite Leather",
            "Astral Plate",
            "Varnished Coat",
            "Full Dragonscale",
            "Elegant Ringmail",
            "Zodiac Leather",
            "Gladiator Plate",
            "Widowsilk Robe",
            "Blood Raiment",
            "General's Brigandine",
            "Saint's Hauberk",
            "Glorious Plate",
            "Assassin's Garb",
            "Vaal Regalia",
            "Sadist Garb",
            "Triumphant Lamellar",
            "Saintly Chainmail",
            "Carnal Armour",
            "Sacrificial Garb"
        };
    }

    public static class BaseTypesBoots
    {
        public static string[] BaseTypes =
        {
            "Iron Greaves",
            "Rawhide Boots",
            "Wool Shoes",
            "Chain Boots",
            "Wrapped Boots",
            "Leatherscale Boots",
            "Velvet Slippers",
            "Steel Greaves",
            "Goathide Boots",
            "Ringmail Boots",
            "Strapped Boots",
            "Ironscale Boots",
            "Deerskin Boots",
            "Silk Slippers",
            "Plated Greaves",
            "Clasped Boots",
            "Mesh Boots",
            "Bronzescale Boots",
            "Scholar Boots",
            "Reinforced Greaves",
            "Nubuck Boots",
            "Shackled Boots",
            "Steelscale Boots",
            "Riveted Boots",
            "Antique Greaves",
            "Satin Slippers",
            "Eelskin Boots",
            "Zealot Boots",
            "Trapper Boots",
            "Serpentscale Boots",
            "Samite Slippers",
            "Sharkskin Boots",
            "Ancient Greaves",
            "Ambush Boots",
            "Soldier Boots",
            "Wyrmscale Boots",
            "Conjurer Boots",
            "Goliath Greaves",
            "Carnal Boots",
            "Shagreen Boots",
            "Legion Boots",
            "Hydrascale Boots",
            "Arcanist Slippers",
            "Vaal Greaves",
            "Stealth Boots",
            "Assassin's Boots",
            "Crusader Boots",
            "Dragonscale Boots",
            "Sorcerer Boots",
            "Titan Greaves",
            "Slink Boots",
            "Murder Boots",
            "Two-Toned Boots",
        };
    }

    public static class BaseTypesGloves
    {
        public static string[] BaseTypes =
        {
            "Iron Gauntlets",
            "Rawhide Gloves",
            "Wool Gloves",
            "Fishscale Gauntlets",
            "Wrapped Mitts",
            "Chain Gloves",
            "Goathide Gloves",
            "Plated Gauntlets",
            "Velvet Gloves",
            "Ironscale Gauntlets",
            "Strapped Mitts",
            "Ringmail Gloves",
            "Deerskin Gloves",
            "Bronze Gauntlets",
            "Silk Gloves",
            "Bronzescale Gauntlets",
            "Clasped Mitts",
            "Mesh Gloves",
            "Nubuck Gloves",
            "Steel Gauntlets",
            "Embroidered Gloves",
            "Trapper Mitts",
            "Steelscale Gauntlets",
            "Riveted Gloves",
            "Eelskin Gloves",
            "Antique Gauntlets",
            "Satin Gloves",
            "Zealot Gloves",
            "Serpentscale Gauntlets",
            "Ambush Mitts",
            "Sharkskin Gloves",
            "Samite Gloves",
            "Ancient Gauntlets",
            "Wyrmscale Gauntlets",
            "Carnal Mitts",
            "Soldier Gloves",
            "Goliath Gauntlets",
            "Shagreen Gloves",
            "Conjurer Gloves",
            "Legion Gloves",
            "Assassin's Mitts",
            "Hydrascale Gauntlets",
            "Arcanist Gloves",
            "Stealth Gloves",
            "Vaal Gauntlets",
            "Crusader Gloves",
            "Murder Mitts",
            "Dragonscale Gauntlets",
            "Titan Gauntlets",
            "Sorcerer Gloves",
            "Slink Gloves",
            "Gripped Gloves",
            "Fingerless Silk Gloves",
            "Spiked Gloves",
        };
    }

    public static class BaseTypesHelmets
    {
        public static string[] BaseTypes =
        {
            "Iron Hat",
            "Vine Circlet",
            "Leather Cap",
            "Scare Mask",
            "Battered Helm",
            "Rusted Coif",
            "Cone Helmet",
            "Iron Circlet",
            "Tricorne",
            "Plague Mask",
            "Soldier Helmet",
            "Sallet",
            "Iron Mask",
            "Torture Cage",
            "Barbute Helmet",
            "Leather Hood",
            "Great Helmet",
            "Visored Sallet",
            "Close Helmet",
            "Tribal Circlet",
            "Festival Mask",
            "Wolf Pelt",
            "Crusader Helmet",
            "Gilded Sallet",
            "Bone Circlet",
            "Gladiator Helmet",
            "Golden Mask",
            "Secutor Helm",
            "Aventail Helmet",
            "Raven Mask",
            "Lunaris Circlet",
            "Reaver Helmet",
            "Hunter Hood",
            "Fencer Helm",
            "Zealot Helmet",
            "Callous Mask",
            "Noble Tricorne",
            "Steel Circlet",
            "Siege Helmet",
            "Lacquered Helmet",
            "Regicide Mask",
            "Great Crown",
            "Necromancer Circlet",
            "Ursine Pelt",
            "Samite Helmet",
            "Harlequin Mask",
            "Magistrate Crown",
            "Fluted Bascinet",
            "Solaris Circlet",
            "Silken Hood",
            "Ezomyte Burgonet",
            "Vaal Mask",
            "Pig-Faced Bascinet",
            "Prophet Crown",
            "Sinner Tricorne",
            "Mind Cage",
            "Royal Burgonet",
            "Deicide Mask",
            "Nightmare Bascinet",
            "Praetor Crown",
            "Hubris Circlet",
            "Eternal Burgonet",
            "Lion Pelt",
            "Bone Helmet",
        };
    }

    public static class BaseTypesShields
    {
        public static string[] BaseTypes =
        {
            "Splintered Tower Shield",
            "Goathide Buckler",
            "Twig Spirit Shield",
            "Spiked Bundle",
            "Corroded Tower Shield",
            "Rotted Round Shield",
            "Plank Kite Shield",
            "Pine Buckler",
            "Yew Spirit Shield",
            "Rawhide Tower Shield",
            "Fir Round Shield",
            "Driftwood Spiked Shield",
            "Linden Kite Shield",
            "Bone Spirit Shield",
            "Painted Buckler",
            "Cedar Tower Shield",
            "Reinforced Kite Shield",
            "Alloyed Spiked Shield",
            "Studded Round Shield",
            "Hammered Buckler",
            "Tarnished Spirit Shield",
            "Copper Tower Shield",
            "Layered Kite Shield",
            "Scarlet Round Shield",
            "Burnished Spiked Shield",
            "Jingling Spirit Shield",
            "War Buckler",
            "Reinforced Tower Shield",
            "Splendid Round Shield",
            "Ornate Spiked Shield",
            "Brass Spirit Shield",
            "Ceremonial Kite Shield",
            "Gilded Buckler",
            "Painted Tower Shield",
            "Walnut Spirit Shield",
            "Oak Buckler",
            "Buckskin Tower Shield",
            "Maple Round Shield",
            "Redwood Spiked Shield",
            "Etched Kite Shield",
            "Ivory Spirit Shield",
            "Enameled Buckler",
            "Mahogany Tower Shield",
            "Ancient Spirit Shield",
            "Spiked Round Shield",
            "Compound Spiked Shield",
            "Corrugated Buckler",
            "Steel Kite Shield",
            "Bronze Tower Shield",
            "Chiming Spirit Shield",
            "Crimson Round Shield",
            "Polished Spiked Shield",
            "Battle Buckler",
            "Laminated Kite Shield",
            "Girded Tower Shield",
            "Thorium Spirit Shield",
            "Sovereign Spiked Shield",
            "Golden Buckler",
            "Baroque Round Shield",
            "Angelic Kite Shield",
            "Crested Tower Shield",
            "Lacewood Spirit Shield",
            "Ironwood Buckler",
            "Alder Spiked Shield",
            "Shagreen Tower Shield",
            "Teak Round Shield",
            "Branded Kite Shield",
            "Fossilised Spirit Shield",
            "Lacquered Buckler",
            "Ebony Tower Shield",
            "Champion Kite Shield",
            "Vaal Spirit Shield",
            "Spiny Round Shield",
            "Ezomyte Spiked Shield",
            "Vaal Buckler",
            "Ezomyte Tower Shield",
            "Mosaic Kite Shield",
            "Harmonic Spirit Shield",
            "Cardinal Round Shield",
            "Mirrored Spiked Shield",
            "Crusader Buckler",
            "Colossal Tower Shield",
            "Archon Kite Shield",
            "Titanium Spirit Shield",
            "Imperial Buckler",
            "Supreme Spiked Shield",
            "Pinnacle Tower Shield",
            "Elegant Round Shield",
        };
    }

    public static class BaseTypesBows
    {
        public static string[] BaseTypes =
        {
            "Crude Bow",
            "Short Bow",
            "Long Bow",
            "Composite Bow",
            "Recurve Bow",
            "Bone Bow",
            "Royal Bow",
            "Death Bow",
            "Grove Bow",
            "Reflex Bow",
            "Decurve Bow",
            "Compound Bow",
            "Sniper Bow",
            "Ivory Bow",
            "Highborn Bow",
            "Decimation Bow",
            "Thicket Bow",
            "Steelwood Bow",
            "Citadel Bow",
            "Ranger Bow",
            "Assassin Bow",
            "Spine Bow",
            "Imperial Bow",
            "Harbinger Bow",
            "Maraketh Bow",
        };
    }

    public static class BaseTypesClaws
    {
        public static string[] BaseTypes =
        {
            "Nailed Fist",
            "Sharktooth Claw",
            "Awl",
            "Cat's Paw",
            "Blinder",
            "Timeworn Claw",
            "Sparkling Claw",
            "Fright Claw",
            "Double Claw",
            "Thresher Claw",
            "Gouger",
            "Tiger's Paw",
            "Gut Ripper",
            "Prehistoric Claw",
            "Noble Claw",
            "Eagle Claw",
            "Twin Claw",
            "Great White Claw",
            "Throat Stabber",
            "Hellion's Paw",
            "Eye Gouger",
            "Vaal Claw",
            "Imperial Claw",
            "Terror Claw",
            "Gemini Claw",
        };
    }

    public static class BaseTypesDaggers
    {
        public static string[] BaseTypes =
        {
            "Glass Shank",
            "Skinning Knife",
            "Carving Knife",
            "Stiletto",
            "Boot Knife",
            "Copper Kris",
            "Skean",
            "Imp Dagger",
            "Flaying Knife",
            "Prong Dagger",
            "Butcher Knife",
            "Poignard",
            "Boot Blade",
            "Golden Kris",
            "Royal Skean",
            "Fiend Dagger",
            "Trisula",
            "Gutting Knife",
            "Slaughter Knife",
            "Ambusher",
            "Ezomyte Dagger",
            "Platinum Kris",
            "Imperial Skean",
            "Demon Dagger",
            "Sai",
        };
    }

    public static class BaseTypesOneHandedAxes
    {
        public static string[] BaseTypes =
        {
            "Rusted Hatchet",
            "Jade Hatchet",
            "Boarding Axe",
            "Cleaver",
            "Broad Axe",
            "Arming Axe",
            "Decorative Axe",
            "Spectral Axe",
            "Etched Hatchet",
            "Jasper Axe",
            "Tomahawk",
            "Wrist Chopper",
            "War Axe",
            "Chest Splitter",
            "Ceremonial Axe",
            "Wraith Axe",
            "Engraved Hatchet",
            "Karui Axe",
            "Siege Axe",
            "Reaver Axe",
            "Butcher Axe",
            "Vaal Hatchet",
            "Royal Axe",
            "Infernal Axe",
            "Runic Hatchet",
        };
    }

    public static class BaseTypesOneHandedMaces
    {
        public static string[] BaseTypes =
        {
            "Driftwood Club",
            "Tribal Club",
            "Spiked Club",
            "Stone Hammer",
            "War Hammer",
            "Bladed Mace",
            "Ceremonial Mace",
            "Dream Mace",
            "Wyrm Mace",
            "Petrified Club",
            "Barbed Club",
            "Rock Breaker",
            "Battle Hammer",
            "Flanged Mace",
            "Ornate Mace",
            "Phantom Mace",
            "Dragon Mace",
            "Ancestral Club",
            "Tenderizer",
            "Gavel",
            "Legion Hammer",
            "Pernarch",
            "Auric Mace",
            "Nightmare Mace",
            "Behemoth Mace",
        };
    }

    public static class BaseTypesOneHandedSwords
    {
        public static string[] BaseTypes =
        {
            "Rusted Sword",
            "Copper Sword",
            "Sabre",
            "Broad Sword",
            "War Sword",
            "Ancient Sword",
            "Elegant Sword",
            "Dusk Blade",
            "Hook Sword",
            "Variscite Blade",
            "Cutlass",
            "Baselard",
            "Battle Sword",
            "Elder Sword",
            "Graceful Sword",
            "Twilight Blade",
            "Grappler",
            "Gemstone Sword",
            "Corsair Sword",
            "Gladius",
            "Legion Sword",
            "Vaal Blade",
            "Eternal Sword",
            "Midnight Blade",
            "Tiger Hook",
        };
    }

    public static class BaseTypesSceptres
    {
        public static string[] BaseTypes =
        {
            "Driftwood Sceptre",
            "Darkwood Sceptre",
            "Bronze Sceptre",
            "Quartz Sceptre",
            "Iron Sceptre",
            "Ochre Sceptre",
            "Ritual Sceptre",
            "Shadow Sceptre",
            "Grinning Fetish",
            "Horned Sceptre",
            "Sekhem",
            "Crystal Sceptre",
            "Lead Sceptre",
            "Blood Sceptre",
            "Royal Sceptre",
            "Abyssal Sceptre",
            "Stag Sceptre",
            "Karui Sceptre",
            "Tyrant's Sekhem",
            "Opal Sceptre",
            "Platinum Sceptre",
            "Vaal Sceptre",
            "Carnal Sceptre",
            "Void Sceptre",
            "Sambar Sceptre",
        };
    }

    public static class BaseTypesStaffs
    {
        public static string[] BaseTypes =
        {
            "Gnarled Branch",
            "Primitive Staff",
            "Long Staff",
            "Iron Staff",
            "Coiled Staff",
            "Royal Staff",
            "Vile Staff",
            "Crescent Staff",
            "Woodful Staff",
            "Quarterstaff",
            "Military Staff",
            "Serpentine Staff",
            "Highborn Staff",
            "Foul Staff",
            "Moon Staff",
            "Primordial Staff",
            "Lathi",
            "Ezomyte Staff",
            "Maelström Staff",
            "Imperial Staff",
            "Judgement Staff",
            "Eclipse Staff",
        };
    }

    public static class BaseTypesThrustingOneHandedSwords
    {
        public static string[] BaseTypes =
        {
            "Rusted Spike",
            "Whalebone Rapier",
            "Battered Foil",
            "Basket Rapier",
            "Jagged Foil",
            "Antique Rapier",
            "Elegant Foil",
            "Thorn Rapier",
            "Smallsword",
            "Wyrmbone Rapier",
            "Burnished Foil",
            "Estoc",
            "Serrated Foil",
            "Primeval Rapier",
            "Fancy Foil",
            "Apex Rapier",
            "Courtesan Sword",
            "Dragonbone Rapier",
            "Tempered Foil",
            "Pecoraro",
            "Spiraled Foil",
            "Vaal Rapier",
            "Jewelled Foil",
            "Harpy Rapier",
            "Dragoon Sword",
        };
    }

    public static class BaseTypesTwoHandedAxes
    {
        public static string[] BaseTypes =
        {
            "Stone Axe",
            "Jade Chopper",
            "Woodsplitter",
            "Poleaxe",
            "Double Axe",
            "Gilded Axe",
            "Shadow Axe",
            "Dagger Axe",
            "Jasper Chopper",
            "Timber Axe",
            "Headsman Axe",
            "Labrys",
            "Noble Axe",
            "Abyssal Axe",
            "Karui Chopper",
            "Talon Axe",
            "Sundering Axe",
            "Ezomyte Axe",
            "Vaal Axe",
            "Despot Axe",
            "Void Axe",
            "Fleshripper",
        };
    }

    public static class BaseTypesTwoHandedMaces
    {
        public static string[] BaseTypes =
        {
            "Driftwood Maul",
            "Tribal Maul",
            "Mallet",
            "Sledgehammer",
            "Jagged Maul",
            "Brass Maul",
            "Fright Maul",
            "Morning Star",
            "Totemic Maul",
            "Great Mallet",
            "Steelhead",
            "Spiny Maul",
            "Plated Maul",
            "Dread Maul",
            "Solar Maul",
            "Karui Maul",
            "Colossus Mallet",
            "Piledriver",
            "Meatgrinder",
            "Imperial Maul",
            "Terror Maul",
            "Coronal Maul",
        };
    }

    public static class BaseTypesTwoHandedSwords
    {
        public static string[] BaseTypes =
        {
            "Corroded Blade",
            "Longsword",
            "Bastard Sword",
            "Two-Handed Sword",
            "Etched Greatsword",
            "Ornate Sword",
            "Spectral Sword",
            "Curved Blade",
            "Butcher Sword",
            "Footman Sword",
            "Highland Blade",
            "Engraved Greatsword",
            "Tiger Sword",
            "Wraith Sword",
            "Lithe Blade",
            "Headman's Sword",
            "Reaver Sword",
            "Ezomyte Blade",
            "Vaal Greatsword",
            "Lion Sword",
            "Infernal Sword",
            "Exquisite Blade",
        };
    }

    public static class BaseTypesWands
    {
        public static string[] BaseTypes =
        {
            "Driftwood Wand",
            "Goat's Horn",
            "Carved Wand",
            "Quartz Wand",
            "Spiraled Wand",
            "Sage Wand",
            "Pagan Wand",
            "Faun's Horn",
            "Engraved Wand",
            "Crystal Wand",
            "Serpent Wand",
            "Omen Wand",
            "Heathen Wand",
            "Demon's Horn",
            "Imbued Wand",
            "Opal Wand",
            "Tornado Wand",
            "Prophecy Wand",
            "Profane Wand",
        };
    }
}