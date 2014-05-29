using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelProgrammingTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            const string url = "http://www.matlus.com";

            //DownloadHtmlOldWay(url);
            DownloadHtmlAsync(url);
            Console.ReadLine();
        }

        private static void DownloadHtmlOldWay(string url)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.BeginGetResponse(ar =>
            {
                var webResponse = httpRequest.EndGetResponse(ar);
                using (var stream = webResponse.GetResponseStream())
                {
                    var manualResetEvent = new ManualResetEvent(false);

                    var buffer = new byte[2048];

                    // In order to have access to the recursion variable from both places this var must be declared first, before it can be used in the next line
                    AsyncCallback readCallback = null;
                    readCallback = srr => 
                        {
                            var bytesRead = stream.EndRead(srr);
                            if (bytesRead > 0)
                            {
                                Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                                // do a reccursive call to the same stream, and keep reading the bytes, and call the same callback as long as bytesRead > 0;
                                stream.BeginRead(buffer, 0, buffer.Length, readCallback, null);
                            }
                            else 
                            { 
                                // Mark that we're done
                                manualResetEvent.Set();
                            }
                        }; 
                    stream.BeginRead(buffer, 0, buffer.Length, readCallback , null);       // start here and do callback on completion
                    manualResetEvent.WaitOne();
                }
            }, null);
        }

        private static async Task DownloadHtmlAsync(string url)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            var webResponse = await httpRequest.GetResponseAsync();

            using (var stream = webResponse.GetResponseStream())
            {
                var buffer = new byte[2048];
                var bytesRead = 0;
                while((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                }
            }
        }

    }
}
