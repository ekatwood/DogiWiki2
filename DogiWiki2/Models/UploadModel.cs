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
        public string ErrorMessage { get; set; }

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
            "Airedale Terrier",
            "Akita",
            "Alaskan Malamute",
            "American English Coonhound",
            "American Eskimo Dog",
            "American Foxhound",
            "American Hairless Terrier",
            "American Staffordshire Terrier",
            "Anatolian Shepherd Dog",
            "Australian Cattle Dog",
            "Australian Shepherd",
            "Australian Terrier",
            "Basenji",
            "Basset Hound",
            "Beagle",
            "Bearded Collie",
            "Beauceron",
            "Bedlington Terrier",
            "Belgian Malinois",
            "Belgian Sheepdog",
            "Belgian Tervuren",
            "Bergamasco Sheepdog",
            "Berger Picard",
            "Bernese Mountain",
            "Bichons Frise",
            "Black Russian Terrier",
            "Black and Tan Coonhound",
            "Bloodhound",
            "Bluetick Coonhound",
            "Boerboel",
            "Border Collie",
            "Border Terrier",
            "Borzois",
            "Boston Terrier",
            "Bouviers des Flandre",
            "Boxer",
            "Briards",
            "Brittany",
            "Brussels Griffon",
            "Bull Terrier",
            "Bulldog",
            "Bullmastiff",
            "Cairn Terrier",
            "Canaan Dog",
            "Cane Corso",
            "Cardigan Welsh Corgi",
            "Cavalier King Charles Spaniel",
            "Cesky Terrier",
            "Chihuahua",
            "Chinese Crested",
            "Chinese Shar-Pei",
            "Chinook",
            "Chow Chow",
            "Cirneco dell’Etna",
            "Collie",
            "Coton de Tulear",
            "Dachshund",
            "Dalmatian",
            "Dandie Dinmont Terrier",
            "Doberman Pinscher",
            "Dogues de Bordeaux",
            "English Foxhound",
            "English Toy Spaniel",
            "Entlebucher Mountain Dog",
            "Finnish Lapphund",
            "Finnish Spitz",
            "Fox Terrier (Smooth)",
            "Fox Terrier (Wire)",
            "French Bulldog",
            "German Pinscher",
            "German Shepherd",
            "Giant Schnauzer",
            "Glen of Imaal Terrier",
            "Great Dane",
            "Great Pyrenee",
            "Greater Swiss Mountain Dog",
            "Greyhound",
            "Harrier",
            "Havanese",
            "Ibizan Hound",
            "Icelandic Sheepdog",
            "Irish Terrier",
            "Irish Wolfhound",
            "Italian Greyhound",
            "Japanese Chin",
            "Keeshonden",
            "Kerry Blue Terrier",
            "Komondorok",
            "Kuvaszok",
            "Lagotti Romagnoli",
            "Lakeland Terrier",
            "Leonberger",
            "Lhasa Apso",
            "Lowchen",
            "Maltese",
            "Manchester Terrier",
            "Mastiff",
            "Miniature American Shepherd",
            "Miniature Bull Terrier",
            "Miniature Pinscher",
            "Miniature Schnauzer",
            "Neapolitan Mastiff",
            "Newfoundland",
            "Norfolk Terrier",
            "Norwegian Buhund",
            "Norwegian Elkhound",
            "Norwegian Lundehund",
            "Norwich Terrier",
            "Old English Sheepdog",
            "Otterhound",
            "Papillon",
            "Parson Russell Terrier",
            "Pekingese",
            "Pembroke Welsh Corgi",
            "Petits Bassets Griffons Vendeen",
            "Pharaoh Hound",
            "Plott",
            "Pointer",
            "Pointer (German Shorthaired)",
            "Pointer (German Wirehaired)",
            "Polish Lowland Sheepdog",
            "Pomeranian",
            "Poodle",
            "Portuguese Podengo Pequeno",
            "Portuguese Water Dog",
            "Pug",
            "Puli",
            "Pumi",
            "Pyrenean Shepherd",
            "Rat Terrier",
            "Redbone Coonhound",
            "Retriever (Chesapeake Bay)",
            "Retriever (Curly-Coated)",
            "Retriever (Flat-Coated)",
            "Retriever (Golden)",
            "Retriever (Labrador)",
            "Retriever (Nova Scotia Duck Tolling)",
            "Rhodesian Ridgeback",
            "Rottweiler",
            "Russell Terrier",
            "Saluki",
            "Samoyed",
            "Schipperke",
            "Scottish Deerhound",
            "Scottish Terrier",
            "Sealyham Terrier",
            "Setter",
            "Setter (English)",
            "Setter (Irish Red or White)",
            "Setter (Irish)",
            "Shetland Sheepdog",
            "Shiba Inu",
            "Shih Tzu",
            "Siberian Husky",
            "Silky Terrier",
            "Skye Terrier",
            "Sloughi",
            "Soft Coated Wheaten Terrier",
            "Spaniel (American Water)",
            "Spaniel (Boykin)",
            "Spaniel (Clumber)",
            "Spaniel (Cocker)",
            "Spaniel (English Cocker)",
            "Spaniel (English Springer)",
            "Spaniel (Field)",
            "Spaniel (Irish Water)",
            "Spaniel (Sussex)",
            "Spaniel (Welsh Springer)",
            "Spanish Water Dog",
            "Spinoni Italiani",
            "St. Bernard",
            "Staffordshire Bull Terrier",
            "Standard Schnauzer",
            "Swedish Vallhund",
            "Tibetan Mastiff",
            "Tibetan Spaniel",
            "Tibetan Terrier",
            "Toy Fox Terrier",
            "Treeing Walker Coonhound",
            "Vizsla",
            "Weimaraner",
            "Welsh Terrier",
            "West Highland White Terrier",
            "Whippet",
            "Wirehaired Pointing Griffon",
            "Wirehaired Vizsla",
            "Xoloitzcuintli",
            "Yorkshire Terrier"
        };

    }

    

    

}