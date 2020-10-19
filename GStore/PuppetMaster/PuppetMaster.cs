using Grpc.Core;
using PuppetMaster.Exceptions;
using System;
using System.IO;
using System.Windows.Forms;

namespace PuppetMaster {
    public partial class PuppetMaster : Form {
        const int PORT = 10001;

        public PuppetMaster() {
            InitializeComponent();

            Server server = new Server {
                Services = { GStore.Status.BindService(new StatusImpl(this)) },
                Ports    = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) },
            };

            server.Start();

            this.Log("Status service started successfully");
        }

        private async void LoadPMFileClick(object sender, EventArgs e) {
            string path = PMFile.Text.Trim();

            if (path.Equals("")) {
                MessageBox.Show("Path cannot be empty", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else if (!File.Exists(path)) {
                MessageBox.Show(String.Format("File \"{0}\" does not exist", path), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try {
                // TODO: Avoid locking the GUI thread
                await Parser.parseScript(this, path).Execute();
            } catch (InvalidURLException ex) {
                MessageBox.Show(String.Format("Invalid address '{0}' on command '{1}'", ex.Url, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (PartitionParameterNumberMismatchException ex) {
                MessageBox.Show(String.Format("Partition command expected {0} server ids, but {1} were given", ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownCommandException ex) {
                MessageBox.Show(String.Format("Unknown command '{0}' in given file", ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownServerException ex) {
                MessageBox.Show(String.Format("Unknown server '{0}' in command '{1}'", ex.Id, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (WrongArgumentNumberException ex) {
                MessageBox.Show(String.Format("Command '{0}' is executed with {1} parameters, but {2} were given", ex.Command, ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
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
