using System;
using System.Collections.Generic;
using System.Threading;

namespace AtomINI {
    public class AtomIni {

        public static bool SetValue<T>(string iniFileName, string keyName, string valueName, T value, bool enableSmartSection = true) {
            
            // Creo oggetto per la sincronizzazione
            AtomIniSynch synch = new AtomIniSynch();
            
            try {
                
                AtomIniUtils.ExtDLog("## Called SetValue with key {valueName} and value {value} with section {keyName} in file {iniFileName}", value, valueName, keyName, iniFileName);
                
                // Controllo se è stato passato un file ini valido
                if (string.IsNullOrEmpty(iniFileName)) {
                    AtomIniUtils.ELog("SetValue: iniFileName is null or empty. This SetValue call will be ignored.");
                    return false;
                }
                
                // Controllo se il file esiste, altrimenti lo creo
                AtomIniUtils.CheckFileExistence(iniFileName);
                
                // Controllo encoding del file ini
                // TODO: Implementare controllo encoding
                
                // Inizio il blocco di sincronizzazione
                synch.Block(iniFileName);

            }catch (Exception e) {
                Console.WriteLine(e);
            }
            
            
            
            
            
            return true;
        }


        public static List<string> ReadSections(string iniFilePath) {
            return new List<string>();
        }

        public static List<string> ReadSectionKeys(string fileName, string sectionName) {
            return new List<string>();
        }
    }
}