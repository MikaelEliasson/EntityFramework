// <auto-generated />
namespace EntityFramework.Tool
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Strings
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("EntityFramework.Tool.Strings", typeof(Strings).GetTypeInfo().Assembly);

        /// <summary>
        /// 
        /// Usage: 
        /// 
        ///   ef.exe command [--key=value]
        /// 
        /// Commands: 
        /// 
        ///   config    Writes configuration
        ///   create    Scaffolds a new migration
        ///   list      Lists migrations
        ///   script    Generates SQL script
        ///   apply     Updates the database
        /// 
        /// Examples:
        /// 
        ///   ef.exe config 
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]
        ///     
        ///   ef.exe create 
        ///     --MigrationName=&lt;name&gt;
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]
        ///     
        ///   ef.exe list
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     
        ///   ef.exe script
        ///     [--TargetMigration=&lt;name&gt;]
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]
        ///     
        ///   ef.exe apply
        ///     [--TargetMigration=&lt;name&gt;]
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]    
        ///       
        ///     
        /// </summary>
        internal static string ToolUsage
        {
            get { return GetString("ToolUsage"); }
        }

        /// <summary>
        /// 
        /// Usage: 
        /// 
        ///   ef.exe command [--key=value]
        /// 
        /// Commands: 
        /// 
        ///   config    Writes configuration
        ///   create    Scaffolds a new migration
        ///   list      Lists migrations
        ///   script    Generates SQL script
        ///   apply     Updates the database
        /// 
        /// Examples:
        /// 
        ///   ef.exe config 
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]
        ///     
        ///   ef.exe create 
        ///     --MigrationName=&lt;name&gt;
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]
        ///     
        ///   ef.exe list
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     
        ///   ef.exe script
        ///     [--TargetMigration=&lt;name&gt;]
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]
        ///     
        ///   ef.exe apply
        ///     [--TargetMigration=&lt;name&gt;]
        ///     [--ContextAssembly=&lt;file&gt;] [--ContextType=&lt;type&gt;]
        ///     [--MigrationAssembly=&lt;file&gt;] [--MigrationNamespace=&lt;namespace&gt;] [--MigrationDirectory=&lt;directory&gt;] 
        ///     [--References=&lt;file[;... n]&gt;]    
        ///       
        ///     
        /// </summary>
        internal static string FormatToolUsage()
        {
            return GetString("ToolUsage");
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);
    
            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
