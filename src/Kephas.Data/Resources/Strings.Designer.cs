﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kephas.Data.Resources {
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
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Kephas.Data.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Not enough elements after {0} in the destination array..
        /// </summary>
        public static string CollectionAdapter_CopyTo_NotEnoughElements_Exception {
            get {
                return ResourceManager.GetString("CollectionAdapter_CopyTo_NotEnoughElements_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command {0} is not supported..
        /// </summary>
        public static string DataCommandFactory_CreateCommand_NotSupported_Exception {
            get {
                return ResourceManager.GetString("DataCommandFactory_CreateCommand_NotSupported_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambiguous match for command &apos;{0}&apos; of &apos;{1}&apos;: at least &apos;{2}&apos; and &apos;{3}&apos; found. To disambiguate, please provide an override priority between them..
        /// </summary>
        public static string DataCommandFactory_GetCommandFactory_AmbiguousMatch_Exception {
            get {
                return ResourceManager.GetString("DataCommandFactory_GetCommandFactory_AmbiguousMatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Operation completed successfully..
        /// </summary>
        public static string DataCommandResult_Successful_Message {
            get {
                return ResourceManager.GetString("DataCommandResult_Successful_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple entries were found with the provided criteria {0}..
        /// </summary>
        public static string DataContext_FindAsync_AmbiguousMatch_Exception {
            get {
                return ResourceManager.GetString("DataContext_FindAsync_AmbiguousMatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided ID is empty..
        /// </summary>
        public static string DataContext_FindAsync_IdEmpty_Exception {
            get {
                return ResourceManager.GetString("DataContext_FindAsync_IdEmpty_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No entry was found with the provided criteria {0}..
        /// </summary>
        public static string DataContext_FindAsync_NotFound_Exception {
            get {
                return ResourceManager.GetString("DataContext_FindAsync_NotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided command execution context targets a different data context than the one supposed to execute the command..
        /// </summary>
        public static string DataContext_MismatchedDataContextInCommand_Exception {
            get {
                return ResourceManager.GetString("DataContext_MismatchedDataContextInCommand_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided context is not a &apos;{0}&apos;..
        /// </summary>
        public static string DataContextBase_BadInitializationContext_Exception {
            get {
                return ResourceManager.GetString("DataContextBase_BadInitializationContext_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The command type parameter &apos;{0}&apos; must inherit {1}..
        /// </summary>
        public static string DataContextBase_CreateCommand_BadCommandType_Exception {
            get {
                return ResourceManager.GetString("DataContextBase_CreateCommand_BadCommandType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided entity &apos;{0}&apos; is not attached to the data context..
        /// </summary>
        public static string DataContextBase_EntityNotAttached_Exception {
            get {
                return ResourceManager.GetString("DataContextBase_EntityNotAttached_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The key (&apos;{0}&apos;) and then entity info ID (&apos;{1}&apos;) do not match..
        /// </summary>
        public static string DataContextCache_KeyAndEntityEntryIdDoNotMatch_Exception {
            get {
                return ResourceManager.GetString("DataContextCache_KeyAndEntityEntryIdDoNotMatch_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected a {0} provider in query {1}..
        /// </summary>
        public static string DataContextQueryableSubstituteTypeConstantHandler_BadQueryProvider_Exception {
            get {
                return ResourceManager.GetString("DataContextQueryableSubstituteTypeConstantHandler_BadQueryProvider_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No data converters were found for source type &apos;{0}&apos; and target type &apos;{1}&apos;..
        /// </summary>
        public static string DataConverterNotFound_Exception {
            get {
                return ResourceManager.GetString("DataConverterNotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validation failed for {0}:
        ///{1}..
        /// </summary>
        public static string DataValidationException_Message {
            get {
                return ResourceManager.GetString("DataValidationException_Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Expected an object of type &apos;{0}&apos;, but received one of type &apos;{1}&apos;..
        /// </summary>
        public static string DataValidator_MismatchedEntityType_Exception {
            get {
                return ResourceManager.GetString("DataValidator_MismatchedEntityType_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ambiguous data context for data store &apos;{0}&apos;: at least &apos;{1}&apos; and &apos;{2}&apos; found. Please provide a proper [ProcessingPriority] attribute on the implementation DataContext classes to be able to disambiguate..
        /// </summary>
        public static string DefaultDataContextProvider_AmbiguousDataContext_Exception {
            get {
                return ResourceManager.GetString("DefaultDataContextProvider_AmbiguousDataContext_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No data context was found for data store &apos;{0}&apos; of target &apos;{1}&apos;..
        /// </summary>
        public static string DefaultDataContextProvider_DataContextNotFoundForDataStoreKind_Exception {
            get {
                return ResourceManager.GetString("DefaultDataContextProvider_DataContextNotFoundForDataStoreKind_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No data context with type &apos;{0}&apos; was found for data store &apos;{1}&apos;..
        /// </summary>
        public static string DefaultDataContextProvider_DataStoreDataContextTypeNotFound_Exception {
            get {
                return ResourceManager.GetString("DefaultDataContextProvider_DataStoreDataContextTypeNotFound_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided source object is null and a type information cannot be identified for it. No conversion has been performed..
        /// </summary>
        public static string DefaultDataConversionService_NonTypedSourceIsNull_Exception {
            get {
                return ResourceManager.GetString("DefaultDataConversionService_NonTypedSourceIsNull_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided target object is null and a type information cannot be identified for it..
        /// </summary>
        public static string DefaultDataConversionService_NonTypedTargetIsNull_Exception {
            get {
                return ResourceManager.GetString("DefaultDataConversionService_NonTypedTargetIsNull_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The data context is already set in the entity entry structure, cannot set multiple times..
        /// </summary>
        public static string EntityEntry_DataContextAlreadySet_Exception {
            get {
                return ResourceManager.GetString("EntityEntry_DataContextAlreadySet_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; must be initialized before working with entities..
        /// </summary>
        public static string InMemoryDataContext_NotInitialized_Exception {
            get {
                return ResourceManager.GetString("InMemoryDataContext_NotInitialized_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The maximum number of iterations ({0}) was exceeded when saving with activated behaviors. Could not update the changed entities. See the affected entities for more information..
        /// </summary>
        public static string PersistChangesCommand_MaximumNumberOfIterationsExceeded_Exception {
            get {
                return ResourceManager.GetString("PersistChangesCommand_MaximumNumberOfIterationsExceeded_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no modified root entities to save..
        /// </summary>
        public static string PersistChangesCommand_NoRootEntitiesToSave_Exception {
            get {
                return ResourceManager.GetString("PersistChangesCommand_NoRootEntitiesToSave_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Persisted {0} changes..
        /// </summary>
        public static string PersistChangesCommand_ResultMessage {
            get {
                return ResourceManager.GetString("PersistChangesCommand_ResultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{0}&apos; of type &apos;{1}&apos; cannot be converted to a list of &apos;{2}&apos;..
        /// </summary>
        public static string QueryHelper_ToListAsync_CannotConvertToListException {
            get {
                return ResourceManager.GetString("QueryHelper_ToListAsync_CannotConvertToListException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot retrieve a data context for reference {0}..
        /// </summary>
        public static string RefBase_GetDataContext_NullDataContext_Exception {
            get {
                return ResourceManager.GetString("RefBase_GetDataContext_NullDataContext_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entity for reference {0} could not be retrieved (null reference)..
        /// </summary>
        public static string RefBase_GetEntity_Null_Exception {
            get {
                return ResourceManager.GetString("RefBase_GetEntity_Null_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entity for reference {0} has been disposed..
        /// </summary>
        public static string RefBase_GetEntityEntry_Disposed_Exception {
            get {
                return ResourceManager.GetString("RefBase_GetEntityEntry_Disposed_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entity entry for reference {0} could not be retrieved (null reference)..
        /// </summary>
        public static string RefBase_GetEntityEntry_Null_Exception {
            get {
                return ResourceManager.GetString("RefBase_GetEntityEntry_Null_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field &apos;{0}&apos; is required..
        /// </summary>
        public static string RequiredRefAttribute_ValidationError {
            get {
                return ResourceManager.GetString("RequiredRefAttribute_ValidationError", resourceCulture);
            }
        }
    }
}
