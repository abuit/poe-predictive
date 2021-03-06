﻿using System;
using System.Diagnostics;
using LiteDB;
using Newtonsoft.Json;

namespace POEStash.Model
{
    [JsonObject]
    public class JsonPOEItem
    {
        private string typeLine = "";
        private string name = "";

        [JsonProperty("verified")]
        public bool Verified { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("ilvl")]
        public int ItemLevel { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; } = "";

        [JsonProperty("league")]
        public string League { get; set; } = "";

        [JsonProperty("id")]
        public string ID { get; set; } = "";

        [JsonProperty("sockets")]
        public JsonPOESocket[] JsonPoeSockets { get; set; } = new JsonPOESocket[0];

        [JsonProperty("name")]
        public string Name
        {
            get { return name; }
            set
            {
                name = StripLocalization(value);
                IsUnique = BaseTypes.IsUnique(name);
            }
        }

        [JsonProperty("typeLine")]
        public string TypeLine
        {
            get { return typeLine; }
            set
            {
                typeLine = StripLocalization(value);
                ItemType = BaseTypes.GetItemType(typeLine);
                if (ItemType == ItemType.Unknown)
                {
                    Debug.WriteLine($"Unknown ItemType! Name: {Name}. TypeLine: {TypeLine}");
                }
            }
        }

        [JsonProperty("identified")]
        public bool Identified { get; set; }

        [JsonProperty("corrupted")]
        public bool Corrupted { get; set; }

        [JsonProperty("lockedToCharacter")]
        public bool LockedToCharacter { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; } = "";

        [JsonProperty("properties")]
        public JsonPOEProperty[] JsonPoeProperties { get; set; } = new JsonPOEProperty[0];

        [JsonProperty("requirements")]
        public JsonPOEProperty[] Requirements { get; set; } = new JsonPOEProperty[0];

        [JsonProperty("explicitMods")]
        public string[] ExplicitMods { get; set; } = new string[0];

        [JsonProperty("implicitMods")]
        public string[] ImplicitMods { get; set; } = new string[0];

        [JsonProperty("enchantMods")]
        public string[] EnchantMods { get; set; } = new string[0];

        [JsonProperty("craftedMods")]
        public string[] CraftedMods { get; set; } = new string[0];

        [JsonProperty("flavourText")]
        public string[] FlavourText { get; set; } = new string[0];

        [JsonProperty("frameType")]
        public FrameType FrameType { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("inventoryId")]
        public string InventoryID { get; set; } = "";

        [JsonProperty("socketedItems")]
        public JsonPOEItem[] SocketedJsonPoeItems { get; set; } = new JsonPOEItem[0];

        [JsonProperty("additionalProperties")]
        public JsonPOEProperty[] AdditionalJsonPoeProperties { get; set; } = new JsonPOEProperty[0];

        [JsonProperty("secDescrText")]
        public string SecondaryDescription { get; set; } = "";

        [JsonProperty("descrText")]
        public string Description { get; set; } = "";

        [JsonProperty("artFilename")]
        public string ArtFileName { get; set; } = "";

        [JsonProperty("duplicated")]
        public bool Duplicated { get; set; }

        [JsonProperty("stackSize")]
        public int StackSize { get; set; }

        [JsonProperty("maxStackSize")]
        public int MaxStackSize { get; set; }

        [JsonProperty("nextLevelRequirements")]
        public JsonPOEProperty[] NextLevelRequirements { get; set; } = new JsonPOEProperty[0];

        [JsonProperty("talismanTier")]
        public int TalismanTier { get; set; }

        [JsonProperty("utilityMods")]
        public string[] UtilityMods { get; set; } = new string[0];

        [JsonProperty("support")]
        public bool Support { get; set; }

        [JsonProperty("cosmeticMods")]
        public string[] CosmeticMods { get; set; } = new string[0];

        [JsonProperty("prophecyDiffText")]
        public string ProphecyDifficultyText { get; set; } = "";

        [JsonProperty("prophecyText")]
        public string ProphecyText { get; set; } = "";

        [JsonProperty("isRelic")]
        public bool IsRelic { get; set; }

        [BsonIgnore]
        public JsonPOEStash Stash { get; set; }

        [BsonIgnore]
        public Currency.Currency Price
        {
            get
            {
                var price = Currency.Currency.Parse(Note);

                if (price.IsEmpty() && Stash != null)
                {
                    price = Stash.Price;
                }

                return price;
            }
        }

        [BsonIgnore]
        public ItemType ItemType { get; private set; } = ItemType.Unknown;

        [BsonIgnore]
        public bool IsUnique { get; private set; } = false;

        public JsonPOEItem() { }

        private string StripLocalization(string input)
        {
            if (input.StartsWith("<<set:MS>><<set:M>><<set:S>>"))
            {
                return input.Substring(28, input.Length - 28);
            }

            return input;
        }
    }
}
