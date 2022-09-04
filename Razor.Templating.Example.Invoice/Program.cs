using System;
using System.Collections.Generic;
using Razor.Templating.Core;
using System.Threading.Tasks;
using jsreport.Types;
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Razor.Templating.Example.Invoice
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var invoiceModel = new Templates.Invoice
            {
                InvoiceNumber = "3232",
                CreatedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                CompanyAddress = new Templates.Address
                {
                    Name = "XY Technologies",
                    AddressLine1 = "XY Street, Park Road",
                    City = "Chennai",
                    Country = "India",
                    Email = "xy-email@gmail.com",
                    PinCode = "600001"
                },
                BillingAddress = new Templates.Address
                {
                    Name = "XY Customer",
                    AddressLine1 = "ZY Street, Loyal Road",
                    City = "Bangalore",
                    Country = "India",
                    Email = "xy-customer@gmail.com",
                    PinCode = "343099"
                },
                PaymentMethod = new Templates.PaymentMethod
                {
                    Name = "Cheque",
                    ReferenceNumber = "94759849374"
                },
                LineItems = new List<Templates.LineItem>
        {
            new Templates.LineItem
            {
            Id = 1,
            ItemName = "USB Type-C Cable",
            Quantity = 3,
            PricePerItem = 10.33M
            },
               new Templates.LineItem
            {
            Id = 1,
            ItemName = "SSD-512G",
            Quantity = 10,
            PricePerItem = 90.54M
            }
        },
                CompanyLogoUrl = "https://raw.githubusercontent.com/soundaranbu/RazorTemplating/master/src/Razor.Templating.Core/assets/icon.png"
            };
            Console.WriteLine("genrating pdf ");
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine(stopwatch.ToString());
            var html = await RazorTemplateEngine.RenderAsync("~/Invoice.cshtml", invoiceModel);
            stopwatch.Stop();
            //var rs = new LocalReporting()
            //           .KillRunningJsReportProcesses()
            //           .UseBinary(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? JsReportBinary.GetBinary() : jsreport.Binary.Linux.JsReportBinary.GetBinary())
            //           .Configure(cfg => cfg.AllowedLocalFilesAccess().FileSystemStore().BaseUrlAsWorkingDirectory())
            //           .AsUtility()
            //           .Create();
            var rs = new LocalReporting()
                .UseBinary(JsReportBinary.GetBinary())
                .Configure(cfg => cfg.AllowedLocalFilesAccess()
                .BaseUrlAsWorkingDirectory())
                .AsUtility()
                .Create();

            Stopwatch stopwatch1 = Stopwatch.StartNew();
            var generatedPdf = await rs.RenderAsync(new RenderRequest
            {
                Template = new Template
                {
                    Recipe = Recipe.ChromePdf,
                    Engine = Engine.None,
                    Content = html,
                    Chrome = new Chrome
                    {
                        MarginTop = "10",
                        MarginBottom = "10",
                        MarginLeft = "50",
                        MarginRight = "50"
                    }
                }
            });
        
            var purchaseOrdersPath = @"C:\Users\Burhan\Desktop\Invoice\Razor.Templating.Example.Invoice\NewFolder\";
            bool exists = System.IO.Directory.Exists(purchaseOrdersPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(purchaseOrdersPath);
            string path = System.IO.Path.Join(purchaseOrdersPath, $"PurchaseOrder{"Burhan"}.pdf");
            using (FileStream fs = System.IO.File.Create(path))
            {
                generatedPdf.Content.CopyTo(fs);
            }
            Console.WriteLine(html);      

            Console.ReadLine();
        }
    }
}
