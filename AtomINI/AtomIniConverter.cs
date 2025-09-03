using System;
using System.Globalization;

namespace AtomINI {
    public class AtomIniConverter {
        
        /**
         * Metodo generico per convertire un valore di tipo T in stringa.
         * Supporta i tipi: int, double, float, long, bool, string.
         * Per i bool, restituisce "1" per true e "0" per false.
         * Se il tipo non è supportato, viene lanciata un'eccezione NotSupportedException.
         */
        public static string ConvertToString<T>(T value) {
            if (value == null) { return ""; }

            Type type = typeof(T);

            if (type == typeof(int) || type == typeof(Int32)) {
                return int.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString();
            }

            if (type == typeof(double)) {
                return double.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            }

            if (type == typeof(float)) {
                return float.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            }

            if (type == typeof(long)) {
                return long.Parse(value.ToString(), CultureInfo.InvariantCulture).ToString();
            }

            if (type == typeof(bool)) { return bool.Parse(value.ToString()) ? "1" : "0"; }

            if (type == typeof(string)) { return value.ToString(); }

            throw new NotSupportedException($"Tipo {typeof(T)} non supportato");
        }
        
        /**
         * Metodo generico per convertire una stringa in un valore di tipo T.
         * Supporta i tipi: int, double, float, long, bool, string.
         * Per i bool, accetta "true", "1" come true e "false", "0" come false (case insensitive).
         * Se la stringa è null o vuota, viene restituito il valore di default passato come parametro.
         * Se il tipo non è supportato, viene lanciata un'eccezione NotSupportedException.
         */
        public static T ConvertFromString<T>(string value, T defValue)  {
            if (string.IsNullOrEmpty(value)) {
                return defValue;
            }

            Type type = typeof(T);

            try {
                if (type == typeof(int) || type == typeof(Int32)) {
                    if(value.Equals("false", StringComparison.OrdinalIgnoreCase)) return (T)(object)0;
                    if(value.Equals("true", StringComparison.OrdinalIgnoreCase)) return (T)(object)1;
                    return (T)(object)int.Parse(value, CultureInfo.InvariantCulture);
                }

                if (type == typeof(double)) {
                    return (T)(object)double.Parse(value, CultureInfo.InvariantCulture);
                }

                if (type == typeof(float)) {
                    return (T)(object)float.Parse(value, CultureInfo.InvariantCulture);
                }

                if (type == typeof(long)){
                    return (T)(object)long.Parse(value, CultureInfo.InvariantCulture);
                }

                if (type == typeof(bool)) {
                    return (T)(object)(value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("1"));
                }

                if (type == typeof(string)) {
                    if(value.Equals("false", StringComparison.OrdinalIgnoreCase) && defValue.Equals("0")) return (T)(object)"0";
                    if(value.Equals("true", StringComparison.OrdinalIgnoreCase) && defValue.Equals("1")) return (T)(object)"1";
                    if(value.Equals("true", StringComparison.OrdinalIgnoreCase) && defValue.Equals("0")) return (T)(object)"1";
                    if(value.Equals("false", StringComparison.OrdinalIgnoreCase) && defValue.Equals("1")) return (T)(object)"0";
                    return (T)(object)value;
                }
            } catch (Exception e) {
                AtomIniUtils.ELog($"Errore nella conversione del valore: {e.Message}");
            }

            throw new NotSupportedException($"Tipo {typeof(T)} non supportato");
        }
        
    }
}