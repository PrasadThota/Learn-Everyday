using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

//
// TODO : should this be a static class ?
//
namespace MediaManager
{
    public class DestinationHandler
    {
        //MediaDestinations
        public List<DirectoryInfo> MediaDestinations { get; set; }
        public Stopwatch stopWatch { get; set; }

        /// <summary>
        /// Reads the destination locations
        /// </summary>
        public DestinationHandler()
        {
            var mediaLocationConfig = (NameValueCollection)ConfigurationManager.GetSection("MediaLocations");

            //<add key="SourceLocations" value="C:\Users\pthota\Pictures # C:\Users\pthota\Pictures\ananya"/>
            //< add key = "DestinationLocations"
            var sourceLocationsString = mediaLocationConfig["SourceLocations"];
            var destinationLocationsString = mediaLocationConfig["DestinationLocations"];

            var destLocations = destinationLocationsString.Split('#');

            var _destinationList = new List<DirectoryInfo>();
            foreach(var destination in destLocations)
            {
                try
                {
                    var dInfo = new DirectoryInfo(destination.Trim());
                    _destinationList.Add(dInfo);

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            MediaDestinations = _destinationList;

            stopWatch = new Stopwatch();
        }

        /// <summary>
        /// Attempts to copy the media element to all the destinations. If done successfully, updates the mediaTask appropriately.
        /// This is the fast path.
        /// </summary>
        /// <param name="mediaTask"></param>
        /// <returns></returns>
        public bool TryWrite(MediaTask mediaTask)
        {
            bool bWrite = false;

            try
            {

                var sourceFile = mediaTask.FullName;

                stopWatch.Reset();
                stopWatch.Start();

                // copy the media file to each destination in the list of destinations.
                foreach (var destination in MediaDestinations)
                {
                    var destFile = Path.Combine(destination.FullName, sourceFile);
                    File.Copy(sourceFile, destFile);
                }
                bWrite = true;
                mediaTask.CopySuccess = true;

                stopWatch.Stop();
                mediaTask.CopyTime = stopWatch.ElapsedMilliseconds;
            }
            catch(Exception ex)
            {
                // This is the fast path, try not doing any blocking operations here. 
                //CreateDestinationfolder(mediaTask);
                Console.WriteLine(ex.Message);
            }
            return bWrite;
        }

        /// <summary>
        /// This does all the prep work needed to ensure that the copy succeeds. This should not fail. Slow path.
        /// </summary>
        /// <param name="mediaTask"></param>
        /// <returns></returns>
        public bool Write(MediaTask mediaTask)
        {
            bool bWrite = false;

            Debug.Assert(null != mediaTask);
            Debug.Assert(null != mediaTask.DateCreated);

            // TODO - How do we handle exceptions 
            // create the necessary folders before attempting a write. The write here should succeed.
            CreateDestinationfolder(mediaTask
            bWrite = TryWrite(mediaTask);
            Debug.Assert(true == bWrite);

            return bWrite;
        }

        /// <summary>
        /// Creates the "year-month" folders based on the DateTaken data in the media file. 
        /// </summary>
        /// <param name="mediaTask"></param>
        private void CreateDestinationfolder(MediaTask mediaTask)
        {
            Debug.Assert(null != mediaTask);

            var dateCreated = mediaTask.DateCreated;
            var newFolderName = String.Format("{0}-{1}", dateCreated.Year, dateCreated.Month);

            try
            {
                foreach (var destination in MediaDestinations)
                {
                    var destinationFolder = Path.Combine(destination.FullName, newFolderName);

                    //Debug.Assert(!Directory.Exists(destinationFolder));
                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}