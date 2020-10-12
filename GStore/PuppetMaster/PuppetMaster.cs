using PuppetMaster.Exceptions;
using System;
using System.IO;
using System.Windows.Forms;

namespace PuppetMaster {
    public partial class PuppetMaster : Form {
        public PuppetMaster() {
            InitializeComponent();
        }

        private async void LoadPMFileClick(object sender, EventArgs e) {
            string path = PMFile.Text.Trim();

            if (path.Equals("")) {
                MessageBox.Show("Path cannot be empty", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if (!File.Exists(path)) {
                MessageBox.Show(String.Format("File \"{0}\" does not exist", path), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                await Parser.parseScript(this, path).Execute();
            } catch (PartitionParameterNumberMismatchException ex) {
                MessageBox.Show(String.Format("Partition command expected {0} server ids, but {1} were given", ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownCommandException ex) {
                MessageBox.Show(String.Format("Unknown command '{0}' in given file", ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (WrongArgumentNumberException ex) {
                MessageBox.Show(String.Format("Command '{0}' is executed with {1} parameters, but {2} were given", ex.Command, ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectPMFileClick(object sender, EventArgs e) {
            SelectPMFile.ShowDialog();
        }

        private void SelectedPMFile(object sender, System.ComponentModel.CancelEventArgs e) {
            PMFile.Text = SelectPMFile.FileName;
        }

        public void Log(string entry) {
            Logs.Items.Add(String.Format("[{0}] {1}", DateTime.Now.ToString(), entry));
            Logs.SelectedIndex = Logs.Items.Count - 1;
        }
    }
}
