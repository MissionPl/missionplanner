﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MissionPlanner.Log {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class LogStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LogStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MissionPlanner.Log.LogStrings", typeof(LogStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure?.
        /// </summary>
        public static string Confirmation {
            get {
                return ResourceManager.GetString("Confirmation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating KML for {0}.
        /// </summary>
        public static string CreatingKmlPrompt {
            get {
                return ResourceManager.GetString("CreatingKmlPrompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Done.
        /// </summary>
        public static string Done {
            get {
                return ResourceManager.GetString("Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to !!Allow 30-90 seconds for erase.
        /// </summary>
        public static string EraseComplete {
            get {
                return ResourceManager.GetString("EraseComplete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error processing file. Make sure the file is not in use.
        /// </summary>
        public static string ErrorProcessingLogfile {
            get {
                return ResourceManager.GetString("ErrorProcessingLogfile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fetching log file {0} ....
        /// </summary>
        public static string FetchingLog {
            get {
                return ResourceManager.GetString("FetchingLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Getting list of log files....
        /// </summary>
        public static string FetchingLogfileList {
            get {
                return ResourceManager.GetString("FetchingLogfileList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No logs to download.
        /// </summary>
        public static string NoLogsFound {
            get {
                return ResourceManager.GetString("NoLogsFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please connect your drone.
        /// </summary>
        public static string NotConnected {
            get {
                return ResourceManager.GetString("NotConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please select some logs by checking the check boxes in the list.
        /// </summary>
        public static string NothingSelected {
            get {
                return ResourceManager.GetString("NothingSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processing {0}.
        /// </summary>
        public static string ProcessingLog {
            get {
                return ResourceManager.GetString("ProcessingLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found {0} log files, note: item sizes are just an estimate..
        /// </summary>
        public static string SomeLogsFound {
            get {
                return ResourceManager.GetString("SomeLogsFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error:.
        /// </summary>
        public static string UnhandledException {
            get {
                return ResourceManager.GetString("UnhandledException", resourceCulture);
            }
        }
    }
}
