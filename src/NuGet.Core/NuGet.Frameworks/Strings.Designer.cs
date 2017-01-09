﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuGet.Frameworks {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Strings() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuGet.Frameworks.Strings", typeof(Strings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
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
        ///    Looks up a localized string similar to Frameworks must have the same identifier, profile, and platform..
        /// </summary>
        public static string FrameworkMismatch {
            get {
                return ResourceManager.GetString("FrameworkMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Invalid framework identifier &apos;{0}&apos;..
        /// </summary>
        public static string InvalidFrameworkIdentifier {
            get {
                return ResourceManager.GetString("InvalidFrameworkIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Invalid framework version &apos;{0}&apos;..
        /// </summary>
        public static string InvalidFrameworkVersion {
            get {
                return ResourceManager.GetString("InvalidFrameworkVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Invalid portable frameworks &apos;{0}&apos;. A hyphen may not be in any of the portable framework names..
        /// </summary>
        public static string InvalidPortableFrameworksDueToHyphen {
            get {
                return ResourceManager.GetString("InvalidPortableFrameworksDueToHyphen", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Invalid portable frameworks for &apos;{0}&apos;. A portable framework must have at least one framework in the profile..
        /// </summary>
        public static string MissingPortableFrameworks {
            get {
                return ResourceManager.GetString("MissingPortableFrameworks", resourceCulture);
            }
        }
    }
}
