using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;


namespace ftp
{
    public class PlatformRomsChecker
    {
        private readonly ILogger _logger;
        private readonly IPlayniteAPI _api;

        public PlatformRomsChecker(IPlayniteAPI api)
        {
            _api = api;
        }

        public Dictionary<string, bool> CheckPlatformsForRoms()
        {
            var platformsWithRoms = new Dictionary<string, bool>();

            try
            {
                // Get all platforms
                var platforms = _api.Database.Platforms;
                foreach (var platform in platforms)
                {
                    // Check if there are any games associated with this platform that have a ROM
                    var Filter = new FilterPresetSettings();
                    Filter.Platform = new IdItemFilterItemProperties(platform.Id);
                    bool hasRoms = false;
                    IEnumerable<Game> filtered = _api.Database.GetFilteredGames(Filter);
                    if (filtered.Any())
                    {
                        hasRoms = true;
                    }
                    platformsWithRoms[platform.Name] = hasRoms;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to check platforms for Roms");
            }

            return platformsWithRoms;
        }
        public List<string> Getextensions(Dictionary<string,bool> platforms)
        {
            List<string> result = new List<string>();
            try
            {
                var databaseplatforms = _api.Database.Platforms.ToList();
                var emulators = _api.Emulation.Emulators.ToList();
                foreach (var emulator in emulators)
                {
                    var profiles=emulator.Profiles.ToList();
                    
                    foreach(var profile in profiles)
                    {
                        foreach(var platform in profile.Platforms)
                        {
                            var platformname = databaseplatforms.Find(p => p.SpecificationId == platform);
                           
                            if(platformname!=null&&platforms.ContainsKey(platformname.Name)&&platforms[platformname.Name]==true && profile.ImageExtensions !=null)
                            {
                                foreach (var ext in profile.ImageExtensions)
                                {
                                    if (!result.Contains(ext) && ext != null)
                                    {
                                        result.Add(ext);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to check emulators for Extensions");
            }
            
            return result;
        }
    }
 


}
