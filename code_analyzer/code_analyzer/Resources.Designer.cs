﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace code_analyzer {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("code_analyzer.Resources", typeof(Resources).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to AggregateException - Exception being wrapped in top-level exception..
        /// </summary>
        internal static string AggregateExceptionTitle {
            get {
                return ResourceManager.GetString("AggregateExceptionTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Blank Block Code.
        /// </summary>
        internal static string BlankBlockCodeTitle {
            get {
                return ResourceManager.GetString("BlankBlockCodeTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Contextual Keyword as a Variable or Member..
        /// </summary>
        internal static string ContextualKeywordsTitle {
            get {
                return ResourceManager.GetString("ContextualKeywordsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Field as Public or Protected.
        /// </summary>
        internal static string EncapsulateFieldTitle {
            get {
                return ResourceManager.GetString("EncapsulateFieldTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enum Without Default Value.
        /// </summary>
        internal static string EnumDefaultValueTitle {
            get {
                return ResourceManager.GetString("EnumDefaultValueTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Exception Without Context.
        /// </summary>
        internal static string ExceptionWithoutContextTitle {
            get {
                return ResourceManager.GetString("ExceptionWithoutContextTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inappropriate Usage of Property.
        /// </summary>
        internal static string InappropriateUsageOfPropertyTitle {
            get {
                return ResourceManager.GetString("InappropriateUsageOfPropertyTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Member Publicly Exposes a Concrete Collection Type.
        /// </summary>
        internal static string LiskovSubsitutionPrincipalTitle {
            get {
                return ResourceManager.GetString("LiskovSubsitutionPrincipalTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}..
        /// </summary>
        internal static string MessageFormat {
            get {
                return ResourceManager.GetString("MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method With bool as Parameter.
        /// </summary>
        internal static string MethodWithBoolAsParameterTitle {
            get {
                return ResourceManager.GetString("MethodWithBoolAsParameterTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method With Too Many Parameters.
        /// </summary>
        internal static string MethodWithMoreThanSevenParametersTitle {
            get {
                return ResourceManager.GetString("MethodWithMoreThanSevenParametersTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Avoid protected / public constants for values that might change.
        /// </summary>
        internal static string NonPrivateConstantsTitle {
            get {
                return ResourceManager.GetString("NonPrivateConstantsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Prefer Class Over Struct.
        /// </summary>
        internal static string PreferClassOverStructTitle {
            get {
                return ResourceManager.GetString("PreferClassOverStructTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Avoid Assert Single Item With UnitOfWork.
        /// </summary>
        internal static string ShouldlySingleAssertInUowTitle {
            get {
                return ResourceManager.GetString("ShouldlySingleAssertInUowTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing Default Case.
        /// </summary>
        internal static string SwitchWihoutDefaultCaseTitle {
            get {
                return ResourceManager.GetString("SwitchWihoutDefaultCaseTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Simplify Test Code by removing unnecessary arguments.
        /// </summary>
        internal static string TestCaseArgumentsTitle {
            get {
                return ResourceManager.GetString("TestCaseArgumentsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ToArray/ToList Inside Foreach Loop..
        /// </summary>
        internal static string ToArrayToListInsideForeachDeclarationTitle {
            get {
                return ResourceManager.GetString("ToArrayToListInsideForeachDeclarationTitle", resourceCulture);
            }
        }
    }
}
