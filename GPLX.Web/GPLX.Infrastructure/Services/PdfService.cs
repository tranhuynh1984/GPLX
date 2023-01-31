using System;
using System.IO;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using iText.Html2pdf;

namespace GPLX.Infrastructure.Services
{
    public class PdfService
    {
        private static readonly FluidParser _parser = new FluidParser(new FluidParserOptions { AllowFunctions = true });

        public static async Task ExportPdfFromFilePath(string templatePath, object parseData, MemoryStream writeStream, TemplateOptions options)
        {
            var rawTemplate = await System.IO.File.ReadAllTextAsync(templatePath);
            await ExportPdf(rawTemplate, parseData, writeStream, options);
        }
        public static async Task ExportPdf(string rawTemplate, object parseData, MemoryStream writeStream, TemplateOptions options)
        {
            if (_parser.TryParse(rawTemplate, out var template, out var error))
            {   
                var context = new TemplateContext(parseData, options);
                var renderHtml = await template.RenderAsync(context);
                HtmlConverter.ConvertToPdf(renderHtml, writeStream);
            }
            else
            {
                Console.WriteLine($"Error: {error}");
            }
        }

        public FunctionValue ToLowerCaseFunc()
        {
            return new FunctionValue((args, context) => 
            {
                var firstArg = args.At(0).ToStringValue();
                if (firstArg == null) firstArg = ""; 
                
                var lower = firstArg.ToLower();
                return new ValueTask<FluidValue>(new StringValue(lower));
            });
        }
    }
}