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
                List<string> MovieRunTime = new List<string>();
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
                            MovieRunTime.Add(movieDetails[3]);
                            // 5th array element contains movie genre(s)
                            // replace "|" with ", "
                            MovieGenres.Add(movieDetails[4].Replace("|", ", "));
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
                 string choice;
                
                do
                {
                      logger.Info("Movies in file {Count}", MovieIds.Count);
                    // display choices to user
                    Console.WriteLine("1) Add Movie");
                    Console.WriteLine("2) Display All Movies");
                    Console.WriteLine("Enter to quit");

                    // input selection
                    choice = Console.ReadLine();
                    logger.Info("User choice: {Choice}", choice);

                    if (choice == "1")
                    {
                        // Add Movie
                        // ask user to input movie title
                        Console.WriteLine("Enter the movie title");
                        // input title
                        string movieTitle = Console.ReadLine();
                        // check for duplicate title
                        List<string> LowerCaseMovieTitles = MovieTitles.ConvertAll(t => t.ToLower());
                        if (LowerCaseMovieTitles.Contains(movieTitle.ToLower()))
                        {
                            logger.Info("Duplicate movie title {Title}", movieTitle);
                        }
                        else
                        {
                            // generate movie id - use max value in MovieIds + 1
                            UInt64 movieId = MovieIds.Max() + 1;
                            //ask for director
                            Console.WriteLine("Enter movie director");
                            string director = Console.ReadLine();
                            //get run time
                            Console.WriteLine("Enter movie run time (separated by :)");
                            string runtime = Console.ReadLine();
                            // input genres
                            List<string> genres = new List<string>();
                            string genre;
                            do
                            {
                                // ask user to enter genre
                                Console.WriteLine("Enter genre (or done to quit)");
                                // input genre
                                genre = Console.ReadLine();
                                // if user enters "done"
                                // or does not enter a genre do not add it to list
                                if (genre != "done" && genre.Length > 0)
                                {
                                    genres.Add(genre);
                                }
                            } while (genre != "done");
                            // specify if no genres are entered
                            if (genres.Count == 0)
                            {
                                genres.Add("(no genres listed)");
                            }
                            // use "|" as delimeter for genres
                            string genresString = string.Join("|", genres);
                            // if there is a comma(,) in the title, wrap it in quotes
                            movieTitle = movieTitle.IndexOf(',') != -1 ? $"\"{movieTitle}\"" : movieTitle;
                            // create file from data
                            StreamWriter sw = new StreamWriter(file, true);
                            sw.WriteLine($"{movieId},{movieTitle},{MovieDirector},{MovieRunTime},{genresString}");
                            sw.Close();
                            // add movie details to Lists
                            MovieIds.Add(movieId);
                            MovieTitles.Add(movieTitle);
                            MovieDirector.Add(director);
                            MovieRunTime.Add(runtime);
                            MovieGenres.Add(genresString);
                            // log transaction
                            logger.Info("Movie id {Id} added", movieId);
                        }
                    }
                    else if (choice == "2")
                    {
                    //     // loop thru Movie Lists
                    // for (int i = 0; i < MovieIds.Count; i++)
                    // {
                    //     // display movie details
                    //     Console.WriteLine($"Id: {MovieIds[i]}");
                    //     Console.WriteLine($"Title: {MovieTitles[i]}");
                    //     Console.WriteLine($"Genre(s): {MovieGenres[i]}");
                    //     Console.WriteLine();
                    // }
                    }
                } while (choice == "1" || choice == "2");
            }
            
            logger.Info("Program ended");
        }
    }
}
