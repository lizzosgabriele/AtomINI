using System;
using System.Collections.Generic;
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

        public static void UpdateFile(string iniFilePath) {
            AtomIniUtils.ExtVLog("Writing data to ini file {iniFileName}", iniFilePath);
            parser.WriteFile(iniFilePath, cache[iniFilePath].data, AtomIniUtils.getActiveEncoding());
            AtomIniUtils.ExtVLog("Data written to ini file {iniFileName} successfully", iniFilePath);
        }

        public static void ReloadFile(string iniFilePath) {
            AtomIniUtils.ExtVLog("Reloading from disk file ", iniFilePath);
            IniData data_read = parser.ReadFile(iniFilePath);
            string lastEditTimestamp = System.IO.File.GetLastWriteTimeUtc(iniFilePath).ToString("o");
            CachedIniData cachedData = new CachedIniData(data_read, lastEditTimestamp);
            cache[iniFilePath] = cachedData;
        }
        
        public static bool NeedsToReload(string iniFilePath) {
            if (!cache.ContainsKey(iniFilePath)) return true;
            string currentTimestamp = System.IO.File.GetLastWriteTimeUtc(iniFilePath).ToString("o");
            return cache[iniFilePath].lastEditTimestamp != currentTimestamp;
        }

        public static CachedIniData Get(string iniFilePath) {
            if (NeedsToReload(iniFilePath)) {
                AtomIniUtils.ExtVLog($"Ini file {iniFilePath} needs to be reloaded from disk.", iniFilePath);
                ReloadFile(iniFilePath);
            } 
            return cache.ContainsKey(iniFilePath) ? cache[iniFilePath] : null;
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

    }
    
    
}