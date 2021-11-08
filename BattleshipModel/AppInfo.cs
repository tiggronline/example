using System;
using System.IO;
using System.Reflection;

namespace Battleship.Model.Config
{

    /// <summary>
    /// Model UI application information (WebClient or API depending on which calls it).
    /// </summary>
    public static class AppInfo
    {

        static AppInfo()
        {
            // Reflection is 'heavy' so pick up the assembly information just once
            var asm = Assembly.GetEntryAssembly(); // This will pick up DMS.WebClient or DMS.API

            Name = asm.GetName().Name;
            Product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            Description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
            Version = asm.GetName().Version;
            CompanyName = asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;

            RootPath = Path.GetDirectoryName(asm.Location);
        }


        /// <summary>
        /// Gets the name from the assembly.
        /// </summary>
        public static string Name { get; }

        /// <summary>
        /// Gets the product name from the assembly.
        /// </summary>
        public static string Product { get; }

        /// <summary>
        /// Gets the product description from the assembly.
        /// </summary>
        public static string Description { get; }

        /// <summary>
        /// Gets the product name from the assembly.
        /// </summary>
        public static Version Version { get; }

        /// <summary>
        /// Gets the company name from the assembly.
        /// </summary>
        public static string CompanyName { get; }

        /// <summary>
        /// Gets the copyright from the assembly.
        /// </summary>
        public static string Copyright { get; }

        /// <summary>
        /// Gets the product authors from the assembly.
        /// </summary>
        public static string Authors { get; }


        /// <summary>
        /// Gets the application root path.
        /// </summary>
        /// <returns>String</returns>
        public static string RootPath { get; }

    }

}