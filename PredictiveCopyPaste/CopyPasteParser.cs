using POEStash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PredictiveCopyPaste
{
    public class CopyPasteParser
    {
        static readonly string WHATEVER = "WHATEVER";

        static readonly string Separator = Regex.Escape($"--------");
        static readonly string QualityText = Regex.Escape($"Quality: +{WHATEVER}% (augmented)").Replace(WHATEVER, ".*");
        static readonly string ArmourText = Regex.Escape($"Armour: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string EvasionText = Regex.Escape($"Evasion: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string EnergyShieldText = Regex.Escape($"Energy Shield: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string RequirementsText = Regex.Escape($"Requirements:");
        static readonly string LevelReqText = Regex.Escape($"Level: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string StrText = Regex.Escape($"Str: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string DexText = Regex.Escape($"Dex: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string IntText = Regex.Escape($"Int: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string SocketsText = Regex.Escape($"JsonPoeSockets: {WHATEVER}").Replace(WHATEVER, ".*");
        static readonly string ItemLevelText = Regex.Escape($"JsonPOEItem Level: {WHATEVER}{Environment.NewLine}").Replace(WHATEVER, ".*");
        static readonly string CorruptionText = Regex.Escape($"Corrupted");
        static readonly string CosmeticText = Regex.Escape($"Has {WHATEVER}").Replace(WHATEVER, ".*");

        public static IEnumerable<string> BullCrap()
        {
            yield return Separator;
            yield return QualityText;
            yield return ArmourText;
            yield return EvasionText;
            yield return EnergyShieldText;
            yield return RequirementsText;
            yield return LevelReqText;
            yield return StrText;
            yield return DexText;
            yield return IntText;
            yield return SocketsText;
            yield return ItemLevelText;
            yield return CorruptionText;
            yield return CosmeticText;
        }

        private string[] dataLines;

        public ItemType ItemType;

        public bool Corrupted;
        public string[] Implicits;
        public string[] Explicits;

        public CopyPasteParser(string data)
        {
            this.dataLines = data.Split(Environment.NewLine.ToCharArray());
        }

        public bool TryParse()
        {
            DeriveItemType();

            if (ItemType == ItemType.Unknown)
                return false;

            DeriveCorrupted();
            DeriveAffixes();

            return true;
        }

        private void DeriveItemType()
        {
            ItemType = ItemType.Unknown;
            foreach (string line in dataLines)
            {
                //If we hit a separator, we haven't found an item type that matches
                if (line.Equals(Separator))
                    break;

                //Try to parse into an item type
                ItemType parsedType = BaseTypes.GetItemType(line);
                if (parsedType != ItemType.Unknown)
                {
                    ItemType = parsedType;
                    break;
                }

            }
        }

        private void DeriveCorrupted()
        {
            Corrupted = false;
            foreach(string line in dataLines)
            {
                if (line.Equals(CorruptionText))
                {
                    Corrupted = true;
                    return;
                }
            }
        }

        private void DeriveAffixes()
        {
            bool namingDone = false;

            List<string> firstArray = new List<string>();
            bool firstArrayDone = false;

            List<string> secondArray = new List<string>();

            foreach (string line in dataLines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                //Try to hit the first separator first.
                if (!namingDone)
                {
                    if (Regex.IsMatch(line, Separator))
                        namingDone = true;
                    else
                        continue;
                }

                bool containsCrap = false;
                foreach(string bullcrapRegex in BullCrap())
                {
                    if (Regex.IsMatch(line, bullcrapRegex))
                    {
                        if (firstArray.Any())
                            firstArrayDone = true;

                        containsCrap = true;
                        break;
                    }
                }

                if (containsCrap)
                    continue;

                if (!firstArrayDone)
                    firstArray.Add(line);
                else
                    secondArray.Add(line);
            }

            Implicits = firstArray.ToArray();
            Explicits = secondArray.ToArray();
        }
    }
}
