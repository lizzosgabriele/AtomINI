using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace AtomINI {
    
    public class CachedIniData {
        public IniData data { get; set; }
        public string lastEditTimestamp { get; set; }

        public CachedIniData(IniData data_p, string lastEditTimestamp_p) {
            data = data_p;
            lastEditTimestamp = lastEditTimestamp_p;
        }
    }

    static class CacheManager {
        
        private static Dictionary<string, CachedIniData> cache = new Dictionary<string, CachedIniData>();
        private static FileIniDataParser parser = new FileIniDataParser(AtomIniSettings.config_parser);
        private static FileSystemWatcher folderWatcher;
        
        static CacheManager() {
            SetupFolderWatcher();
        }
        
        private static void SetupFolderWatcher() {
            if (folderWatcher != null) return;
            folderWatcher = new FileSystemWatcher {
                Path = AtomIniSettings.atomIniDefaultFolder,
                Filter = "*.ini",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Attributes | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            folderWatcher.Changed += OnFileChanged;
        }
        
        private static void OnFileChanged(object sender, FileSystemEventArgs e) {
            AtomIniUtils.ExtVLog("Called OnFileChanged for file {file} with change type {changeType}.", e.FullPath, e.ChangeType);
            lock (cache) {
                if (cache.ContainsKey(e.FullPath)) {
                    AtomIniUtils.ExtVLog("Detected changed INI file {file}, reloading cache.", e.FullPath);
                    ReloadFile(e.FullPath);
                }
            }
        }
        
        private static void OnFileRenamed(object sender, RenamedEventArgs e) {
            AtomIniUtils.ExtVLog("Detected renamed INI file from {oldFile} to {newFile}.", e.OldFullPath, e.FullPath);
        }
        
        private static void OnFileDeleted(object sender, FileSystemEventArgs e) {
            AtomIniUtils.ExtVLog("Called OnFileDeleted for file {file}.", e.FullPath);
            lock (cache) {
                if (cache.ContainsKey(e.FullPath)) {
                    AtomIniUtils.ExtVLog("Detected deleted INI file {file}, removing from cache.", e.FullPath);
                    cache.Remove(e.FullPath);
                }
            }
        }

        public static void UpdateFile(string iniFilePath) {
            lock (cache) {
                if (cache.ContainsKey(iniFilePath)) {
                    AtomIniUtils.ExtVLog("Writing data to ini file {iniFilePath}", iniFilePath);
                    parser.WriteFile(iniFilePath, cache[iniFilePath].data, AtomIniUtils.getActiveEncoding());
                    AtomIniUtils.ExtVLog("Data written to ini file {iniFilePath} successfully", iniFilePath);
                } else {
                    AtomIniUtils.ELog("UpdateFile called for unknown cached file {iniFilePath}", iniFilePath);
                }
            }
        }

        public static void ReloadFile(string iniFilePath) {
            try {
                IniData data_read = parser.ReadFile(iniFilePath);
                string lastEditTimestamp = File.GetLastWriteTimeUtc(iniFilePath).ToString("o");
                cache[iniFilePath] = new CachedIniData(data_read, lastEditTimestamp);
                AtomIniUtils.ExtVLog("Loaded INI file into cache: {iniFilePath}", iniFilePath);
            } catch (Exception e) {
                AtomIniUtils.ELog($"Error loading INI file {iniFilePath}: {e.Message}");
            }
        }
        
        public static bool NeedsToReload(string iniFilePath) {
            if (!cache.ContainsKey(iniFilePath)) return true;
            string currentTimestamp = File.GetLastWriteTimeUtc(iniFilePath).ToString("o");
            return cache[iniFilePath].lastEditTimestamp != currentTimestamp;
        }
        
        private static bool IsInWatchedFolder(string iniFilePath) {
            return iniFilePath.StartsWith(AtomIniSettings.atomIniDefaultFolder, StringComparison.OrdinalIgnoreCase);
        }

        public static CachedIniData Get(string iniFilePath) {
            lock (cache) {
                if (IsInWatchedFolder(iniFilePath)) {
                    if (!cache.ContainsKey(iniFilePath)) {
                        ReloadFile(iniFilePath);
                    }
                } else {
                    AtomIniUtils.ExtVLog("File ini {iniFilePath} is not in watched folder " + AtomIniSettings.atomIniDefaultFolder + ". Checking timestamp...", iniFilePath);
                    if (NeedsToReload(iniFilePath)) {
                        AtomIniUtils.ExtVLog("File ini {iniFilePath} needs to be reloaded.", iniFilePath);
                        ReloadFile(iniFilePath);
                    }
                }
                return cache[iniFilePath];
            }
        }
    }

    public static class AtomIniData {
        
        private static CachedIniData GetCachedData(string iniFilePath) {
            CachedIniData cachedData = CacheManager.Get(iniFilePath);
            if (cachedData == null) {
                throw new ArgumentNullException("An error occurred while accessing the cached data for ini file " + iniFilePath);
            }
            return cachedData;
        }
        
        public static void WriteValue(string iniFilePath, string section, string valueName, string value) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            cachedData.data[section][valueName] = value;
            CacheManager.UpdateFile(iniFilePath);
        }

        public static bool DeleteSection(string iniFilePath, string section) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            if (cachedData.data.Sections.ContainsSection(section)) {
                AtomIniUtils.ExtDLog("AtomINI: ## Called DeleteSection with section {Section} in file {filePath}", section, iniFilePath);
                cachedData.data.Sections.RemoveSection(section);
                CacheManager.UpdateFile(iniFilePath);
                return true;
            } else {
                AtomIniUtils.ExtDLog("AtomINI: ## Called DeleteSection with section {Section} in file {filePath}, but section does not exist.", section, iniFilePath);
                return false;
            }
        }

        public static string Read(string iniFilePath, string section, string valueName, string defaultValue) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            return cachedData.data[section][valueName];
        }

        public static bool SectionKeyPairExist(string section, string key, string iniFilePath) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            return cachedData.data.Sections.ContainsSection(section) && cachedData.data[section].ContainsKey(key);
        }

        public static bool DeleteKeyPair(string section, string key, string iniFilePath) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            if (SectionKeyPairExist(section, key, iniFilePath)) {
                cachedData.data.Sections[section].RemoveKey(key);
                CacheManager.UpdateFile(iniFilePath);
                return true;
            }
            return false;
        }
        
        public static List<string> GetAllSections(string iniFilePath) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            List<string> sections = new List<string>();
            foreach (var section in cachedData.data.Sections) {
                sections.Add(section.SectionName);
            }
            return sections;
        }
        
        public static List<string> GetAllSectionKeys(string iniFilePath, string section) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            List<string> keys = new List<string>();
            if (cachedData.data.Sections.ContainsSection(section)) {
                foreach (var key in cachedData.data[section]) {
                    keys.Add(key.KeyName);
                }
            }
            return keys;
        }
        
        public static Dictionary<string, string> GetSectionKeyValuePairs(string iniFilePath, string section) {
            CachedIniData cachedData = GetCachedData(iniFilePath);
            return cachedData.data[section].ToDictionary(key => key.KeyName, key => key.Value);
        }

    }
    
    
}