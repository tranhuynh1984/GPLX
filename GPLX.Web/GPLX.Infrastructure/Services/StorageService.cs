using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.DTO;
using GPLX.Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace GPLX.Infrastructure.Services
{
    public class StorageService
    {
        public static async Task<List<FileResponse>> UploadFiles(string uploadRootPath, 
            [NotNull] StorageConfig storageConfig,
            string folderPath,
            [NotNull] IFormFileCollection formFileCollection)
        {
            try
            {
                var listFiles = new List<FileResponse>();
                foreach (IFormFile formFile in formFileCollection)
                {
                    var file = await UploadFile(uploadRootPath, storageConfig, folderPath, formFile);
                    listFiles.Add(file);
                }

                return listFiles;
            } catch (Exception e)
            {
                throw new UploadFileFailedException("Upload file thất bại!", e);
            }
        }
        
        public static async Task<FileResponse> UploadFile(string uploadRootPath, [NotNull] StorageConfig storageConfig,
            string folderPath, [NotNull] IFormFile formFile)
        {
            try
            {
                var id = Guid.NewGuid();
                var fileExt = Path.GetExtension(formFile.FileName);
                var fileName = $"{id.ToString()}{fileExt}";
                string uploadPath = $"{uploadRootPath}{storageConfig.BasePath}{folderPath}{fileName}";
                await CopyFile(formFile, uploadPath);

                return new FileResponse
                {
                    Id = Guid.NewGuid(),
                    BasePath = $"{uploadRootPath}{storageConfig.BasePath}",
                    Ext = fileExt,
                    Path = uploadPath,
                    Size = formFile.Length,
                    FileName = fileName,
                    OriginalFileName = formFile.FileName,
                    ObjectId = "",
                    ObjectType = 1,
                    Url = $"{storageConfig.BaseUrl}{folderPath}{fileName}"
                };
            } catch (Exception e)
            {
                throw new UploadFileFailedException("Upload file thất bại!", e);
            }
        }
        
        private static async Task CopyFile(IFormFile file, string uploadPath)
        {
            var folderPath = string.Join(@"/", uploadPath
                .Split(@"/", StringSplitOptions.RemoveEmptyEntries)
                .SkipLast(1));

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await file.CopyToAsync(stream).ConfigureAwait(false);
            }
        }
    }

    public class FileResponse
    {
        public Guid Id { get; set; }
        public string BasePath { get; set; }
        public string Ext { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ObjectId { get; set; }
        public int ObjectType { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
    }
}