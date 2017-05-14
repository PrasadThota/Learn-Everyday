using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Coding
{
    /// <summary>
    /// TODO : How do I make sure this class is visible only in this file, and to class Server ?
    /// </summary>
    internal class MediaTask
    {
        //private string _mediafile;

        public MediaTask(string mediaFile)
        {
            Debug.Assert(null != mediaFile);
            MediaFile = mediaFile;
        }
        public string MediaFile { get; set; }
        public bool LastItem { get; set; }
    }
    public class Server
    {

        private static readonly string MediaSourceLocation = @"C:\Users\pthota\Pictures";
        private static readonly string MediaTargetLocation = @"C:\Users\pthota\Pictures\Temp";

        private static readonly int MillisecondsPerSecond = 1000;
        private static readonly string ProducerPrefix = @"\\t++";
        private static readonly string ConsumerPrefix = @"\t\t--";

        private Queue<MediaTask> MTaskQ { get; set; }
        private ManualResetEvent ProductionEvent { get; set; }

        public Server()
        {
            Console.WriteLine("Server class for background processing ..");
            MTaskQ = new Queue<MediaTask>();

            // This is used to communicate between the two threads. Initially set to non-signalled state. When Producer has done some portion of its work, 
            // it signals the Consumer thread.
            ProductionEvent = new ManualResetEvent(false);
        }

        private IEnumerable<MediaTask> GetMediaFile()
        {
            try
            {
                var jpgFiles = Directory.EnumerateFiles(MediaSourceLocation, "*.jpg");

                foreach (var image in jpgFiles)
                {
                    yield return new MediaTask(image);
                }
            }
#if false
            // TODO : yield can't occur when there is caatch clause around. Read up some more
            http://stackoverflow.com/questions/346365/why-cant-yield-return-appear-inside-a-try-block-with-a-catch
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            } 
#endif
            finally
            {
                // TODO : Cannot return anything from here.
                //var lastTask = new MediaTask(null);
                //return lastTask;
            }
        }

        // TODO : 
        // Producer needs to "run first" and produce some stuff. Until such time, Consumer should sit tight.
        // There is no point for consumer to wake up, figure out there is nothing in the Q, and get all confused.
        // Question is what is a good way for Consumer to wait for Producer. 
        //

        public void ProducerTask()
        {
            Console.WriteLine("Inserting items to que ...");

            // get a file from source location and insert it in to que, and sleep
            foreach (var mediaTask in GetMediaFile())
            {
                // lock only for as a little time as needed.
                lock (MTaskQ)
                {
                    MTaskQ.Enqueue(mediaTask);
                    Console.WriteLine("{0} Enqueueed media task to que [{1}] : {2}",
                        ProducerPrefix,
                        MTaskQ.Count,
                        mediaTask.MediaFile);
                }

                ProductionEvent.Set();
                // Simulate work
                //Thread.Sleep(MillisecondsPerSecond/10);
                KillTime(500);

            }

        }

        // TODO : How does consumer know when it has processed the last item ?

        public void ConsumerTask()
        {
            Console.WriteLine("Processing from the queue..");
            do
            {
                // wait for Producer to signal that there is data to process
                var dataAvailable = ProductionEvent.WaitOne(MillisecondsPerSecond * 3);
                if (false == dataAvailable)
                {
                    Console.WriteLine("{0} Done processing data", ConsumerPrefix);
                    break;
                }

                MediaTask mediaTask = null;
                // TODO : Can locks be acquired on instance variables ?
                lock (MTaskQ)
                {
                    mediaTask = MTaskQ.Dequeue();
                    Console.WriteLine("{0} Dequeued media task frm que [{1}]) : {2}",
                        ConsumerPrefix,
                        MTaskQ.Count,
                        mediaTask.MediaFile);

                    // if all the items are exhausted, go back ot waiting. Producer will add more to the que, and signal us.
                    if (MTaskQ.Count == 0)
                    {
                        Console.WriteLine(ConsumerPrefix + "Q lenght is 0. Waiting for more data ...");
                        ProductionEvent.Reset();
                    }
                }

                // Simulate work
                //Thread.Sleep(MillisecondsPerSecond * 2);

                //copy image to the destination location
                Debug.Assert(null != mediaTask);
                var sourceFile = new FileInfo(mediaTask.MediaFile);
                var destFile = Path.Combine(MediaTargetLocation, sourceFile.Name);
                File.Copy(sourceFile.FullName, destFile, true);
                KillTime(1000);

            } while (true);

        }

        /// <summary>
        /// Create a Producer thread
        /// Create a Consumer thread
        /// 
        /// </summary>
        public void DoWork()
        {
            var producerTh = new Thread(this.ProducerTask);
            var consumerTh = new Thread(this.ConsumerTask);

            producerTh.Start();
            consumerTh.Start();
        }

        private void KillTime(uint ms)
        {
            var now = DateTime.Now;
            var now_1 = now.AddMilliseconds(ms);

            uint randCount = 0;
            do
            {
                var rand = new Random();
                randCount++;
                now = DateTime.Now;
            } while (now < now_1);
            Console.WriteLine("KillTime : Generated {0} random numbers", randCount);

        }
    }

}
