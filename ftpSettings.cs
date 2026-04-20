using ftp;
using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
namespace ftp
{
    public class FtpSettings : ObservableObject
    {
        private Dictionary<string,bool> _PlatformcheckBoxes = null;
        public Dictionary<string,bool> CheckBoxes
        {
            get => _PlatformcheckBoxes;set=>SetValue(ref _PlatformcheckBoxes, value);
        }

        private Dictionary<string, bool> extensions = null;
        public Dictionary <string, bool> Extensions
        {
            get => extensions;set => SetValue(ref extensions, value);
        }
        private string _ftpServer = string.Empty;
        public string FtpServer
        {
            get => _ftpServer;
            set => SetValue(ref _ftpServer, value);
        }

        private int _ftpPort;
        public int FtpPort
        {
            get => _ftpPort;
            set => SetValue(ref _ftpPort, value);

        }

        private string _ftpUsername = string.Empty;
        public string FtpUsername
        {
            get => _ftpUsername;
            set => SetValue(ref _ftpUsername, value);
        }

        private string _ftpPassword = string.Empty;

        public string FtpPassword
        {
            get => _ftpPassword;
            set=> SetValue(ref _ftpPassword, value);
        }
       

        private string _tempPath = string.Empty ;

        public string TempPath
        {
            get => _tempPath;
            set=>SetValue(ref _tempPath,value);       
        }

        private string _serverbasepath=string.Empty ;
        public string ServerBasepath
        {
            get=>_serverbasepath;set=>SetValue(ref _serverbasepath,value);
        }

        private int _mingamesize = 5000;

        public int Mingamesize
        {
            get => _mingamesize;set=>SetValue(ref _mingamesize,value);
        }
    }
        public class FtpSettingsViewModel : ObservableObject, ISettings
        {
            private readonly Ftp plugin;
            private FtpSettings editingClone { get; set; }

            private FtpSettings settings;
            public FtpSettings Settings
            {
                get => settings;
                set
                {
                    settings = value;
                    OnPropertyChanged();
                }
            }

            public FtpSettingsViewModel(Ftp plugin)
            {
                // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
                this.plugin = plugin;
            
                // Load saved settings.
                var savedSettings = plugin.LoadPluginSettings<FtpSettings>();

                // LoadPluginSettings returns null if not saved data is available.
                if (savedSettings != null)
                {
                    Settings = savedSettings;
                    Settings.FtpPassword=ftp.Encryption.UnprotectData(savedSettings.FtpPassword);
                }
                else
                {
                    Settings = new FtpSettings();
                }
            }

            public void BeginEdit()
            {
                // Code executed when settings view is opened and user starts editing values.
                editingClone = Serialization.GetClone(Settings);
            }

            public void CancelEdit()
            {
                // Code executed when user decides to cancel any changes made since BeginEdit was called.
                // This method should revert any changes made to Option1 and Option2.
                Settings = editingClone;
            }

            public void EndEdit()
            {
                // Code executed when user decides to confirm changes made since BeginEdit was called.
                // This method should save settings made to Option1 and Option2.
                var  crypted =Serialization.GetClone(Settings);
                crypted.FtpPassword = ftp.Encryption.ProtectData(Settings.FtpPassword);
                plugin.SavePluginSettings(crypted);
            }

            public bool VerifySettings(out List<string> errors)
            {
                // Code execute when user decides to confirm changes made since BeginEdit was called.
                // Executed before EndEdit is called and EndEdit is not called if false is returned.
                // List of errors is presented to user if verification fails.
                errors = new List<string>();
                return true;
            }
        }
    }
