
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace ImagesBugMaui
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Preferences.Set("key_api_tmdb", "yourapikey");
            Preferences.Set("language_api_tmdb", "es-MX");
            Preferences.Set("language_details_api_tmdb", "es");
            Preferences.Set("image_language_api_tmdb", "es");
            Preferences.Set("region_api_tmdb", "US");

            GetCatalog(TypeCatalog.Carousel);
            GetCatalog(TypeCatalog.TopMovies);
            GetCatalog(TypeCatalog.TopMovieUpComming);
            GetCatalog(TypeCatalog.TopTvShows);
            GetCatalog(TypeCatalog.TopRatedTvShows);
            GetCatalog(TypeCatalog.TopAnime);
        }
        List<movie> _listaCarousel;

        public List<movie> ListaCarousel
        {
            get { return _listaCarousel; }
            set { SetProperty(ref _listaCarousel, value); }
        }

        List<movie> _listaCarouselReload;
        public List<movie> ListaCarouselReload
        {
            get { return _listaCarouselReload; }
            set
            {
                SetProperty(ref _listaCarouselReload, value);
            }
        }


        /// <summary>
        /// https://www.themoviedb.org/movie
        /// </summary>
        List<movie> _listaTopMoviesPopular;
        public List<movie> ListaTopMovies
        {
            get { return _listaTopMoviesPopular; }
            set { SetProperty(ref _listaTopMoviesPopular, value); }
        }

        List<movie> _listaTopMoviesPopularReload;
        public List<movie> ListaTopMoviesReload
        {
            get { return _listaTopMoviesPopularReload; }
            set
            {
                SetProperty(ref _listaTopMoviesPopularReload, value);
            }
        }

        /// <summary>
        /// https://www.themoviedb.org/movie/now-playing
        /// </summary>
        List<movie> _listaTopMoviesNowPlaying;
        public List<movie> ListaTopMoviesNowPlaying
        {
            get { return _listaTopMoviesNowPlaying; }
            set { SetProperty(ref _listaTopMoviesNowPlaying, value); }
        }

        List<movie> _listaTopMoviesNowPlayingReload;
        public List<movie> ListaTopMoviesNowPlayingReload
        {
            get { return _listaTopMoviesNowPlayingReload; }
            set
            {
                SetProperty(ref _listaTopMoviesNowPlayingReload, value);
            }
        }


        /// <summary>
        /// https://www.themoviedb.org/movie/upcoming
        /// </summary>
        List<movie> _listaTopMoviesUpComming;
        public List<movie> ListaTopMoviesUpComming
        {
            get { return _listaTopMoviesUpComming; }
            set { SetProperty(ref _listaTopMoviesUpComming, value); }
        }

        List<movie> _listaTopMoviesUpCommingReload;
        public List<movie> ListaTopMoviesUpCommingReload
        {
            get { return _listaTopMoviesUpCommingReload; }
            set
            {
                SetProperty(ref _listaTopMoviesUpCommingReload, value);
            }
        }

        /// <summary>
        /// https://www.themoviedb.org/tv
        /// </summary>
        List<movie> _listaTopTvShows;
        public List<movie> ListaTopTvShows
        {
            get { return _listaTopTvShows; }
            set { SetProperty(ref _listaTopTvShows, value); }
        }

        List<movie> _listaTopTvShowsReload;
        public List<movie> ListaTopTvShowsReload
        {
            get { return _listaTopTvShowsReload; }
            set
            {
                SetProperty(ref _listaTopTvShowsReload, value);
            }
        }

        /// <summary>
        /// https://www.themoviedb.org/tv/top-rated
        /// </summary>
        List<movie> _listaTopRatedTvShows;
        public List<movie> ListaTopRatedTvShows
        {
            get { return _listaTopRatedTvShows; }
            set { SetProperty(ref _listaTopRatedTvShows, value); }
        }

        List<movie> _listaTopRatedTvShowsReload;
        public List<movie> ListaTopRatedTvShowsReload
        {
            get { return _listaTopRatedTvShowsReload; }
            set
            {
                SetProperty(ref _listaTopRatedTvShowsReload, value);
            }
        }

        bool isRefreshing = false;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        bool isRefreshing2 = false;
        public bool IsRefreshing2
        {
            get { return isRefreshing2; }
            set
            {
                isRefreshing2 = value;
                OnPropertyChanged();
            }
        }


        public enum TypeCatalog
        {
            None = 0,
            Carousel = 1,
            TopMovies = 2,
            TopTvShows = 3,
            TopRatedTvShows = 4,
            TopMovieUpComming = 5,
            CollectionTop1 = 6,
            TopAnime = 7

        }

        public async void GetCatalog(TypeCatalog type)
        {
            switch (type)
            {
                case TypeCatalog.Carousel:

                    await Init();
                    ListaCarousel = ListaCarouselReload;

                    break;
                case TypeCatalog.TopMovies:
                    await InitTopMoviePopular();
                    ListaTopMovies = ListaTopMoviesReload;
                    break;
                case TypeCatalog.TopMovieUpComming:

                    await InitTopMovieUpComming();
                    ListaTopMoviesUpComming = ListaTopMoviesUpCommingReload;

                    break;
                case TypeCatalog.TopTvShows: //Netflix

                    //await initSearchTVTop();
                    //ListaTopTvShows = ListaTopTvShowsReload;

                    break;
                case TypeCatalog.TopRatedTvShows:

                    await initTopRatedTvShows();
                    ListaTopRatedTvShows = ListaTopRatedTvShowsReload;

                    break;
            }
        }
        #region PROCESOS
        public async Task Init()
        {
            ListaCarouselReload = new List<movie>();
            try
            {
                TMDbClient client = new TMDbClient(Preferences.Get("key_api_tmdb", ""));
                //https://api.themoviedb.org/3/trending/movie/week?api_key=6dc0f77903f07ee7e6b2715eccf15683&page=1&language=es
                //CancellationToken cancellationToken = cancellationTokenSource.Token;
                //SearchContainer<SearchMovie> movieTop = await client.GetTrendingMoviesAsync(TMDbLib.Objects.Trending.TimeWindow.Week, 0, cancellationToken);
                HttpClient httpClient = new();
                var movies = await httpClient.GetFromJsonAsync<mCatalogTrendingMovie>("https://api.themoviedb.org/3/trending/movie/week?api_key=" + Preferences.Get("key_api_tmdb", "") + "&page=1&language=es");
                foreach (var movie in movies.results)
                {
                    string date = "Año no disponible";
                    if (movie.release_date != null)
                    {
                        DateTime releaseDate = Convert.ToDateTime(movie.release_date);
                        DateTime dates = releaseDate;
                        date = dates.ToString("yyyy");
                    }

                    string BackdropPath = "No found BackdropPath";
                    if (!string.IsNullOrEmpty(movie.backdrop_path))
                    {
                        BackdropPath = movie.backdrop_path;
                    }
                    string PosterPath = "/kqjL17yufvn9OVLyXYpvtyrFfak.jpg";
                    if (!string.IsNullOrEmpty(movie.poster_path))
                    {
                        PosterPath = movie.poster_path;
                    }

                    if (ListaCarouselReload.Count < 10)
                        ListaCarouselReload.Add(new movie()
                        {
                            title = movie.title,
                            imagePoster = LoadImagePoster(movie.poster_path),
                            BackdropPath = LoadImageBack(movie.backdrop_path),
                        });

                }
            }

            catch (Exception ex)
            {

            }
        }

        public async Task InitTopMoviePopular()
        {
            ListaTopMoviesReload = new List<movie>();
            try
            {
                TMDbClient client = new TMDbClient(Preferences.Get("key_api_tmdb", ""));
                SearchContainer<SearchMovie> movieTop = await client.GetMoviePopularListAsync(Preferences.Get("language_api_tmdb", ""), 0, Preferences.Get("region_api_tmdb", "US"));
                foreach (SearchMovie result in movieTop.Results)
                {
                    string ever = "No tenemos un resumen disponible.";
                    if (!string.IsNullOrEmpty(result.Overview))
                    {
                        ever = result.Overview;
                    }
                    ListaTopMoviesReload.Add(new movie
                    {
                        title = result.Title,
                        description = ever,
                        imagePoster = LoadImagePoster(result.PosterPath),
                        BackdropPath = LoadImageBack(result.BackdropPath),
                    });
                }
            }

            catch
            {

            }
            finally
            {
            }
        }

        public async Task InitTopMovieNowPlaying()
        {

            ListaTopMoviesNowPlayingReload = new List<movie>();
            try
            {
                TMDbClient client = new TMDbClient(Preferences.Get("key_api_tmdb", ""));
                SearchContainer<SearchMovie> movieTop = await client.GetMovieNowPlayingListAsync(Preferences.Get("language_api_tmdb", ""), 0, Preferences.Get("region_api_tmdb", "US"));
                foreach (SearchMovie result in movieTop.Results)
                {

                    ListaTopMoviesNowPlayingReload.Add(new movie()
                    {
                        title = result.Title,

                        imagePoster = LoadImagePoster(result.PosterPath),

                        BackdropPath = LoadImageBack(result.BackdropPath),
                    });


                }
            }
            catch
            {
            }
        }

        public async Task InitTopMovieUpComming()
        {
            ListaTopMoviesUpCommingReload = new List<movie>();
            try
            {
                TMDbClient client = new TMDbClient(Preferences.Get("key_api_tmdb", ""));
                SearchContainer<SearchMovie> movieTop = await client.GetMovieUpcomingListAsync(Preferences.Get("language_api_tmdb", ""), 0, Preferences.Get("region_api_tmdb", "US"));
                foreach (SearchMovie result in movieTop.Results)
                {
                    ListaTopMoviesUpCommingReload.Add(new movie()
                    {
                        title = result.Title,
                        imagePoster = LoadImagePoster(result.PosterPath),
                        BackdropPath = LoadImageBack(result.BackdropPath),

                    });
                }
            }
            catch
            {
            }
        }

        public async Task initTopRatedTvShows()
        {
            ListaTopRatedTvShowsReload = new List<movie>();

            try
            {
                TMDbClient client = new TMDbClient(Preferences.Get("key_api_tmdb", ""));
                SearchContainer<SearchTv> tv = await client.GetTvShowTopRatedAsync(0, Preferences.Get("language_api_tmdb", ""));

                foreach (SearchTv result in tv.Results)
                {
                    string date = "Año no disponible";
                    //SearchTv tvShow = client.GetTvShowPopularAsync(CurrentPage, ps.language).Result;
                    if (result.FirstAirDate != null)
                    {
                        DateTime releaseDate = (DateTime)result.FirstAirDate;
                        DateTime dates = releaseDate;
                        date = dates.ToString("yyyy");
                    }

                    string ever = "No tenemos un resumen disponible.";
                    if (!string.IsNullOrEmpty(result.Overview))
                    {
                        ever = result.Overview;
                    }
                    string BackdropPath = "No found BackdropPath";
                    if (!string.IsNullOrEmpty(result.BackdropPath))
                    {
                        BackdropPath = result.BackdropPath;
                    }
                    string PosterPath = "/kqjL17yufvn9OVLyXYpvtyrFfak.jpg";
                    if (!string.IsNullOrEmpty(result.PosterPath))
                    {
                        PosterPath = result.PosterPath;
                    }


                    ListaTopRatedTvShowsReload.Add(new movie()
                    {
                        BackdropPath = LoadImageBack(result.BackdropPath),
                        title = result.Name,
                        description = ever,
                        imagePoster = LoadImagePoster(result.PosterPath),
                    });
                }

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("API key is invalid"))
                {
                }
                else if (ex.Message.Contains("Most likely the provided user credentials are invalid"))
                {
                }
            }

        }
        #endregion

        static string imageNoFound = "loading_error";//"image_no_found.svg";
        public static string mainDir = FileSystem.Current.AppDataDirectory;
        public static string pathFinal = Path.Combine(mainDir, "image_cache");



        public static string LoadImagePoster(string url)
        {
            //Debug.WriteLine("imageload:" + url);
            if (!String.IsNullOrEmpty(url))
            {
                if (File.Exists(pathFinal + url + ".jpg"))
                {
                    return pathFinal + url;
                }
                else
                {
                    Uri uri = new Uri(url, UriKind.Relative);

                    bool results = Uri.IsWellFormedUriString(url, UriKind.Absolute);
                    if (!results)
                    {


                        return "https://image.tmdb.org/t/p/w92" + url;

                    }
                    else
                    {
                        return imageNoFound;
                    }
                }

            }

            else
            {
                return imageNoFound;
            }


        }

        public static string LoadImageBack(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return imageNoFound;
            }
            else
            {
                if (File.Exists(pathFinal + url + ".jpg"))
                {
                    return pathFinal + url;
                }
                else
                {

                    return "https://image.tmdb.org/t/p/w300" + url;

                }

            }
        }
    }


}
