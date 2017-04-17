using POEStash;
using Predictive;
using PredictiveCopyPaste;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredictiveConsole
{
    public partial class ConsoleForm : Form
    {
        NetworkTrainer trainer;

        public ConsoleForm()
        {
            InitializeComponent();
            this.ConsoleTextBox.TextChanged += ScrollConsole;
            this.ConsoleTextBox.TextAlignChanged += ClearResult;
            this.Activated += ConsoleForm_Activated;

            ConsoleWriter.Instance.WriteLineEvent += WriteLineEvent;
            ConsoleWriter.Instance.WriteEvent += WriteEvent;

            StartNetworks();
        }

        private void WriteEvent(object sender, ConsoleWriterEventArgs e)
        {
            AddConsoleText(e.Value);
        }

        private void WriteLineEvent(object sender, ConsoleWriterEventArgs e)
        {
            AddConsoleText(e.Value + Environment.NewLine);
        }

        private void AddConsoleText(string text)
        {
            if (this.ConsoleTextBox.InvokeRequired)
                this.ConsoleTextBox.BeginInvoke((Action)(() => AddConsoleText(text)));
            else
                this.ConsoleTextBox.Text += text;
        }

        private void StartNetworks()
        {
            //Based off of http://www.poeex.info/index/index/table/1
            var rates = new Dictionary<CurrencyType, float>()
            {
                { CurrencyType.ChromaticOrb, 1f/13f },
                { CurrencyType.OrbOfAlteration, 1f/13f },
                { CurrencyType.JewellersOrb, 1f/7f },
                { CurrencyType.OrbOfChance, 1f/5f },
                { CurrencyType.CartographersChisel, 1/4f },
                { CurrencyType.OrbOfFusing, 1f/2f },
                { CurrencyType.OrbOfAlchemy, 1f/3.5f },
                { CurrencyType.OrbOfScouring, 1f/1.8f },
                { CurrencyType.BlessedOrb, 1f/1.3f },
                { CurrencyType.OrbOfRegret, 1f },
                { CurrencyType.RegalOrb, 1.3f},
                { CurrencyType.GemcuttersPrism, 1.8f},
                { CurrencyType.DivineOrb, 17f},
                { CurrencyType.ExaltedOrb, 50f},
                { CurrencyType.VaalOrb, 1f},
            };
            var conversionTable = new ConversionTable(rates);

            trainer = new NetworkTrainer(conversionTable);
            trainer.StartTraining();
        }

        private void ConsoleForm_Activated(object sender, EventArgs e)
        {
            InputData.Focus();
        }

        private void ClearResult(object sender, EventArgs e)
        {
            ResultLabel.Text = string.Empty;
            DeterminePrice.Enabled = true;
        }

        private void ScrollConsole(object sender, EventArgs e)
        {
            this.ConsoleTextBox.SelectionStart = this.ConsoleTextBox.TextLength;
            this.ConsoleTextBox.ScrollToCaret();
        }

        private void DeterminePrice_Click(object sender, EventArgs e)
        {
            CopyPasteParser parser = new CopyPasteParser(this.InputData.Text);
            if (parser.TryParse())
            {
                if (!NetworkTrainer.SupportedItemTypes.Contains(parser.ItemType))
                {
                    ResultLabel.Text = $"The item type {parser.ItemType} is not yet supported";
                    return;
                }
                else
                {
                    Predictive.ParsedItem b = new Predictive.ParsedItem(parser.Corrupted, parser.Implicits, parser.Explicits);
                    trainer.GetItemNetwork(parser.ItemType).PredictBelt(b);
                    ResultLabel.Text = $"{b.CalculatedPrice} Chaos";
                }
            }
            else
            {
                ResultLabel.Text = "Couldn't parse item";
                return;
            }
        }
    }
}
