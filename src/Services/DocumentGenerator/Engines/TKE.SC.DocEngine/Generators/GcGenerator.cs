using GrapeCity.Documents.Html;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TKE.CPQ.DocEngine.Generators
{
    public class GcGenerator : IDocumentGenerator
    {
        private readonly IConfiguration _configuration;

        public GcGenerator(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public Stream CreateDocument(string templatePath, string data, string header)
        {
            var tmp = Path.GetTempFileName();
            var document = File.ReadAllText(templatePath);
            document = document.Replace("###Content###", data);
            var licenseKey = _configuration["GcPdf:LicenseKey"] ?? "ThyssenKrupp Elevator Americas Corp.,875245821616987#A0JE5M8UDNyUzN8IiOiQWSisnOiQkIsISP3E5VNNEdxY6ZzBzUFFkRo96SOdmcRlEVYJlUHpEO5QHWZhETzJHawtme72kdq36VLVjWTNFW5E4LhZVZMp6dIRlTptEetBFOwJDVzpXWkZnVZlnMrZmYiojITJCL5ETO6gDN6YTM0IicfJye35XX3JCSTdVUiojIDJCLiQjdgQXZO9CImRGUgI7bmBCduVWb5N6bEByQHJiOi8kI1tlOiQmcQJCLiYDM5ATNwACOwUDMxIDMyIiOiQncDJCLi8Ccy36QgMXYjlmcl5WQgI7b4FmdlxWRgAHc5J7SuV6czlHaUJiOiEmTDJCLicDOtY8M";

            using (var htmlRenderer = new GcHtmlRenderer(document))
            {
                var pdfSettings = GetPdfSettings();
                pdfSettings.HeaderTemplate = header;
                htmlRenderer.ApplyGcPdfLicenseKey(licenseKey);
                htmlRenderer.RenderToPdf(tmp, pdfSettings);
            }

            //Copy the created PDF from the temp file to target stream
            return File.OpenRead(tmp);

        }

        public Stream CreateDocument()
        {
            throw new NotImplementedException();
        }

        public Stream CreateDocument(string templatePath, string data)
        {
            throw new NotImplementedException();
        }

        public void Save(Stream stream, bool incrementalUpdate = false)
        {

        }

        public void Save(string fileName, bool incrementalUpdate = false)
        {
            throw new NotImplementedException();
        }

        private PdfSettings GetPdfSettings()
        {
            var pdfSettingsPath = "GcPdf:PdfSettings:";
            return new PdfSettings()
            {
                Margins = new Margins(0.2f, 1.7f, 0.2f, 0.5f),
                IgnoreCSSPageSize = Convert.ToBoolean(_configuration[$"{pdfSettingsPath}IgnoreCssPageSize"]),
                DisplayHeaderFooter = Convert.ToBoolean(_configuration[$"{pdfSettingsPath}DisplayHeaderFooter"]),
                FooterTemplate = _configuration[$"{pdfSettingsPath}FooterTemplate"]
            };
        }
    }
}
