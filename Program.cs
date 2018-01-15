using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DownloadListOfURLs
{
    class Program
    {
        static void Main(string[] args)
        {
            string URLFile = "";
            string OutDirectory = "Downloads";

            if (args.Length > 0)
                URLFile = args[0];
            else
            {
                Console.Write("Please enter a file containing a list of URLs: ");
                URLFile = Console.ReadLine();
            }

            if(!Directory.Exists(OutDirectory))
            {
                Directory.CreateDirectory(OutDirectory);
            }

            WebClient wc = new WebClient();

            foreach(string url in File.ReadAllLines(URLFile))
            {
                bool retry = true;

                retry:
                Console.WriteLine($"Downloading file \"{url}\"...");

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
                        using (var response = client.GetAsync(url).Result)
                        {
                            // make sure our request was successful
                            response.EnsureSuccessStatusCode();

                            // read the filename from the Content-Disposition header
                            var filename = response.Content.Headers.ContentDisposition.FileName;

                            // read the downloaded file data
                            var stream = response.Content.ReadAsStreamAsync();

                            // Where you want the file to be saved
                            var destinationFile = Path.Combine(OutDirectory + "/", filename.Replace("\"", ""));

                            // write the steam content into a file
                            using (var fileStream = File.Create(destinationFile))
                            {
                                stream.Result.CopyTo(fileStream);
                            }
                        }
                    }
                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error downloading file \"{url}\" - {ex.ToString()}");
                    if(retry)
                    {
                        Console.WriteLine($"Retrying to download files \"{url}\"");
                        retry = false;
                        goto retry;
                    }
                }
            }

        }
    }
}
