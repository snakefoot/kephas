﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kephas.Model.Resources {
    using System;
    
    
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Model.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Cannot provide an element information for the runtime element &apos;{0}&apos;..
        /// </summary>
        internal static string CannotProvideElementInfoForRuntimeElement {
            get {
                return ResourceManager.GetString("CannotProvideElementInfoForRuntimeElement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple base classifiers found for {0}: {1}. To disambiguate, please mark all, except at most one, as mix-ins..
        /// </summary>
        internal static string ClassifierBase_AmbiguousBase_Exception {
            get {
                return ResourceManager.GetString("ClassifierBase_AmbiguousBase_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot instantiate the implementation type &apos;{0}&apos; because it is not resolved to an {1} instance. The provided type info was &apos;{2}&apos;..
        /// </summary>
        internal static string ClassifierBase_CannotInstantiateAbstractTypeInfo_Exception {
            get {
                return ResourceManager.GetString("ClassifierBase_CannotInstantiateAbstractTypeInfo_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple members with the name {0} found in the bases of {1}: {2}. Possible resolutions: either add a new one at that classifier level to inherit from all the base members, or remove one or more from the bases..
        /// </summary>
        internal static string ClassifierBase_ConflictingMembersInBases_Exception {
            get {
                return ResourceManager.GetString("ClassifierBase_ConflictingMembersInBases_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A generic classifier with more than one part is not supported. Check the {0} classifier with parts: {1}..
        /// </summary>
        internal static string ClassifierBase_MultipleGenericPartsNotSupported_Exception {
            get {
                return ResourceManager.GetString("ClassifierBase_MultipleGenericPartsNotSupported_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The element &apos;{0}&apos; must be a direct member of &apos;{1}&apos;, not inherited, to be used in its configurator..
        /// </summary>
        internal static string ClassifierConfiguratorBase_WithProperty_ForeignProperty_Exception {
            get {
                return ResourceManager.GetString("ClassifierConfiguratorBase_WithProperty_ForeignProperty_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The element &apos;{0}&apos; is not a member of &apos;{1}&apos;..
        /// </summary>
        internal static string ElementNotFoundInMembers {
            get {
                return ResourceManager.GetString("ElementNotFoundInMembers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The model space is not set in the construction context..
        /// </summary>
        internal static string NamedElementBase_MissingModelSpaceInConstructionContext_Exception {
            get {
                return ResourceManager.GetString("NamedElementBase_MissingModelSpaceInConstructionContext_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a constructible model element instead of {0}.
        ///.
        /// </summary>
        internal static string NonConstructibleElementException_Message {
            get {
                return ResourceManager.GetString("NonConstructibleElementException_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some indicated granted permissions for {0} are not found: {1}..
        /// </summary>
        internal static string PermissionType_MissingGrantedPermissions_Exception {
            get {
                return ResourceManager.GetString("PermissionType_MissingGrantedPermissions_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property &apos;{0}&apos; in &apos;{1}&apos; does not have any parts to be able to compute the property type..
        /// </summary>
        internal static string Property_MissingPartsToComputePropertyType_Exception {
            get {
                return ResourceManager.GetString("Property_MissingPartsToComputePropertyType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create a model element out of the provided attribute {0}..
        /// </summary>
        internal static string RuntimeModelElementConfiguratorBase_AddAttribute_CannotCreateModelElement_Exception {
            get {
                return ResourceManager.GetString("RuntimeModelElementConfiguratorBase_AddAttribute_CannotCreateModelElement_Excepti" +
                        "on", resourceCulture);
            }
        }
    }
}
