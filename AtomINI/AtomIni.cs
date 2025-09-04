using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

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

                // Genero il nome della sezione dinamicamente
                // TODO: Aggiungere gestione sezione duplicata
                AtomIniUtils.ExtVLog("Generating section name for key {keyName} with value {valueName}", keyName, valueName);
                string sectionName = AtomIniSections.GenSectionName(iniFileName, keyName, enableSmartSection);
                AtomIniUtils.ExtVLog("Generated section name: {sectionName}", sectionName);

                // Converto il valore in stringa
                string stringValue = AtomIniConverter.ConvertToString(value);

                // Inizio il blocco di sincronizzazione
                synch.Block(iniFileName);

                // Scrivo il valore e prendo il risultato
                AtomIniData.WriteValue(iniFileName, sectionName, valueName, stringValue);
                
                AtomIniUtils.ExtVLog("SetValue Succeded!");
                
                return true;
            } catch (Exception e) {
                AtomIniUtils.ELog(e.Message);
                return false;
            } finally {
                // Rilascio il blocco di sincronizzazione
                synch.Release(iniFileName);
            }
        }


        public static T GetValue<T>(string iniFileName, string keyName, string valueName, T defaultValue = default(T), bool enableSmartSection = true) {
            
            // Creo oggetto per la sincronizzazione
            AtomIniSynch synch = new AtomIniSynch();

            try {
                
                AtomIniUtils.ExtDLog("## Called GetValue with key {valueName} and section {keyName} in file {iniFileName}", valueName, keyName, iniFileName);

                // Controllo se è stato passato un file ini valido
                if (string.IsNullOrEmpty(iniFileName)) {
                    AtomIniUtils.ELog("GetValue: iniFileName is null or empty. This GetValue call will be ignored.");
                    return defaultValue;
                }
                
                // Controllo se il file esiste, altrimenti lo creo
                AtomIniUtils.CheckFileExistence(iniFileName);

                // Controllo encoding del file ini
                // TODO: Implementare controllo encoding
                
                // Genero il nome della sezione dinamicamente
                // TODO: Aggiungere gestione sezione duplicata
                AtomIniUtils.ExtVLog("Generating section name for key {keyName} with value {valueName}", keyName, valueName);
                string sectionName = AtomIniSections.GenSectionName(iniFileName, keyName, enableSmartSection);
                AtomIniUtils.ExtVLog("Generated section name: {sectionName}", sectionName);
                
                // Inizio il blocco di sincronizzazione
                synch.Block(iniFileName);
                
                // Converto il valore in stringa
                string stringDefaultValue = AtomIniConverter.ConvertToString(defaultValue);
                
                // Controllo se la sezione/chiave esiste, altrimenti la scrivo con il valore di default
                AtomIniUtils.ExtVLog("Checking if section {sectionName} and key {valueName} exist in file {iniFileName}", sectionName, valueName, iniFileName);
                if (!AtomIniData.SectionKeyPairExist(sectionName, valueName, iniFileName)) {
                    AtomIniUtils.ExtDLog("AtomINI: Key {valueName} not found in section {sectionName}. Setting default value.", valueName, sectionName);
                    SetValue(iniFileName, sectionName, valueName, defaultValue);
                    return defaultValue;
                }
                
                // Leggo il valore e prendo il risultato
                string returnValue = AtomIniData.Read(iniFileName, sectionName, valueName, stringDefaultValue);
                
                AtomIniUtils.ExtVLog("GetValue result: {returnValue}", returnValue);
                
                // Converto il valore letto in T
                return AtomIniConverter.ConvertFromString(returnValue, defaultValue);
                
            } catch (Exception e) {
                AtomIniUtils.ELog(e.Message);
                return defaultValue;
            } finally {
                // Rilascio il blocco di sincronizzazione
                synch.Release(iniFileName);
            }

        }

        public static bool DeleteSection(string filePath, string Section) {
            if (!File.Exists(filePath) || string.IsNullOrWhiteSpace(Section)) { return false; }
            AtomIniSynch synch = new AtomIniSynch();
            try {
                synch.Block(filePath);
                return AtomIniData.DeleteSection(filePath, Section);
            }catch (Exception e) {
                AtomIniUtils.ELog(e.Message);
                return false;
            }finally {
                synch.Release(filePath);
            }
        }

        public static List<string> ReadSections(string iniFilePath) {
            return new List<string>();
        }

        public static List<string> ReadSectionKeys(string fileName, string sectionName) {
            return new List<string>();
        }
    }
}