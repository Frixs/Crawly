﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InformationRetrievalManager.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("InformationRetrievalManager.Localization.Resource", typeof(Resource).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter timestamp in the following format: yyyy-MM-dd HH:mm..
        /// </summary>
        internal static string CrawlerUpdateParameterEntry_Description_Timestamp {
            get {
                return ResourceManager.GetString("CrawlerUpdateParameterEntry_Description_Timestamp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter text for checking existence in the document title..
        /// </summary>
        internal static string CrawlerUpdateParameterEntry_Description_Title {
            get {
                return ResourceManager.GetString("CrawlerUpdateParameterEntry_Description_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ask with logical operators AND, OR, and NOT with the ability to prioritize them with brackets. Searched documents must contain/match the logical expression..
        /// </summary>
        internal static string QueryEntry_Description_Boolean {
            get {
                return ResourceManager.GetString("QueryEntry_Description_Boolean", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ask anything. The TF–IDF value increases proportionally to the number of times a word appears in the document..
        /// </summary>
        internal static string QueryEntry_Description_TfIdf {
            get {
                return ResourceManager.GetString("QueryEntry_Description_TfIdf", resourceCulture);
            }
        }
    }
}
