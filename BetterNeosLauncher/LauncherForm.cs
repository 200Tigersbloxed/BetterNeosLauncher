using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace BetterNeosLauncher
{
    public partial class LauncherForm : Form
    {
        // We need to find the Steam Directory First
        public string SteamInstallPath = String.Empty;
        public string NeosInstallLocation = String.Empty;
        Config config = new Config();

        public LauncherForm()
        {
            InitializeComponent();
        }

        public async Task Initialize()
        {
            // Config
            string cs = await ConfigHelper.ReadConfigFile();
            config = ConfigHelper.LoadConfig(cs);
            if (!ConfigHelper.VerifyConfig(config))
            {
                MessageBox.Show("Failed to validate JSON Schema in Config file! Did you have a typo?", "Failed to load config.json!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                config = new Config();
            }
            if (config.NeosPath == String.Empty)
            {
                SteamInstallPath = SteamHelper.FindSteamPath();
                NeosInstallLocation = SteamHelper.FindNeosDir(SteamHelper.CreateSteamAppsManager(SteamInstallPath));
            }
            else
            {
                NeosInstallLocation = config.NeosPath;
            }
            DirectoryTextBox.Text = PrettyFormat.Directory(NeosInstallLocation);
            // Data Path
            if(config.DataPath != String.Empty)
            {
                DataPathTextBox.Text = config.DataPath;
            }
            else
            {
                DataPathTextBox.Text = "";
            }
            // Cache Path
            if(config.CachePath != String.Empty)
            {
                CachePathTextBox.Text = config.CachePath;
            }
            else
            {
                CachePathTextBox.Text = "";
            }
            // SRanipal
            if(config.SRanipalPath != String.Empty)
            {
                SRanipalPathTextBox.Text = config.SRanipalPath;
            }
            else
            {
                SRanipalPathTextBox.Text = SRAHelper.GetSRAnipalEXE();
            }
            // Launch Options
            if(config.OtherLaunchOptions != String.Empty)
            {
                LaunchOptionsTextBox.Text = config.OtherLaunchOptions;
            }
            else
            {
                LaunchOptionsTextBox.Text = "";
            }
        }

        public void RefreshPlugins()
        {
            if (NeosInstallLocation != null)
            {
                if (Directory.Exists(NeosInstallLocation + "/Libraries"))
                {
                    PluginsCheckBoxList.Items.Clear();
                    foreach (string fileName in Directory.GetFiles(NeosInstallLocation + "/Libraries"))
                    {
                        PluginsCheckBoxList.Items.Add(Path.GetFileName(fileName), false);
                    }
                }
            }
        }

        private async void LauncherForm_Load(object sender, EventArgs e)
        {
            await Initialize();
            ModeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ModeComboBox.SelectedIndex = 3;
            RefreshPlugins();
        }

        private async void SelectDirectoryButton_Click(object sender, EventArgs e)
        {
            using(FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if(result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Config newConfig = config;
                    newConfig.NeosPath = fbd.SelectedPath;
                    await ConfigHelper.SaveConfig(newConfig);
                    await Initialize();
                }
            }
        }

        private void LaunchSRAButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (config.SRanipalPath == string.Empty)
                    Process.Start(SRAHelper.GetSRAnipalEXE());
                else
                    Process.Start(config.SRanipalPath);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not find the SRanipal application!", "SRAHelper", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void SetSRLocation_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Title = "Select SRanipal",
                Filter = "Application Files (*.exe)|*.exe" + "|" + "All Files (*.*)|*.*"
            };
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openDialog.FileName;
                config.SRanipalPath = file;
                await ConfigHelper.SaveConfig(config);
                await Initialize();
            }
        }

        private async void SetDataPathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    config.DataPath = fbd.SelectedPath;
                    await ConfigHelper.SaveConfig(config);
                    await Initialize();
                }
            }
        }

        private async void ResetDataPathConfigButton_Click(object sender, EventArgs e)
        {
            Config newConfig = config;
            newConfig.DataPath = String.Empty;
            await ConfigHelper.SaveConfig(newConfig);
            await Initialize();
        }

        private async void SetCachePathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    config.CachePath = fbd.SelectedPath;
                    await ConfigHelper.SaveConfig(config);
                    await Initialize();
                }
            }
        }

        private async void ResetCachePathConfigButton_Click(object sender, EventArgs e)
        {
            Config newConfig = config;
            newConfig.CachePath = String.Empty;
            await ConfigHelper.SaveConfig(newConfig);
            await Initialize();
        }

        private async void ResetSRAConfig_Click(object sender, EventArgs e)
        {
            Config newConfig = config;
            config.SRanipalPath = String.Empty;
            await ConfigHelper.SaveConfig(newConfig);
            await Initialize();
        }

        private async void ResetConfigButton_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("THIS WILL RESET YOUR BETTERNEOSLAUNCHER CONFIG! ARE YOU SURE YOU WANT TO CONTINUE?", "WARNING!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(dr == DialogResult.Yes)
            {
                await ConfigHelper.SaveConfig(new Config());
                string cs = await ConfigHelper.ReadConfigFile();
                config = ConfigHelper.LoadConfig(cs);
                await Initialize();
            }
        }

        private void RefreshPluginsButton_Click(object sender, EventArgs e)
        {
            RefreshPlugins();
        }

        private async void LaunchButton_Click(object sender, EventArgs e)
        {
            // Setup Arguments
            string arguments = String.Empty;
            void AddArgument(string argument)
            {
                arguments = arguments + argument + " ";
            }
            // ModeComboBox Argument
            switch (ModeComboBox.SelectedIndex)
            {
                case 0:
                    AddArgument("-Screen");
                    break;
                case 1:
                    AddArgument("-SteamVR");
                    break;
                case 2:
                    AddArgument("-RiftTouch");
                    break;
            }
            // Plugins Argument
            for(int i = 0; i < PluginsCheckBoxList.Items.Count; i++)
            {
                if (PluginsCheckBoxList.GetItemChecked(i))
                {
                    AddArgument("-LoadAssembly " + "\"" + NeosInstallLocation + "\\Libraries\\" + (string)PluginsCheckBoxList.Items[i] + "\"");
                }
            }
            // Data Path
            if(!String.IsNullOrEmpty(DataPathTextBox.Text))
            {
                AddArgument("-DataPath " + DataPathTextBox.Text);
            }
            // Cache Path
            if (!String.IsNullOrEmpty(CachePathTextBox.Text))
            {
                AddArgument("-CachePath " + CachePathTextBox.Text);
            }
            // Misc
            if (ForceSRACheckbox.Checked) { AddArgument("-ForceSRAnipal"); }
            if (LegacyScreenCheckBox.Checked) { AddArgument("-LegacyScreen"); }
            if (InvisibleCheckBox.Checked) { AddArgument("-Invisible"); }
            if (ForceNoVoiceCheckBox.Checked) { AddArgument("-ForceNoVoice"); }
            // Other Launch Options
            arguments = arguments + LaunchOptionsTextBox.Text;
            Console.WriteLine(arguments);
            // Start Neos
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = NeosInstallLocation + @"\Neos.exe";
            startInfo.Arguments = arguments;
            Process.Start(startInfo);
            // Save things that save automatically
            Config newConfig = config;
            newConfig.OtherLaunchOptions = LaunchOptionsTextBox.Text;
            await ConfigHelper.SaveConfig(newConfig);
        }

        private async void ResetDirectoryCacheButton_Click(object sender, EventArgs e)
        {
            Config newConfig = config;
            newConfig.NeosPath = String.Empty;
            await ConfigHelper.SaveConfig(newConfig);
            await Initialize();
        }
    }
}
