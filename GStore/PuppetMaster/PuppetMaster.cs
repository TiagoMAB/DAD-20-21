using PuppetMaster.Exceptions;
using System;
using System.IO;
using System.Windows.Forms;

namespace PuppetMaster {
    public partial class PuppetMaster : Form {
        public PuppetMaster() {
            InitializeComponent();
        }

        private void LoadPMFileClick(object sender, EventArgs e) {
            string path = PMFile.Text.Trim();

            if (path.Equals("")) {
                MessageBox.Show("Path cannot be empty", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!File.Exists(path)) {
                MessageBox.Show(String.Format("File \"{0}\" does not exist", path), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                Parser.parseScript(path).Execute();
            } catch (PartitionParameterNumberMismatchException ex) {
                MessageBox.Show(String.Format("Partition command expected {0} server ids, but {1} were given", ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownCommandException ex) {
                MessageBox.Show(String.Format("Unknown command '{0}' in given file", ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (WrongArgumentNumberException ex) {
                MessageBox.Show(String.Format("Command '{0}' is executed with {1} parameters, but {2} were given", ex.Command, ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PMFile_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
