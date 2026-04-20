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
            logger.Info("starting ftp extension");
            string ftpPath = null;
            string localFilePath = null;
            bool directory = false;
            if (ftp)
            {
                ftpPath = args.Game.InstallDirectory.Replace(args.Game.GetInstallDrive(), settings.Settings.ServerBasepath);
                directory = true;
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
                            
                                        client.DownloadFile(localFilePath, ftpPath, FluentFTP.FtpLocalExists.Overwrite, FluentFTP.FtpVerify.None, progress);
                                        break;
                                    case true:
                                        client.DownloadDirectory(localFilePath, ftpPath, FluentFTP.FtpFolderSyncMode.Mirror,FluentFTP.FtpLocalExists.Overwrite,FluentFTP.FtpVerify.None,null,progress);
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
                        PlayniteApi.Dialogs.ShowErrorMessage("ERROR");
                        logger.Error(ex, $"Error downloading ROM for game {args.Game.Name}");
                        logger.Info($"Failed to download ROM for game {args.Game.Name}: {ex.Message}");
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
            var roms = args.Game.Roms;
            bool enabled = false;
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
                  
                }
            }
                //get default
                if (CheckDrive.IsNetworkDrive(args.Game.InstallDirectory) & BytesToMB(args.Game.InstallSize) >= settings.Settings.Mingamesize && enabled)
                {
                    ftp = true;



                    var emuid = action.EmulatorId;
                    var profileid = action.EmulatorProfileId;
                    var emulatorName = _api.Database.Emulators.Get(emuid).Name;
                    var profileName = _api.Database.Emulators.Get(emuid).GetProfile(profileid);
                    var builtin = _api.Database.Emulators.Get(emuid).BuiltinProfiles.ToList();
                    var profile = builtin.Find(p => p.Id == profileid);
                    var emulator = _api.Database.Emulators.Get(emuid);
                    var definitions = _api.Emulation.GetEmulator(emulatorName).Profiles.ToList();
                    var definition = definitions.Find(p => p.Name == profileName.Name);
                    var builtinargs = definition.StartupArguments;


                    string act = settings.Settings.TempPath + "/{ImageName}";
                    var newargs = builtinargs.Replace("\"{ImagePath}\"", $"\"{act}\"");

                    string arguments = newargs;

                    action.Arguments = arguments;
                    action.OverrideDefaultArgs = true;
                }
                else
                {
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