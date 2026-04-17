using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> UploadFileAsync(IFormFile file);
        void Delete(string fileName);
    }
}
