using System;
using System.Collections.Generic;
using System.Linq;

namespace AtomINI {
    
    public class AtomIniSections {
        
        private const string ROOT_KEY = "Software\\String\\Nesting System";
        
        
        public static string GenSectionName(string iniFileName, string section, bool smartSectionEnabled) {
            
            if (!smartSectionEnabled || AtomIniSettings.alwaysDisableSmartSection) {
                return section;
            }

            List<string> allSectionInFile = AtomIni.ReadSections(iniFileName);
            
            bool isAtomSectionId = AtomIniSettings.atomSectionIds.Contains(section);
            string normalizedSection = NormalizeKeyName(section);
            bool hasNormalizedSection = allSectionInFile.Contains(normalizedSection);
            bool hasParameterSection = allSectionInFile.Contains(section);
            bool equalParameter = normalizedSection == section;
            
            if (equalParameter) // if parameter is already normalized
            {
                string sectionNotNormalized = normalizedSection.Split('\\').Last();
                hasParameterSection = allSectionInFile.Contains(sectionNotNormalized);
                isAtomSectionId = AtomIniSettings.atomSectionIds.Contains(sectionNotNormalized);

                if (isAtomSectionId) return normalizedSection;

                if (hasParameterSection && hasNormalizedSection) {
                    AtomIniUtils.ELog("INI-ILFASHION: Found a duplicate section inside {iniFileName}.", iniFileName);
                    int normalizedCountKeys = AtomIni.ReadSectionKeys(iniFileName, normalizedSection).Count;
                    int notNormalizedCountKeys = AtomIni.ReadSectionKeys(iniFileName, sectionNotNormalized).Count;
                    bool normalizedHasMoreKeys = normalizedCountKeys > notNormalizedCountKeys;
                    bool notNormalizedHasMoreKeys = notNormalizedCountKeys > normalizedCountKeys;
                    bool equalKeys = normalizedCountKeys == notNormalizedCountKeys;
                    if (equalKeys) return normalizedSection;
                    if(normalizedHasMoreKeys) return normalizedSection;
                    if(notNormalizedHasMoreKeys) return sectionNotNormalized;
                }
                
                return normalizedSection;

            }
            else // if parameter is not normalized
            {
                if (isAtomSectionId) // if parameter is an atom section ID
                {
                    if (hasNormalizedSection && hasParameterSection) // check for duplicates
                    {
                        AtomIniUtils.ELog("Found a duplicate section inside {iniFileName}.", iniFileName);
                        return normalizedSection;
                    }
                    if (hasNormalizedSection) return normalizedSection;
                    if (hasParameterSection) return section;
                    return normalizedSection;
                }
                else // if parameter is not an atom section ID
                {
                    if (hasNormalizedSection && hasParameterSection) // check for duplicates
                    {
                        AtomIniUtils.ELog("Found a duplicate section inside {iniFileName}.", iniFileName);
                        return section;
                    }
                    if (hasNormalizedSection && !hasParameterSection)
                    {
                        return normalizedSection;
                    }
                    if (!hasNormalizedSection && hasParameterSection)
                    {
                        return section;
                    }
                    return section;
                }
            }
        }
        
        /**
         * Metodo per normalizzare il 3nome della sezione.
         * Se il nome della sezione è vuoto o nullo, restituisce la root key.
         *
         * In altre parole converte ad esempio. "MeasureUnits\Used" in "Software\String\Nesting System\MeasureUnits\Used"
         */
        private static string NormalizeKeyName(string keyName) {
            if (string.IsNullOrEmpty(keyName)) {
                return ROOT_KEY;
            }else if (keyName.StartsWith(ROOT_KEY, StringComparison.OrdinalIgnoreCase)) {
                return keyName;
            }else {
                return $@"{ROOT_KEY}\{keyName}";
            }
        }
        
    }
    
}