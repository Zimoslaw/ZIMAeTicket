using Org.BouncyCastle.Utilities;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZIMAeTicket.Services
{
    public class PDFService
    {
        const string title = "Twój bilet na wydarzenie";
        const string subject = "Twój bilet zakupiony na stronie zima.sklep.pl. Zachowaj go i pokaż na wejściu";

        public async Task<byte[]> CreateNewPDFTicket(MemoryStream qrImage, string orderId, string orderEmail, string orderDate, int ticketNo, string eventName = "Wydarzenie")
        {
            try
            {
                // PDF setup
                GlobalFontSettings.UseWindowsFontsUnderWindows = true;
                PdfDocument document = new();
                document.Info.Title = title;
                document.Info.Subject = subject;
                var page = document.AddPage();
                page.Size = PageSize.A4;
                page.Orientation = PageOrientation.Portrait;
                var fontHeadline = new XFont("Arial", 24, XFontStyleEx.Bold);
                var fontText = new XFont("Arial", 16);
                var fontFooter = new XFont("Arial", 12);
                var qr = XImage.FromStream(qrImage);
                var logo = XImage.FromFile(await CopyImageFromResourcesAsync("logo.png"));
                var gfx = XGraphics.FromPdfPage(page);

                // Template
                int currentHeight = 72;
                gfx.DrawString($"Twój bilet nr {ticketNo} na {eventName}", fontHeadline, XBrushes.Black,
                    new XRect(72, currentHeight, page.Width.Point - 144, 0), XStringFormats.BaseLineCenter);

                gfx.DrawString($"E-mail: {orderEmail}", fontText, XBrushes.Black,
                    new XRect(72, currentHeight += 32, page.Width.Point, 0), XStringFormats.BaseLineLeft);

                gfx.DrawString($"Zamówienie nr: {orderId}", fontText, XBrushes.Black,
                    new XRect(72, currentHeight += 24, page.Width.Point, 0), XStringFormats.BaseLineLeft);

                gfx.DrawString($"Data zamówienia: {orderDate}", fontText, XBrushes.Black,
                    new XRect(72, currentHeight += 24, page.Width.Point, 0), XStringFormats.BaseLineLeft);

                gfx.DrawImage(qr, 147, currentHeight += 16, 300, 300); // QR code

                gfx.DrawString("Zapisz ten plik na telefonie lub wydrukuj go.",
                    fontText, XBrushes.Black, new XRect(147, currentHeight += 316, 300, 0), XStringFormats.BaseLineCenter);
                gfx.DrawString("Aby wejść na wydarzenie niezbędne będzie ukazanie tego pliku",
                    fontText, XBrushes.Black, new XRect(147, currentHeight += 16, 300, 0), XStringFormats.BaseLineCenter);
                gfx.DrawString("(w celu zeskanowania kodu QR)",
                    fontText, XBrushes.Black, new XRect(147, currentHeight += 16, 300, 0), XStringFormats.BaseLineCenter);

                gfx.DrawImage(logo, 247, 610, 100, 100); // Logo

                // Template footer
                gfx.DrawString("ZIMA Firma Fonograficzno-Handlowa", 
                    fontFooter, XBrushes.Black, new XRect(72, 730, page.Width.Point - 144, 0), XStringFormats.BaseLineCenter);
                gfx.DrawString("ul. Bankowa 1/9 44-100 Gliwice", 
                    fontFooter, XBrushes.Black, new XRect(72, 744, page.Width.Point - 144, 0), XStringFormats.BaseLineCenter);
                gfx.DrawString("https://zima.sklep.pl", 
                    fontFooter, XBrushes.Black, new XRect(72, 756, page.Width.Point - 144, 0), XStringFormats.BaseLineCenter);

                // Saving PDF to bytes
                byte[] pdfTicketBytes;
                using (MemoryStream ms = new())
                {
                    document.Save(ms);
                    pdfTicketBytes = ms.ToArray();
                }

                return pdfTicketBytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in creating ticket PDF: {ex}");
            }
        }

        private async Task<string> CopyImageFromResourcesAsync(string fileName)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            var resourcePath = $"{assembly.GetName().Name}.Resources.Raw.{fileName}";
            string targetPath = "";

            using (Stream resourceStream = await FileSystem.OpenAppPackageFileAsync(fileName))
            {
                if (resourceStream == null)
                    throw new FileNotFoundException("Resource not found: " + resourcePath);

                targetPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                using FileStream fileStream = File.Create(targetPath);
                resourceStream.CopyTo(fileStream);
            }

            return targetPath;
        }
    }
}
