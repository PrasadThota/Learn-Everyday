using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


using System.Drawing;
using System.Globalization;

namespace MediaManager
{
    /// <summary>
    /// TODO : How do I make sure this class is visible only in this file, and to class Server ?
    /// </summary>
    //internal class MediaTask
    public class MediaTask
    {
        //private string _mediafile;

        public MediaTask(string mediaFile)
        {
            Debug.Assert(null != mediaFile);
            MediaFileInfo = new FileInfo(mediaFile);
            LastItem = false;
            CopySuccess = false;
        }
        private FileInfo MediaFileInfo { get; set; }
        public string FullName
        {
            get
            {
                return MediaFileInfo.FullName;
            }
            set { }
        }
        public string Name {
            get
            {
                return MediaFileInfo.Name;
            }
            set { }
        }
        public bool LastItem { get; set; }
        public long ProcessingTime { get; set; }
        public long CopyTime { get; set; }
        public DateTime DateCreated { get; set; }
        //indicates if this element was successfully copied over to destinatin 
        public bool CopySuccess { get;  set;}

    }
    public class Server
    {

        private static readonly string MediaSourceLocation = @"C:\Users\pthota\Pictures";
        private static readonly string MediaTargetLocation = @"C:\Users\pthota\Pictures\Temp";

        private static readonly int MillisecondsPerSecond = 1000;
        private static readonly string ProducerPrefix = @"\\t++";
        private static readonly string ConsumerPrefix = @"\t\t--";

        private static DateTime AncientDate = new DateTime(1973, 10, 09);

        // media tasks get on this que for processing. By the time they get on this que, the following information is available for the media item
        // - CreationDate
        private Queue<MediaTask> MTaskQ { get; set; }

        // This que holds media tasks that need to be processed at a later time.
        private Queue<MediaTask> MStragglerQ { get; set; }

        // Event used to by Production task to signal work to the Consumer task.
        private ManualResetEvent ProductionEvent { get; set; }
        
        // Event used by the Production task to signal work to the task that will process stragglers.
        private ManualResetEvent StragglerEvent { get; set; }

        // Destination handler
        private DestinationHandler MediaDestinationHandler {get; set;}
        public Server()
        {
            Console.WriteLine("Server class for background processing ..");
            MTaskQ = new Queue<MediaTask>();

            // This is used to communicate between the two threads. Initially set to non-signalled state. When Producer has done some portion of its work, 
            // it signals the Consumer thread.
            ProductionEvent = new ManualResetEvent(false);

            //Destination Handler object - used for copying a media element to its final destinations
            MediaDestinationHandler = new DestinationHandler();
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
                // extract creation date from each image. If a date conn't be found, assign an old date.
                if (!TryGetCreationDate(mediaTask))
                {
                    mediaTask.DateCreated = AncientDate;
                }

                // lock only for as a little time as needed.
                lock (MTaskQ)
                {
                    MTaskQ.Enqueue(mediaTask);
                    Console.WriteLine("{0} Enqueueed media task to que [{1}] : {2}",
                        ProducerPrefix,
                        MTaskQ.Count,
                        mediaTask.FullName);
                }

                // TODO : Signalling this event for each media item looks a little silly. It looks like we should signal it after a few hundred items or such.
                ProductionEvent.Set();
                // Simulate work
                //Thread.Sleep(MillisecondsPerSecond/10);
                KillTime(500);

            }

        }

        // TODO : How does consumer know when it has processed the last item ?

        public void ConsumerTask()
        {
            Console.WriteLine("Starting Consumer task ...");
            do
            {
                // wait for Producer to signal that there is data to process. If there are no items for more than 
                // 3 seconds bail.
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
                        mediaTask.FullName);
                    
                    // try writing the media file to all its destinations. If it cannot be written in the fast path, 
                    // add to the Stragglers que, and let them be processed in the slow path.
                    if (!MediaDestinationHandler.TryWrite(mediaTask))
                    {
                        // Add the media task to the Stargglers que, and signal the Straggler's thread.
                        MStragglerQ.Enqueue(mediaTask);
                        StragglerEvent.Set();
                    }

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
                
                KillTime(1000);

            } while (true);

        }


        public void StragglersTask()
        {
            Console.WriteLine("Starting Stragglers task ...");

            do
            {
                //wait to be signlled by the (fast path) consumer task
                StragglerEvent.WaitOne();

                lock (MStragglerQ)
                {
                    // If there are no items to process, exit. This can happen when all the necessary folders are in place, 
                    // and all the media elements have been procesed in the fast path.
                    if (MStragglerQ.Count == 0)
                    {
                        break;
                    }

                    var mediaTask = MStragglerQ.Dequeue();
                    Debug.Assert(null != mediaTask);

                    MediaDestinationHandler.TryWrite
                
                }

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

        /// <summary>
        /// Attempts to extract creation date out of the media item. If date is not available in the media item, 
        /// assigns an arbitrarily old date.
        /// </summary>
        /// <param name="mediaTask"></param>
        /// <returns>
        /// returns true when able to extract image successfully.
        /// </returns>
        private bool TryGetCreationDate(MediaTask mediaTask)
        {
            Debug.Assert(null != mediaTask);
            Debug.Assert(null != mediaTask.FullName);
            Debug.Assert(null == mediaTask.DateCreated);

            bool bDateExtracted = false;

            try
            {
                // create a bitmap object, and get the dateCreated property
                var bitmap = new Bitmap(mediaTask.FullName);
                var dateCreatedProperty = bitmap.GetPropertyItem(0x132);

                // Extract the date property as a string
                System.Text.ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                var dateCreatedString = asciiEncoding.GetString(dateCreatedProperty.Value, 0, dateCreatedProperty.Len - 1);

                // convert the date string in to a DateTime object
                DateTime dateCreated = DateTime.ParseExact(dateCreatedString, "yyyy.MM:dd H:m:s", CultureInfo.InvariantCulture);
                mediaTask.DateCreated = dateCreated;
                bDateExtracted = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                mediaTask.DateCreated = AncientDate;
            }

            return bDateExtracted;
            #region SO_Code
            //// This is just an example directory, please use your fully qualified file path
            //string oldFilePath = @"C:\Users\User\Desktop\image.JPG";
            //// Get the path of the file, and append a trailing backslash
            //string directory = System.IO.Path.GetDirectoryName(oldFilePath) + @"\";

            //// Get the date property from the image
            //Bitmap image = new Bitmap(oldFilePath);
            //PropertyItem test = image.GetPropertyItem(0x132);

            //// Extract the date property as a string
            //System.Text.ASCIIEncoding a = new ASCIIEncoding();
            //string date = a.GetString(test.Value, 0, test.Len - 1);

            //// Create a DateTime object with our extracted date so that we can format it how we wish
            //System.Globalization.CultureInfo provider = CultureInfo.InvariantCulture;
            //DateTime datCreated = DateTime.ParseExact(date, "yyyy:MM:d H:m:s", provider);

            //// Create our own file friendly format of daydayMonthMonthYearYearYearYear
            //string fileName = dateCreated.ToString("ddMMyyyy");

            //// Create the new file path
            //string newPath = directory + fileName + ".JPG";

            //// Use this method to rename the file
            ////System.IO.File.Move(oldFilePath, newPath);

            //Console.WriteLine(newPath);
            #endregion
        }
    }

}
