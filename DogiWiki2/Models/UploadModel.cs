using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DogiWiki2.Models
{
    public class UploadModel
    {
        public string Name { get; set;}

        public string Description { get; set; }

        public string SelectedBreed { get; set; }

        public static List<String> Breed = new List<string>
        {
            "",
            "Labrador",
            "Golden Retriever",
            "Double Doodle"
        };

        public static async Task WriteBlobStream(Stream blob, string containerName, string blobPath)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=dogiwikistorage;AccountKey=Ijl+ST0jBJWEMBTkT+uEaPrmqpLMTKr5eiIXDSn6X4JauwuDAcnRy6f1YDdrKh/qSFHKwOmGx/2im4/28m34Jw==;EndpointSuffix=core.windows.net");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            // Create the container if it doesn't already exist.
            //await container.CreateIfNotExistsAsync();
            
            // create a blob in the path of the <container>/
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobPath);
            blockBlob.Properties.ContentType = "image/jpg";

            await blockBlob.UploadFromStreamAsync(blob);
        }

    }

    

    

}