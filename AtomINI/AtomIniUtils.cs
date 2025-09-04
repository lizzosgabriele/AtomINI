using System;
using System.IO;
using System.Text;
using Serilog;

namespace AtomINI {
    
    public class AtomIniUtils {
        
        
        public static void ILog(string message, params object[] args) {
            Log.Information("AtomINI: " + message, args);
            if(AtomIniSettings.enableTestLogsOnDesktop) { /*AtomTestLog.LogToDesktop(message);*/}
        }
        
        /**
         * Metodo per il logging di messaggi di Debug estesi.
         * Vengono loggati solo se il flag enableExtLogging è attivo.
         * Se il flag enableTestLogsOnDesktop è attivo, i messaggi vengono loggati anche su un file di testo sul desktop.
         */
        public static void ExtDLog(string message, params object[] args) {
            if (AtomIniSettings.enableExtLogging) Log.Debug("AtomINI: " + message, args);
            if(AtomIniSettings.enableTestLogsOnDesktop) { /*AtomTestLog.LogToDesktop(message);*/}
        }
        
        /**
         * Metodo per il logging di messaggi Verbose estesi.
         * Vengono loggati solo se il flag enableExtLogging è attivo.
         * Se il flag enableTestLogsOnDesktop è attivo, i messaggi vengono loggati anche su un file di testo sul desktop.
         */
        public static void ExtVLog(string message, params object[] args) {
            if(AtomIniSettings.enableExtLogging) Log.Verbose("AtomINI: " + message, args);
            if(AtomIniSettings.enableTestLogsOnDesktop) {/*AtomTestLog.LogToDesktop(message);*/}
        }

        /**
         * Metodo per il logging di messaggi di errore.
         * Vengono sempre loggati.
         * Se il flag enableTestLogsOnDesktop è attivo, i messaggi vengono loggati anche su un file di testo sul desktop.
         */
        public static void ELog(string message, params object[] args) {
            Log.Error("AtomINI: " + message, args);
            if(AtomIniSettings.enableTestLogsOnDesktop) {/*AtomTestLog.LogToDesktop(message);*/}
        }
        
        /**
         * Restituisce encoding attivo in base al flag enableUnicode della libreria.
         */
        public static Encoding getActiveEncoding() { return AtomIniSettings.enableUnicode ? Encoding.UTF8 : Encoding.ASCII; }
        
        /**
         * Controlla l'esistenza del file ini specificato dal path.
         * Se il file non esiste, lo crea vuoto.
         */
        public static void CheckFileExistence(string path) {
            if (File.Exists(path)) return;
            try {
                using (FileStream fs = File.Create(path))
                using (StreamWriter writer = new StreamWriter(fs, getActiveEncoding())) {
                    writer.WriteLine("\n");
                }
                ExtVLog("Created empty ini file at {path}", path);
            } catch (Exception ex) {
                ELog("An error occurred while creating empty ini file: " + ex.Message);
            }
        }
        
    }
}