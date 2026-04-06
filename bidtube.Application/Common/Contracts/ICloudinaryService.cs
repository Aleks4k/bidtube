using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bidtube.Application.Common.Contracts
{
    public interface ICloudinaryService
    {
        Task<string> UploadImage(string base64, string name, CancellationToken cancellationToken);
    }
}
