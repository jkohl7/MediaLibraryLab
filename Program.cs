using System;
using NLog.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MovieListing
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "\\nlog.config";

            // create instance of Logger
            var logger = NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
            logger.Info("Program started");

            string file = "movies.csv";
             // make sure movie file exists
            if (!File.Exists(file))
            {
                logger.Error("File does not exist: {File}", file);
            }
            else
            {
            // create parallel lists of movie details
            // lists are used since we do not know number of lines of data
             List<UInt64> MovieIds = new List<UInt64>();
                List<string> MovieTitles = new List<string>();
                List<string> MovieGenres = new List<string>();
                List<string> MovieDirector = new List<string>();
                List<UInt64> MovieRunTime = new List<UInt64>();
                // to populate the lists with data, read from the data file
                try
                {
                    StreamReader sr = new StreamReader("movies.csv");
                    // first line contains column headers
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                         // first look for quote(") in string
                        // this indicates a comma(,) in movie title
                        int idx = line.IndexOf('"');
                        if (idx == -1)
                        {
                             // no quote = no comma in movie title
                            // movie details are separated with comma(,)
                            string[] movieDetails = line.Split(',');
                            // 1st array element contains movie id
                            MovieIds.Add(UInt64.Parse(movieDetails[0]));
                            // 2nd array element contains movie title
                            MovieTitles.Add(movieDetails[1]);
                            //3rd array element contains movie director
                            MovieDirector.Add(movieDetails[2]);
                            //4th array element contains movie run time
                            MovieRunTime.Add(UInt64.Parse(movieDetails[3]));
                            // 5th array element contains movie genre(s)
                            // replace "|" with ", "
                            MovieGenres.Add(movieDetails[2].Replace("|", ", "));
                        }
                         else
                        {
                            // quote = comma in movie title
                            // extract the movieId
                            MovieIds.Add(UInt64.Parse(line.Substring(0, idx - 1)));
                            // remove movieId and first quote from string
                            line = line.Substring(idx + 1);
                            // find the next quote
                            idx = line.IndexOf('"');
                            // extract the movieTitle
                            MovieTitles.Add(line.Substring(0, idx));
                            // remove title and last comma from the string
                            line = line.Substring(idx + 2);
                            // replace the "|" with ", "
                            MovieGenres.Add(line.Replace("|", ", "));
                        }
                    }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
                logger.Info("Program ended");
            }
        }
    }
}