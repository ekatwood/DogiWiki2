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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static List<String> Breed = new List<string>
        {
            "",
            "Mix",
            "Affenpinscher",
            "Afghan Hound",
            "Akita",
            "Alaskan Malamute",
            "American Eskimo Dog",
            "American Foxhound",
            "Anatolian Shepherd Dog",
            "Australian Cattle Dog",
            "Australian Shepherd",
            "Basenji",
            "Basset Hound",
            "Beagle",
            "Bearded Collie",
            "Beauceron",
            "Belgian Malinois",
            "Belgian Tervuren",
            "Berger Picard",
            "Bernese Mountain",
            "Bichons Frise",
            "Bloodhound",
            "Boerboel",
            "Border Collie",
            "Borzois",
            "Bouviers des Flandre",
            "Boxer",
            "Briards",
            "Brittany",
            "Brussels Griffon",
            "Bulldog",
            "Bullmastiff",
            "Canaan Dog",
            "Cane Corso",
            "Chihuahua",
            "Chinese Crested",
            "Chinese Shar-Pei",
            "Chinook",
            "Chow Chow",
            "Cirneco dell’Etna",
            "Collie",
            "Coonhound",
            "Corgi",
            "Coton de Tulear",
            "Dachshund",
            "Dalmatian",
            "Doberman Pinscher",
            "Dogues de Bordeaux",
            "Double Doodle",
            "English Foxhound",
            "Entlebucher Mountain Dog",
            "Finnish Lapphund",
            "Finnish Spitz",
            "French Bulldog",
            "German Pinscher",
            "German Shepherd",
            "Giant Schnauzer",
            "Goldendoodle",
            "Golden Retriever",
            "Great Dane",
            "Great Pyrenee",
            "Greater Swiss Mountain Dog",
            "Greyhound",
            "Harrier",
            "Havanese",
            "Ibizan Hound",
            "Irish Wolfhound",
            "Italian Greyhound",
            "Japanese Chin",
            "Keeshonden",
            "Komondorok",
            "Kooikerhondje",
            "Kuvaszok",
            "Labrador",
            "Labradoodle",
            "Lagotti Romagnoli",
            "Leonberger",
            "Lhasa Apso",
            "Lowchen",
            "Maltese",
            "Mastiff",
            "Miniature American Shepherd",
            "Miniature Pinscher",
            "Miniature Schnauzer",
            "Neapolitan Mastiff",
            "Newfoundland",
            "Norwegian Buhund",
            "Norwegian Elkhound",
            "Norwegian Lundehund",
            "Otterhound",
            "Papillon",
            "Pekingese",
            "Petits Bassets Griffons Vendeen",
            "Pharaoh Hound",
            "Plott",
            "Pointer",
            "Pomeranian",
            "Poodle",
            "Portuguese Podengo Pequeno",
            "Portuguese Water Dog",
            "Pug",
            "Puli",
            "Pumi",
            "Pyrenean Shepherd",
            "Retriever (Chesapeake Bay)",
            "Retriever (Nova Scotia Duck Tolling)",
            "Rhodesian Ridgeback",
            "Rottweiler",
            "Saluki",
            "Samoyed",
            "Schipperke",
            "Scottish Deerhound",
            "Setter",
            "Sheepdog",
            "Shiba Inu",
            "Shih Tzu",
            "Siberian Husky",
            "Sloughi",
            "Spaniel",
            "Spanish Water Dog",
            "Spinoni Italiani",
            "St. Bernard",
            "Standard Schnauzer",
            "Swedish Vallhund",
            "Terrier",
            "Tibetan Mastiff",
            "Vizsla",
            "Weimaraner",
            "Whippet",
            "Wirehaired Pointing Griffon",
            "Wirehaired Vizsla",
            "Xoloitzcuintli",
        };

    }

    

    

}