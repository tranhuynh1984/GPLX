using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace GPLX.Core.Extensions
{
    public class FileImageValidator : AbstractValidator<IFormFile>
    {
        private string[] _allow_img = new string[]
        {
           "image/jpeg"
            ,"image/jpg"
            ,"image/png"
            ,"image/gif"
            ,"image/webp"
        };

        public FileImageValidator()
        {

        }
    }

    public class FileDataValidator : AbstractValidator<IFormFile>
    {
        public FileDataValidator()
        {

        }
    }
}
