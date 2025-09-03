using System;
using System.Threading;
using Serilog;

namespace AtomINI {
    
    public class AtomIniSynch {
        private readonly AtomIniMutex atomutex = new AtomIniMutex();

        public void Block(string iniFileName) {
            atomutex.CheckForWaitMutex(iniFileName);
        }
        
        public void Release(string iniFileName) {
            atomutex.CheckForReleaseMutex(iniFileName);
        }
    }

    public class AtomIniMutex {
        
        Mutex mutex = null;
        bool isMutexAcquired = false;

        public bool CheckForWaitMutex(string iniFileName) {
            try {
                if (!AtomIniSettings.useMutex) {
                    AtomIniUtils.ExtVLog("Mutex setting is disabled. Ignoring mutex...");
                    return false;
                }
                mutex = new Mutex(false, AtomIniSettings.MUTEX_NAME);
                AtomIniUtils.ExtVLog("Waiting for mutex availability for file {iniFileName}", iniFileName);
                mutex.WaitOne();
                isMutexAcquired = true;
                AtomIniUtils.ExtVLog("Acquired mutex for file {iniFileName}", iniFileName);
                return true;
            } catch (Exception e) {
                var exceptionType = e.GetType().Name;
                AtomIniUtils.ELog("An error of type {exceptionType} occurred while trying to acquire the mutex for file {iniFileName}. Exception: {e}.",
                    exceptionType, iniFileName, e.Message);
                //AtomWindowsEvent.WriteErrorLog("AtomINI","An error of type " + exceptionType + " occurred while trying to acquire the mutex for file " + iniFileName + ". Exception: " + e.Message);
                return false;
            }
        }
 
        public void CheckForReleaseMutex(string iniFileName) {
            try {
                if (!AtomIniSettings.useMutex) return;
                if (mutex != null && isMutexAcquired) {
                    AtomIniUtils.ExtVLog("Releasing mutex for file {iniFileName}", iniFileName);
                    mutex.ReleaseMutex();
                    AtomIniUtils.ExtVLog("Mutex released!", iniFileName);
                } else {
                    Log.Error("Mutex for file {iniFileName} is null, cannot release it!", iniFileName);
                }
            } catch (Exception e) {
                var exceptionType = e.GetType().Name;
                AtomIniUtils.ELog("An error of type {exceptionType} occurred while trying to release the mutex for file {iniFileName}. Exception: {e}.",
                    exceptionType, iniFileName, e.Message);
                //AtomWindowsEvent.WriteErrorLog("AtomINI","An error of type " + exceptionType + " occurred while trying to release the mutex for file " + iniFileName + ". Exception: " + e.Message);
            }
            
        }

    }

}