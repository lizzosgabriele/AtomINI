using System.Text;
using System.Text.RegularExpressions;
using IniParser.Model.Configuration;
using IniParser.Parser;

namespace AtomINI {
    
    public static class AtomIniSettings {
        
        /**
         * Questa regex viene utilizzata per identificare i commenti all'interno di un file ini.
         * Ci sono due casi:
         * - Commento all'inizio della riga.
         * - Commento dopo la corrispondenza chiave/valore.
         *
         * Nel primo caso, sono accettati tutti i caratteri che non sono alfanumerici.
         * Nel secondo caso, sono accettati i caratteri # e ; come commento, oppure due barre (//)
         */
        const string regex = @"^(?!\s*\[.*\])[^a-zA-Z0-9\n\s]+(.*)|(?!\s*\[.*\])(?:#+|\s+;|\/{2,})(.*)";
        
        /**
         * Config da utilizzare per i parser della libreria parser-ini
         */
        public static readonly IniDataParser config_parser = new IniDataParser(new IniParserConfiguration { AssigmentSpacer = "", CommentRegex = new Regex(regex), CaseInsensitive = true, SkipInvalidLines = true });

        /**
         * Flag per abilitare i log estesi (Debug/Verbose) all'interno della libreria.
         */
        public static bool enableExtLogging = false;
        
        /**
         * Flag per abilitare la scrittura dei log di questa libreria ANCHE il un file temporaneo sul desktop.
         * Da utilizzare quando l'eseguibile che usa questa libreria:
         * - non usa Serilog
         * - Non ha logging attivo
         */
        public static bool enableTestLogsOnDesktop = false;
        
        /**
         * Nome mutex globale di sistema per i config atom.
         */
        public static readonly string MUTEX_NAME = "Global\\AtomIniMutex";
        
        /**
         * Flag per abilitare l'uso del mutex globale di sistema per la lettura/scrittura dei file ini.
         * Usato per evitare problemi di concorrenza tra più processi.
         */
        public static bool useMutex = true;
        
        /**
         * Flag to ALWAYS disable the smart generation of section name. If set to TRUE, the section name used inside GetValue/SetValue
         * will always be the parameter passed!
         */
        public static bool alwaysDisableSmartSection = false;
        
        /**
         * Flag per abilitare il formato UTF8 (with BOM) nella scrittura di un file ini.
         * Con questo flag abilitato, tutti gli ini al momento della lettura saranno convertiti in UTF8, e la libreria scriverà
         * i nuovi file con UTF8.
         *
         * NOTA: Con questo flag spento, la libreria sarà in grado di leggere e scrivere file ini in ASCII e UTF8, la
         * differenza sta solo nel fatto che con questo flag spento, i nuovi file ini saranno scritti in ASCII.
         */
        public static bool enableUnicode = false;
        
        /**
         * Elenco degli ID sezione standard di ATOM. Usate per il riconoscimento automatico del file ini e per il calcolo
         * del nome completo della sezione.
         */
        public static string[] atomSectionIds = {
            "InteractiveLay",
            "Acquisition",
            "ZebraPrinter",
            "Wtc",
            "Layout Editor",
            "ANSGeneral",
            "General",
            "MeasureUnits\\Used"
        };

        /**
         * Elenco dei file ini per i quali NON deve essere usata la generazione intelligente del nome della sezione.
         */
        public static string[] smartSectionFileBlackList = {
            "C:\\ProgramData\\ATOM\\DxfConverter\\dxf2die.ini"
        };
        
        /**
         * Nome dell'applicazione eseguibile che sta usando la libreria.
         * Usato per i log.
         */
        public static string iniExeAppName = "Unknown";

    }
    
}