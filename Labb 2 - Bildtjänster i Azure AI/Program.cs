// Importera namnrymder
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace ImageAnalysisApp
{
    class Program
    {
        //  Endpoint och nyckel
        private static readonly string endpoint = "https://compvisisonservice321.cognitiveservices.azure.com/";
        private static readonly string apiKey = "678c4c9afb4e41a5a813ee7ab4d44ebd";

        static async Task Main(string[] args)
        {
            var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey))
            {
                Endpoint = endpoint
            };

            Console.WriteLine("Enter the image path (local or URL): ");
            string imagePath = Console.ReadLine();

            //Kontrollorerar om inmatningen är en url, skapar miniatyrbilden med angiven storlek
            if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                // Analyserar bilden från url
                await AnalyzeImageUrl(client, imagePath);
                await GenerateThumbnailFromUrl(client, imagePath, 500, 500);
            }
            else if (File.Exists(imagePath))
            {
                // Analayserar bilden från lokal filväg
                await AnalyzeImagePath(client, imagePath);
                await GenerateThumbnailFromPath(client, imagePath, 500, 500);
            }
            else
            {
                Console.WriteLine("Invalid image path.");
            }
        }

        //Tar emot en url inmkatning och analyserar den, samt hämtar taggar och beskrivning
        static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            var features = new List<VisualFeatureTypes?>
    {
        VisualFeatureTypes.Tags,
        VisualFeatureTypes.Description
    };

            var analysis = await client.AnalyzeImageAsync(imageUrl, features);

            Console.WriteLine("Image tags:");
            foreach (var tag in analysis.Tags)
            {
                Console.WriteLine($" - {tag.Name} ({tag.Confidence:P})");
            }

            Console.WriteLine($"Description: {analysis.Description.Captions[0].Text}");
        }

        //Tar emot en lokal filväg från inmkatning och analyserar den, samt hämtar taggar och beskrivning
        static async Task AnalyzeImagePath(ComputerVisionClient client, string imagePath)
        {
            using var imageStream = File.OpenRead(imagePath);
            var features = new List<VisualFeatureTypes?>
    {
        VisualFeatureTypes.Tags,
        VisualFeatureTypes.Description
    };

            var analysis = await client.AnalyzeImageInStreamAsync(imageStream, features);

            Console.WriteLine("Image tags:");
            foreach (var tag in analysis.Tags)
            {
                Console.WriteLine($" - {tag.Name} ({tag.Confidence:P})");
            }

            Console.WriteLine($"Description: {analysis.Description.Captions[0].Text}");
        }

        //Genererar en miniatyrbild från url
        static async Task GenerateThumbnailFromUrl(ComputerVisionClient client, string imageUrl, int width, int height)
        {
            var thumbnailStream = await client.GenerateThumbnailAsync(width, height, imageUrl, smartCropping: true);

            using var fileStream = File.Create("thumbnail.jpg");
            thumbnailStream.CopyTo(fileStream);
            Console.WriteLine("Thumbnail saved as 'thumbnail.jpg'");
        }

        //Genererar en miniatyrbild lokalt från en filväg
        static async Task GenerateThumbnailFromPath(ComputerVisionClient client, string imagePath, int width, int height)
        {
            using var imageStream = File.OpenRead(imagePath);
            var thumbnailStream = await client.GenerateThumbnailInStreamAsync(width, height, imageStream, smartCropping: true);

            using var fileStream = File.Create("thumbnail.jpg");
            thumbnailStream.CopyTo(fileStream);
            Console.WriteLine("Thumbnail saved as 'thumbnail.jpg'");
        }
    }
}
