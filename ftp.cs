using FluentFTP;
using Playnite.SDK;
using Playnite.SDK.Events;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

namespace ftp
{
    public class Ftp : GenericPlugin
    {
        private readonly IPlayniteAPI _api;

        private static readonly ILogger logger = LogManager.GetLogger();
        private FtpSettingsViewModel settings { get; set; }
        public string downloadpath { get; set; }

        public bool ftp = false;

        
        public override Guid Id { get; } = Guid.Parse("15d4bd73-1f70-42b9-80f5-53b6539adc6a");

        public Ftp(IPlayniteAPI api) : base(api)
        {
            _api = api;
            
            
            settings = new FtpSettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
        }

        public override void OnGameInstalled(OnGameInstalledEventArgs args)
        {
            // Add code to be executed when game is finished installing.
        }

        public override void OnGameSelected(OnGameSelectedEventArgs args)
        {
            base.OnGameSelected(args);
            
        }
       
        public override void OnGameStarted(OnGameStartedEventArgs args)
        {

        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new FtpSettingsView(settings.Settings,_api);
        }
        public override void OnGameStarting(OnGameStartingEventArgs args)
        {
            bool directory = false;
            logger.Info("starting ftp extension");
            string ftpPath = null;
            string localFilePath = null;
            List<string> isdirectory = new List<string> { ".cue"};
            foreach(string dir in isdirectory)
            {
                if (Path.GetExtension(args.SelectedRomFile) == dir)
                {
                    directory = true;
                    break;
                }

            }
           
            if (ftp)
            {
                ftpPath = args.Game.InstallDirectory.Replace(args.Game.GetInstallDrive(), settings.Settings.ServerBasepath);
                // directory = true;
                var GameDirectory =new DirectoryInfo(Path.GetDirectoryName(args.Game.InstallDirectory)).Name;
                localFilePath = settings.Settings.TempPath;
                NetworkCredential credentials = new NetworkCredential(settings.Settings.FtpUsername,settings.Settings.FtpPassword);

                // Temporary directory
               
                if (!new DirectoryInfo(settings.Settings.TempPath).Exists)
                {
                    Directory.CreateDirectory(settings.Settings.TempPath);
                }

                // Download ROM using FTP
                using (var client = new FtpClient(settings.Settings.FtpServer, settings.Settings.FtpPort))
                {
                    try
                    {
                        client.Credentials = credentials;
                        client.Connect();
                        
                        downloadpath = localFilePath;
                        PlayniteApi.Dialogs.ActivateGlobalProgress((prg) =>
                        {
                            Action<FtpProgress> progress = delegate (FtpProgress p)
                            {
                                if (p.Progress == 1)
                                {
                                    // all done!
                                }
                                else
                                {
                                    prg.CurrentProgressValue = p.Progress;  // percent done = (p.Progress * 100)
                                    prg.Text = "Downloading " + p.TransferSpeedToString();
                                }
                            };
                            prg.ProgressMaxValue = 100;
                            try
                            {
                                switch (directory)
                                {
                                    case false:
                            
                                        client.DownloadFile(args.SelectedRomFile.Replace("{InstallDir}", localFilePath), args.SelectedRomFile.Replace("{InstallDir}",ftpPath), FluentFTP.FtpLocalExists.Resume, FluentFTP.FtpVerify.Retry, progress);
                                        break;
                                    case true:
                                        client.DownloadDirectory(localFilePath+"/"+GameDirectory, ftpPath, FluentFTP.FtpFolderSyncMode.Mirror,FluentFTP.FtpLocalExists.Resume,FluentFTP.FtpVerify.Retry,null,progress);
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                logger.Info(e.Message);
                               
                            }
                        }, new GlobalProgressOptions("") { IsIndeterminate = false });                            
                        logger.Info($"ROM downloaded and launched successfully for game {args.Game.Name}.");
                    }
                    catch (Exception ex)
                    {
                        PlayniteApi.Dialogs.ShowErrorMessage("ERROR Aborting");
                        logger.Error(ex, $"Error downloading ROM for game {args.Game.Name}");
                        logger.Info($"Failed to download ROM for game {args.Game.Name}: {ex.Message}");
                        args.Game.IsLaunching = false;
                    }
                    finally
                    {
                        client.Disconnect();
                    }
                }
            }
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameStopped(OnGameStoppedEventArgs args)
        {
            // Add code to be executed when game is preparing to be started.
        }

        public override void OnGameUninstalled(OnGameUninstalledEventArgs args)
        {
            // Add code to be executed when game is uninstalled.
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            // Add code to be executed when Playnite is initialized.
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            // Add code to be executed when Playnite is shutting down.
        }

        public override void OnLibraryUpdated(OnLibraryUpdatedEventArgs args)
        {
            // Add code to be executed when library is updated.
        }
        public class CheckDrive
        {
            public static bool IsNetworkDrive(string driveLetter)
            {
                DriveInfo drive = new DriveInfo(driveLetter);
                if (drive.IsReady && drive.DriveType == DriveType.Network)
                {
                    return true;
                }
                return false;
            }
        }
        public override IEnumerable<PlayController> GetPlayActions(GetPlayActionsArgs args)
        {
            
            var action = args.Game.GameActions[0];
           // action.OverrideDefaultArgs = false;
            var roms = args.Game.Roms;
            bool enabled = false;
            bool folder = false;
            foreach (var platform in args.Game.Platforms)
            {
                if (settings.Settings.CheckBoxes.ContainsKey(platform.Name) && settings.Settings.CheckBoxes[platform.Name] == true)
                {
                    enabled = true;
                    break;
                }
            }
            if (enabled)
            {
                foreach (var rom in roms)
                {
                    if (settings.Settings.Extensions[Path.GetExtension(rom.Path).TrimStart('.')] == false)
                    {
                        enabled = false;
                        break;
                    }
                    else if(Path.GetExtension(rom.Path).TrimStart('.')=="cue")
                    {
                        folder = true;
                    }
                  
                }
            }
                //get default
              
            if (CheckDrive.IsNetworkDrive(args.Game.InstallDirectory) & BytesToMB(args.Game.InstallSize) >= settings.Settings.Mingamesize && enabled)  
            {
                ftp = true;

                string builtinargs = null;

                var emuid = action.EmulatorId;
                var profileid = action.EmulatorProfileId;
                var emulatorName = _api.Database.Emulators.Get(emuid).Name;
                EmulatorProfile profileName = _api.Database.Emulators.Get(emuid).GetProfile(profileid);
                var builtin = _api.Database.Emulators.Get(emuid).BuiltinProfiles.ToList();
                var profile = builtin.Find(p => p.Id == profileid);
                var emulator = _api.Database.Emulators.Get(emuid);
                var definitions = _api.Emulation.GetEmulator(emulator.BuiltInConfigId).Profiles.ToList();
               
                var definition = definitions.Find(p => p.Name == profileName.Name);
                if (definition != null)
                {
                    builtinargs = definition.StartupArguments;
                }
                else
                {
                    var custom = _api.Database.Emulators.Get(emuid).CustomProfiles.ToList();
                    var prof= custom.Find(p => p.Name==profileName.Name);
                    builtinargs = prof.Arguments;
                }

                ///differentiate between cue and iso
                string act;
                var InstallDirectory = new DirectoryInfo(args.Game.InstallDirectory).Name;
                if (folder)
                {
                    var GameDirectory = new DirectoryInfo(Path.GetDirectoryName(args.Game.InstallDirectory)).Name;

                    act = settings.Settings.TempPath + "/" + GameDirectory + "/{ImageName}";
                }
                else
                {
                    

                    act = settings.Settings.TempPath + "/{ImageName}";
                }
                var newargs = builtinargs.Replace("\"{ImagePath}\"", $"\"{act}\"");
                ///change this to install directory
                string arguments = newargs;

                action.Arguments = arguments;
                action.OverrideDefaultArgs = true;
            }
            else
            {

                ftp = false;
                action.OverrideDefaultArgs = false;
            }
            yield break;
            
        }
        public static double BytesToMB(ulong? bytes)
        {
            return (double)bytes / (1024 * 1024);
        }

    }
}