using bidtube.Application.Common.Contracts;
using bidtube.Domain.Entities;
using bidtube.Infrastructure.Exceptions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace bidtube.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IEventLog _logger;
        public CloudinaryService(Cloudinary cloudinary, IEventLog logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }
        public async Task<string> UploadImage(string base64, string name, CancellationToken cancellationToken)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(base64),
                UseFilename = true,
                UniqueFilename = false
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if(uploadResult != null)
            {
                if(uploadResult.Error == null)
                {
                    return uploadResult.SecureUrl.ToString();
                } 
                else
                {
                    var eventLog = new EventLog();
                    eventLog.error_timestamp = DateTime.UtcNow;
                    eventLog.error_message = uploadResult.Error.Message;
                    eventLog.error_code = ((int)uploadResult.StatusCode);
                    await _logger.WriteLog(eventLog);
                    throw new CloudinaryException("We were unable to upload the image. Please reach out to the administrator for support.");
                }
            } 
            else
            {
                var eventLog = new EventLog();
                eventLog.error_timestamp = DateTime.UtcNow;
                eventLog.error_message = "Upload result is empty.";
                eventLog.error_code = 404;
                await _logger.WriteLog(eventLog);
                throw new CloudinaryException("We were unable to upload the image. Please reach out to the administrator for support.");
            }
        }
    }
}
