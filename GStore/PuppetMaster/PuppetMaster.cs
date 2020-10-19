using Grpc.Core;
using PuppetMaster.Commands;
using PuppetMaster.Exceptions;
using PuppetMaster.MVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster {
    public partial class PuppetMaster : Form {
        const int PORT = 10001;
        private readonly Grpc.Core.Server server;
        private readonly ServerController controller;

        public PuppetMaster() {
            InitializeComponent();

            this.server = new Grpc.Core.Server {
                Services = { GStore.Status.BindService(new StatusImpl(this)) },
                Ports    = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) },
            };

            server.Start();

            this.Log("Status service started successfully");

            List<ComboBox> views = new List<ComboBox> {
                crashSelector,
                freezeSelector,
                unfreezeSelector
            };

            this.controller = new ServerController(this, views);
        }

        private void SelectPMFileClick(object sender, EventArgs e) {
            SelectPMFile.ShowDialog();
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
                await Task.Run(async () => await Parser.parseScript(this, path).Execute().ConfigureAwait(false));
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

        private void SelectedPMFile(object sender, System.ComponentModel.CancelEventArgs e) {
            PMFile.Text = SelectPMFile.FileName;
        }

        public void AddServer(string name, string URL) {
            this.controller.AddServer(name, URL);
        }

        public void RemoveServer(string name) {
            this.controller.RemoveServer(name);
        }

        public void Log(string entry) {
            Logs.Items.Add(String.Format("[{0}] {1}", DateTime.Now.ToString(), entry));
            Logs.SelectedIndex = Logs.Items.Count - 1;
        }

        public void AddToCombo(ComboBox box, string item) {
            box.Items.Add(item);
        }

        public void UpdateCombo(ComboBox box, List<string> items) {
            box.Items.Clear();
            box.SelectedIndex = -1;
            box.Items.AddRange(items.ToArray());
        }

        private async void PM_Closing(object sender, FormClosingEventArgs e) {
            await this.server.ShutdownAsync();
        }

        private async Task CommandHandler(Command command) {
            try {
                await Task.Run(async () => await command.Execute().ConfigureAwait(false));
            } catch (InvalidURLException ex) {
                MessageBox.Show(String.Format("Invalid address '{0}' on command '{1}'", ex.Url, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (PartitionParameterNumberMismatchException ex) {
                MessageBox.Show(String.Format("Partition command expected {0} server ids, but {1} were given", ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (UnknownServerException ex) {
                MessageBox.Show(String.Format("Unknown server '{0}' in command '{1}'", ex.Id, ex.Command), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (WrongArgumentNumberException ex) {
                MessageBox.Show(String.Format("Command '{0}' is executed with {1} parameters, but {2} were given", ex.Command, ex.Expected, ex.Given), "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private async void CrashBtn_Click(object sender, EventArgs e) {
            if (crashSelector.SelectedIndex == -1) {
                MessageBox.Show("Select a server to crash", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command = new Crash(this, crashSelector.Text);
            await CommandHandler(command);
        }

        private async void FreezeBtn_Click(object sender, EventArgs e) {
            if (freezeSelector.SelectedIndex == -1) {
                MessageBox.Show("Select a server to freeze", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command = new Freeze(this, freezeSelector.Text);
            await CommandHandler(command);
        }

        private async void UnfreezeBtn_Click(object sender, EventArgs e) {
            if (unfreezeSelector.SelectedIndex == -1) {
                MessageBox.Show("Select a server to unfreeze", "PuppetMaster", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Command command = new Unfreeze(this, unfreezeSelector.Text);
            await CommandHandler(command);
        }
    }
}
