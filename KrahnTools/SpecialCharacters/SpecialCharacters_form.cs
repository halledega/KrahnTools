using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KrahnTools.SpecialCharacters
{
    public partial class SpecialCharacters_form : Form
    {
        private string specialCharKey;
        private string specialChar;

        private Dictionary<string, string> getAltChars()
        {
            Dictionary<string, string> altChars;
            altChars = new Dictionary<string, string>();
            altChars.Add("----Drafting Symbols----", "0");
            altChars.Add("Diameter - Ø", "Ø");
            altChars.Add("Copy Right - ©", "©");
            altChars.Add("Registered- - ®", "®");
            altChars.Add("Plus Minus - ±", "±");
            altChars.Add("----Greek Letters----", "0");
            altChars.Add("Alpha - α", "α");
            altChars.Add("Beta - β", "β");
            altChars.Add("Theta - ϴ", "ϴ");
            altChars.Add("Phi - Φ", "Φ");
            altChars.Add("Pi - π", "π");
            altChars.Add("Sigma - Σ", "Σ");
            altChars.Add("----Math Symbols----", "0");
            altChars.Add("Squared - ²", "²");
            altChars.Add("Cubed - ³", "³");

            return altChars;

        }

        public SpecialCharacters_form()
        {
            Dictionary<string, string> altChars = getAltChars();

        //this must come before any other code so form exists prior to manipulating items
            InitializeComponent();

        //pupulate SpecialChars_lb (listbox) with the key values the dictionary containing the special characters
            foreach (KeyValuePair<string, string> dictEntry in altChars)
            {
                SpecialChars_lb.Items.Add(dictEntry.Key);
            }
        }

        private void SpecialChars_lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dictionary<string, string> altChars = getAltChars();
            string keyValue = SpecialChars_lb.Text;

            specialCharKey = SpecialChars_lb.Text;
            specialChar = altChars[specialCharKey];

            if (specialChar != "0")
            {
                Preview_lbl.Text = specialChar;
            }
        }

        private void Paste_btn_Click(object sender, EventArgs e)
        {
            if (specialChar != "0")
            {
                try
                {
                    Clipboard.SetText(specialChar);
                }
                catch
                {

                }
                MessageBox.Show(specialChar + " copied to clipboard.","Speical Cahracter Copied");
            }
            SpecialCharacters_form.ActiveForm.Close();
        }
    }
}
