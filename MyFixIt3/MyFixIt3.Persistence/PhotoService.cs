using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using MyFixIt.Logging;

namespace MyFixIt3.Persistence
{
    public class PhotoService : IPhotoService
    {
        readonly Logger _logger = new Logger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public async Task<string> UploadPhotoAsync(HttpPostedFileBase photo)
        {
            if (photo == null || photo.ContentLength == 0)
            {
                return null;
            }

            try
            {
                var storageAccount = CloudStorageAccount.Parse(
    ConfigurationManager.ConnectionStrings["ImageStorageConnectionString"].ConnectionString);

                var blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                var container = blobClient.GetContainerReference("image");

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();

                var imageBlobName = new StringBuilder();
                imageBlobName.AppendFormat("Image-{0}-{1}",
                    Guid.NewGuid(),
                    photo.FileName);

                var blockBlob = container.GetBlockBlobReference(imageBlobName.ToString());
                await blockBlob.UploadFromStreamAsync(photo.InputStream);

                var fullPath = blockBlob.Uri.ToString();
                return fullPath;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UploadPhotoAsync faied");
                throw;
            }

        }
    }
}
