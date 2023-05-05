// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using WupiEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Moryx.Runtime.Modules;
using Moryx.Asp.Extensions;

namespace Moryx.Asp.Extensions
{
    /// <summary>
    /// License exception page builder class
    /// </summary>
    public static  class MoryxLicenseExceptionPageBuilder
    {
        internal static readonly string LicensePageName = "index.html";
        internal static readonly string LicensePageRootFolder = "LicenseExceptionFolder";
        internal static bool MissingLicenseDetected { get; private set; }

        /// <summary>
        /// Listens to exception that happens in the app and collects License exception
        /// </summary>
        internal static void RegisterLicenseExceptionBuilder()
        {
            //catching all exception not handled in the app
            AppDomain.CurrentDomain.FirstChanceException += HandleMissingLicenseException;
        }


        /// <summary>
        /// (Extension method)
        /// Boot system and start all modules
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IModuleManager StartAllMoryxModules(this IServiceProvider serviceProvider)
        {
            //register the license exception handling
            RegisterLicenseExceptionBuilder();

            var moduleManager = serviceProvider.GetRequiredService<IModuleManager>();
            moduleManager.StartModules();

            //start moryx license page builder
            if (MissingLicenseDetected)
                StartLicensePageBuilder();

            return moduleManager;
        }

        /// <summary>
        /// Builds the license page to display to the user
        /// Note: This generate a raw html file.
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private static LicensePageContent BuildLicensePage(string pageTitle, string description)
        {
            var exceptions = LicenseExceptions.GroupBy(x => x.Source).Select(d => new
            {
                Title = d.FirstOrDefault().Source,
                d.FirstOrDefault().Message,
            }).ToList();

            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.AppendLine("<!DOCTYPE html>");
            contentBuilder.AppendLine("<html>");
            contentBuilder.AppendLine("<body>");
            contentBuilder.AppendLine(@"
             <style>
                #table {
                  font-family: Arial, Helvetica, sans-serif;
                  border-collapse: collapse;
                  width: 100%;
                 }

                #table td, #table th {
                  border: 1px solid #ddd;
                  padding: 8px;
                }

                #table tr:nth-child(even){background-color: #f2f2f2;}

                #table tr:hover {background-color: #ddd;}

                #table th {
                  padding-top: 12px;
                  padding-bottom: 12px;
                  text-align: left;
                  background-color: red;
                  color: white;
                }
            </style>");
            contentBuilder.AppendLine($"<h1> MORYX License </h1>");
            contentBuilder.AppendLine($"<h2>{pageTitle}</h2>");
            contentBuilder.AppendLine($"<p>{description}</p>");
            //content table

            contentBuilder.AppendLine("<table id=\"table\">");
            //header
            contentBuilder.AppendLine("<tr>");
            contentBuilder.AppendLine(" <th> Title</th>");
            contentBuilder.AppendLine(" <th> Message</th>");
            contentBuilder.AppendLine("</tr>");
            //content

            foreach (var exception in exceptions)
            {
                contentBuilder.AppendLine("<tr>");
                contentBuilder.AppendLine($"    <td>{exception.Title}</td>");
                contentBuilder.AppendLine($"    <td>License missing!</td>");
                contentBuilder.AppendLine("</tr>");
            }
            contentBuilder.AppendLine("</table>");
            //end of centent
            contentBuilder.AppendLine("</body>");
            contentBuilder.AppendLine("</html>");

            return new LicensePageContent(contentBuilder.ToString(), LicenseExceptions.Count == 0);
        }

        /// <summary>
        /// build the license exception page in the environment
        /// </summary>
        /// <param name="env"> Current environment</param>
        internal static void BuildLicensePage(IWebHostEnvironment env)
        {
            if (LicenseExceptions.Count == 0) return;

            var content = BuildLicensePage("Missing license dectected!", "Your app was not able to load because of missing licenses.");
            if (content.IsEmpty) return;

            GenerateFile(env, content);

            //license exception already handled , empty the list to catch new incoming license exceptions
            LicenseExceptions.Clear();
            MissingLicenseDetected = false;
        }

        internal static void StartLicensePageBuilder()
        {
            //configure the host
            var host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<CustomStartup>();
                    webBuilder.UseIISIntegration();
                })
                .Build();
            //start the host for the license page
            host.Run();
        }

        //creates the html file on the webroot 
        private static void GenerateFile(IWebHostEnvironment env, LicensePageContent content)
        {
            var path = Path.Combine(env.WebRootPath, LicensePageRootFolder);
            var fileFullPath = Path.Combine(path, LicensePageName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.WriteAllText(fileFullPath, content.Content.ToString());
        }

        private static List<MoryxLicenseException> LicenseExceptions { get; set; } = new List<MoryxLicenseException>();

        private static void HandleMissingLicenseException(object sender, FirstChanceExceptionEventArgs e)
        {
            //skip all exceptions that are not WupiException
            if (e.Exception is not WupiException licenseException) return;

            //register all license exception to display to the user
            LicenseExceptions.Add(new MoryxLicenseException(licenseException));
            MissingLicenseDetected = true;
        }

    }

    /// <summary>
    /// License page content class use to generate the Html page content
    /// </summary>
    internal class LicensePageContent
    {
        public LicensePageContent(string content, bool isEmpty)
        {
            this.Content = content;
            this.IsEmpty = isEmpty;
        }
        public string Content { get; }
        public bool IsEmpty { get; }
    }

    /// <summary>
    /// Custom startup class to configure the license exception page host
    /// </summary>
    internal class CustomStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //generate the page
            MoryxLicenseExceptionPageBuilder.BuildLicensePage(env);

            //configure the web root folder (wwwroot/LicensePageRootFolder/)
            env.WebRootFileProvider = new PhysicalFileProvider(
                Path.Combine(env.WebRootPath, MoryxLicenseExceptionPageBuilder.LicensePageRootFolder));
            env.WebRootPath = Path.Combine(env.WebRootPath, MoryxLicenseExceptionPageBuilder.LicensePageRootFolder);
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }

}

