using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KrahnTools.PSDCreator
{
  
    public partial class PSDOptions_form : Form
    {

        public Boolean isStructural;

        public PSDOptions_form()
        {
            InitializeComponent();


            rtxtInstructions.Text = "Steps to create panel shop drawings: \n"
                + "\t Step 1: Make sure the PSD titleblock is loaded into the project \n"
                + "\t Step 2: Make sure the PSD_Embeds and PSD_Rebar view templates are loaded into your project \n"
                + "\t Step 3: Once everything is loaded in, Proceed to a 3D view and click OK below. "
                + "You will be promted to select a panel. Select a panel and the PSD will be automatically "
                + "created and placed on to a sheet \n"
                + "\t Step 4: Proceed to the sheet view to continue working with panels";


            
            isStructural = rdo_Struct.Checked;

        }

        
        
        private void rdo_Struct_CheckedChanged(object sender, EventArgs e)
        {
            isStructural = rdo_Struct.Checked;
        }

        private void rdo_nonStruct_CheckedChanged(object sender, EventArgs e)
        {
            isStructural = rdo_Struct.Checked;
        }
    }
}
