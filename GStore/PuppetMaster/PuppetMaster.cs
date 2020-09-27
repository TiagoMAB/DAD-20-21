using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class PuppetMaster : Form
    {
        public PuppetMaster()
        {
            InitializeComponent();
        }

        private void loadPMFileClick(object sender, EventArgs e)
        {
            string path = PMFile.Text;
            
            if(path.Equals(""))
            {
                // TODO: Handle error
                return;
            }
            else if(!File.Exists(path))
            {
                // TODO: Handle error
                return;
            }

            Parser.parseScript(path).Execute();
        }
    }
}
