namespace FSharp.Data.Fred
/// <namespacedoc>
///   <summary>
///   Functionality related to access FRED database.  
///   Short for Federal Reserve Economic Data, FRED is an online database consisting 
///   of hundreds of thousands of economic data time series from scores of national, 
///   international, public, and private sources. FRED Website: https://fred.stlouisfed.org/.
///   </summary>
/// </namespacedoc>


open System
open FSharp.Data

module Json =
    type KeyFile = JsonProvider<EmbeddedResources.KeyFileSample>
    type SearchResponse = JsonProvider<EmbeddedResources.SearchResponseSample>
    type SeriesResponse = JsonProvider<EmbeddedResources.SeriesSample>
    type SeriesObservationsResponse = JsonProvider<EmbeddedResources.SeriesObservationsSample>
    type SeriesCategoriesResponse = JsonProvider<EmbeddedResources.SeriesCategoriesSample>
    type SeriesReleaseResponse = JsonProvider<EmbeddedResources.SeriesReleaseSample>
    type SeriesTagsResponse = JsonProvider<EmbeddedResources.SeriesTagsSample>

///   <summary>
///   Represents the type of search to perform.
///   </summary>
///   <category index="1">Unions</category>
type SearchType = 
    /// Searches series attributes title, units, frequency, and tags by parsing words into stems. 
    /// This makes it possible for searches like 'Industry' to match series containing related words such as 'Industries'. 
    /// Of course, you can search for multiple words like 'money' and 'stock'. Remember to url encode spaces (e.g. 'money%20stock').
    | FullText 
    /// Performs a substring search on series IDs. Searching for 'ex' will find series containing 'ex' anywhere in a series ID.
    /// '*' can be used to anchor searches and match 0 or more of any character. Searching for 'ex*' will find series containing 'ex' at the beginning of a series ID.
    /// Searching for '*ex' will find series containing 'ex' at the end of a series ID. It's also possible to put an '*' in the middle of a string.
    /// 'm*sl' finds any series starting with 'm' and ending with 'sl'.
    | SeriesId

///   <summary>
///    Order results by values of the specified attribute. 
///   </summary>
///   <category index="2">Unions</category>
[<RequireQualifiedAccessAttribute>]
type SearchOrder = 
    /// Order search by rank
    | SearchRank
    /// Order search by id numbering order
    | SeriesIdOrder
    /// Order search by title 
    | Title
    /// Order search by units
    | Units
    /// Order search by frequency
    | Frequency
    /// Order search by seasonal adjustment
    | SeasonalAdjustment 
    /// Order search by most recent real time start
    | RealTimeStart 
    /// Order search by most recent real time end
    | RealTimeEnd
    /// Order search by most recent updated series
    | LastUpdated
    /// Order search by most recent observation start date
    | ObservationStart
    /// Order search by most recent observation end date 
    | ObservationEnd
    /// Order search by most popular
    | Popularity
    /// Order search by most popular groups
    | GroupPopularity

///   <summary>
///    Sort results is ascending or descending observation date order.
///   </summary>
///   <category index="3">Unions</category>
[<RequireQualifiedAccessAttribute>]
type SortOrder =
    /// Sort by ascending date order
    | Ascending
    /// Sort by descending date order
    | Descending

///   <summary>
///   A key that indicates a data value transformation.
///   </summary>
///   <category index="4">Unions</category>
[<RequireQualifiedAccessAttribute>]
type Units =
    /// Observation(t), default.
    | Levels
    /// Observation(t) - Observation(t-1)
    | Change
    /// Observation(t) - Observation(t-#ObsPerYear)
    | ChangeFromYearAgo
    /// ((Observation(t)/Observation(t-1)) - 1) * 100
    | PercentChange
    /// ((Observation(t)/Observation(t-#ObsPerYear)) - 1) * 100
    | PercentChangeFromYearAgo
    /// (((Observation(t)/Observation(t-1)) ** (#ObsPerYear)) - 1) * 100
    | CompoundedAnnualRateofChange
    /// (ln(Observation(t)) - ln(Observation(t-1))) * 100
    | ContinuouslyCompoundedRateofChange
    /// ((ln(Observation(t)) - ln(Observation(t-1))) * 100) * #ObsPerYear
    | ContinuouslyCompoundedAnnualRateofChange
    /// ln(Observation(t))
    | NaturalLog

///   <summary>
///   The FRED frequency aggregation feature converts higher frequency data series into lower frequency 
///   data series (e.g. converts a monthly data series into an annual data series). 
///   In FRED, the highest frequency data is daily, and the lowest frequency data is annual.  
/// </summary>
///   <category index="5">Unions</category>
[<RequireQualifiedAccessAttribute>]
type Frequency =
    /// Daily frequency. 
    | Daily
    /// Weekly frequency. 
    | Weekly
    /// Biweekly frequency. 
    | Biweekly
    /// Monthly frequency. 
    | Monthly
    /// Quarterly frequency. 
    | Quarterly
    /// Semiannual frequency. 
    | Semiannual
    /// Annual frequency. 
    | Annual
    /// WeeklyEndingFriday frequency. 
    | WeeklyEndingFriday
    /// WeeklyEndingThursday frequency. 
    | WeeklyEndingThursday
    /// WeeklyEndingWednesday frequency. 
    | WeeklyEndingWednesday
    /// WeeklyEndingTuesday frequency. 
    | WeeklyEndingTuesday
    /// WeeklyEndingMonday frequency. 
    | WeeklyEndingMonday
    /// WeeklyEndingSunday frequency. 
    | WeeklyEndingSunday
    /// WeeklyEndingSaturday frequency. 
    | WeeklyEndingSaturday
    /// BiweeklyEndingWednesday frequency. 
    | BiweeklyEndingWednesday
    /// BiweeklyEndingMonday frequency. 
    | BiweeklyEndingMonday
    /// Default frequency. 
    | Default

///   <summary>
///   A key that indicates the aggregation method used for frequency aggregation. 
///   This parameter has no affect if the frequency parameter is not set.   
///   </summary>
///   <category index="6">Unions</category>
[<RequireQualifiedAccessAttribute>]
type AggMethod =
    /// Computes the average to get the observation values for the desired frequency.
    | Average
    /// Computes the sum to get the observation values for the desired frequency.
    | Sum
    /// Set the end of period value to get the observation values for the desired frequency.
    | EndOfPeriod

///   <summary>
///   Order Tags by values of the specified attribute.
///   </summary>
///   <category index="8">Unions</category>
[<RequireQualifiedAccessAttribute>]
type OrderByTags =
    ///Order By SeriesCount
    | SeriesCount
    ///Order By PopularityTags
    | PopularityTags
    ///Order By Created date
    | Created
    ///Order By Name
    | Name
    ///Order By GroupId
    | GroupId

type internal Endpoint =
    | SeriesSearch
    | SeriesInfo
    | SeriesObservations
    | SeriesCategories
    | SeriesRelease
    | SeriesTags

module internal Helpers =
    let endpointToString endpoint =
        match endpoint with
        | SeriesSearch -> "series/search"
        | SeriesInfo -> "series"
        | SeriesObservations -> "series/observations"
        | SeriesCategories -> "series/categories"
        | SeriesRelease -> "series/release"
        | SeriesTags -> "series/tags"

    let endpointSample endpoint =
        match endpoint with
        | SeriesSearch -> EmbeddedResources.SearchResponseSample
        | SeriesInfo -> EmbeddedResources.SeriesSample
        | SeriesObservations -> EmbeddedResources.SeriesObservationsSample
        | SeriesCategories -> EmbeddedResources.SeriesCategoriesSample
        | SeriesRelease -> EmbeddedResources.SeriesReleaseSample
        | SeriesTags -> EmbeddedResources.SeriesTagsSample

    let request key endpoint query =
        match key with
        | "developer" -> endpoint |> endpointSample
        | _ ->
            Http.RequestString($"https://api.stlouisfed.org/fred/{endpointToString endpoint}?",
                       query = query @ [ "api_key", key; "file_type", "json"], 
                       headers = [ HttpRequestHeaders.UserAgent "FSharp.Data.Fred" 
                                   HttpRequestHeaders.Accept HttpContentTypes.Json ])


type Search(key:string,searchText:string,?searchType:SearchType,?realtimeStart:DateTime,?realtimeEnd:DateTime,?limit:int,?orderBy:SearchOrder,?sortOrder:SortOrder)=
    let searchType = 
        match defaultArg searchType SearchType.FullText with
        | FullText -> "full_text"
        | SeriesId -> "series_id"
    let limit = defaultArg limit 20
    let searchText = System.Uri.EscapeUriString(searchText)
    let realtimeStart = 
        let dt = defaultArg realtimeStart DateTime.Now
        dt.ToString("yyyy-MM-dd")
    let realtimeEnd =
        let dt = defaultArg realtimeEnd DateTime.Now
        dt.ToString("yyyy-MM-dd")
    let orderBy = 
        if searchType = "series_id" then 
            match defaultArg orderBy SearchOrder.SeriesIdOrder with
            | SearchOrder.SearchRank -> "search_rank"
            | SearchOrder.SeriesIdOrder -> "series_id"
            | SearchOrder.Title -> "title"
            | SearchOrder.Units -> "units"
            | SearchOrder.Frequency -> "frequency"
            | SearchOrder.SeasonalAdjustment -> "seasonal_adjustment"
            | SearchOrder.RealTimeStart -> "realtime_start"
            | SearchOrder.RealTimeEnd -> "realtime_end"
            | SearchOrder.LastUpdated -> "last_updated"
            | SearchOrder.ObservationStart -> "observation_start"
            | SearchOrder.ObservationEnd -> "observation_end"
            | SearchOrder.Popularity -> "popularity"
            | SearchOrder.GroupPopularity -> "group_popularity"
        else 
            match defaultArg orderBy SearchOrder.SearchRank with
            | SearchOrder.SearchRank -> "search_rank"
            | SearchOrder.SeriesIdOrder -> "series_id"
            | SearchOrder.Title -> "title"
            | SearchOrder.Units -> "units"
            | SearchOrder.Frequency -> "frequency"
            | SearchOrder.SeasonalAdjustment -> "seasonal_adjustment"
            | SearchOrder.RealTimeStart -> "realtime_start"
            | SearchOrder.RealTimeEnd -> "realtime_end"
            | SearchOrder.LastUpdated -> "last_updated"
            | SearchOrder.ObservationStart -> "observation_start"
            | SearchOrder.ObservationEnd -> "observation_end"
            | SearchOrder.Popularity -> "popularity"
            | SearchOrder.GroupPopularity -> "group_popularity"
    let sortOrder =
        if orderBy = "search_rank" || orderBy = "popularity" then
            match defaultArg sortOrder SortOrder.Descending with
            | SortOrder.Ascending -> "asc"
            | SortOrder.Descending -> "desc"
        else 
            match defaultArg sortOrder SortOrder.Ascending with
            | SortOrder.Ascending -> "asc"
            | SortOrder.Descending -> "desc"
    let searchRequest = 
        let queryParameters = [
            "search_text", searchText
            "search_type", searchType 
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
            "limit", string limit
            "order_by", orderBy
            "sort_order", sortOrder
        ]
        Helpers.request key Endpoint.SeriesSearch queryParameters
        |> Json.SearchResponse.Parse
    member this.Results = searchRequest
    member this.Summary(?n:int) =
        let shortDate (dt: System.DateTime) = dt.ToString("yyyy-MM-dd")
        // Currently if given n > limit then the returns search summary
        // will be less than n. Probably should print something for the user
        // so that they know why theyre getting less than n results.
        let n = min searchRequest.Seriess.Length (defaultArg n  10)
        printfn $"Count of search hits: {this.Results.Count}"
        printfn $"Top {n} results:"
        let limitedResults = searchRequest.Seriess |> Seq.truncate n |> Seq.mapi (fun i x -> i+1, x)
        for i, series in limitedResults do
            let sd = shortDate series.ObservationStart
            let ed = shortDate series.ObservationEnd
            let freq = series.Frequency
            printfn $"""%3i{i}. {series.Title} """
            printfn $"         Id: %-10s{series.Id} Period: {sd} to {ed}  Freq: {freq} \n" 


and Series(key:string) =
    member this.Info(id:string) = 
        Helpers.request key Endpoint.SeriesInfo [ "series_id", id.ToUpper() ]
        |> Json.SeriesResponse.Parse

    member this.Observations(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime,?limit:int,?sortOrder:SortOrder,?observationStart:DateTime,?observationEnd:DateTime,?units:Units,?frequency:Frequency,?aggMethod:AggMethod) =
        let realtimeStart = 
            let dt = defaultArg realtimeStart DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd =
            let dt = defaultArg realtimeEnd DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let sortOrder = 
            match defaultArg sortOrder SortOrder.Ascending with
            | SortOrder.Ascending -> "asc"
            | SortOrder.Descending -> "desc"
        let observationStart =
            let dt = defaultArg observationStart (DateTime(1776, 07, 04))
            dt.ToString("yyyy-MM-dd")
        let observationEnd =
            let dt = defaultArg observationEnd (DateTime(9999, 12, 31))
            dt.ToString("yyyy-MM-dd")
        let units =
            match defaultArg units Units.Levels with
            | Units.Levels -> "lin"
            | Units.Change -> "chg"
            | Units.ChangeFromYearAgo -> "ch1"
            | Units.PercentChange -> "pch"
            | Units.PercentChangeFromYearAgo -> "pc1"
            | Units.CompoundedAnnualRateofChange -> "pca"
            | Units.ContinuouslyCompoundedRateofChange -> "cch"
            | Units.ContinuouslyCompoundedAnnualRateofChange -> "cca"
            | Units.NaturalLog -> "log"
        let frequency =
            match defaultArg frequency Frequency.Default with
            | Frequency.Daily -> "d"
            | Frequency.Weekly -> "w"
            | Frequency.Biweekly -> "bw"
            | Frequency.Monthly -> "m"
            | Frequency.Quarterly -> "q"
            | Frequency.Semiannual -> "sa"
            | Frequency.Annual -> "a"
            | Frequency.WeeklyEndingFriday -> "wef"
            | Frequency.WeeklyEndingThursday -> "weth"
            | Frequency.WeeklyEndingWednesday -> "wew"
            | Frequency.WeeklyEndingTuesday -> "wetu"
            | Frequency.WeeklyEndingMonday -> "wem"
            | Frequency.WeeklyEndingSunday -> "wesu"
            | Frequency.WeeklyEndingSaturday -> "wesa"
            | Frequency.BiweeklyEndingWednesday -> "bwew"
            | Frequency.BiweeklyEndingMonday -> "bwem"
            | Frequency.Default -> ""
        let aggMethod =
            match defaultArg aggMethod AggMethod.Average with
            | AggMethod.Average -> "avg"
            | AggMethod.Sum -> "sum"
            | AggMethod.EndOfPeriod -> "eop"
        let queryParameters = [
            "series_id", id.ToUpper()
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
            "limit", string limit
            "sort_order", sortOrder
            "observation_start", observationStart
            "observation_end", observationEnd
            "units", units
            "frequency", frequency
            "aggregation_method", aggMethod
        ]
        Helpers.request key Endpoint.SeriesObservations queryParameters
        |> Json.SeriesObservationsResponse.Parse
                
    member this.Search(searchText:string,?searchType:SearchType,?realtimeStart:DateTime,?realtimeEnd:DateTime,?limit:int,?orderBy:SearchOrder,?sortOrder:SortOrder) = 
        Search(key, searchText=searchText, ?searchType=searchType, ?realtimeStart=realtimeStart, ?realtimeEnd=realtimeEnd, ?limit=limit, ?orderBy=orderBy, ?sortOrder=sortOrder)

    member this.Categories(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime) =
        let realtimeStart = 
            let dt = defaultArg realtimeStart DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd =
            let dt = defaultArg realtimeEnd DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let queryParameters = [
            "series_id", id.ToUpper()
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
        ]

        Helpers.request key Endpoint.SeriesCategories queryParameters
        |> Json.SeriesCategoriesResponse.Parse
    member this.Release(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime) =
        let realtimeStart =
            let dt = defaultArg realtimeStart DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd = 
            let dt = defaultArg realtimeEnd DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let queryParameters = [
            "series_id", id.ToUpper()
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
        ]
            
        Helpers.request key Endpoint.SeriesRelease queryParameters
        |> Json.SeriesReleaseResponse.Parse 


    member this.Tags(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime,?orderBy:OrderByTags,?sortOrder:SortOrder) =
        let realtimeStart =
            let dt = defaultArg realtimeStart DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd = 
            let dt = defaultArg realtimeEnd DateTime.Now
            dt.ToString("yyyy-MM-dd")
        let orderBy = 
            match defaultArg orderBy OrderByTags.SeriesCount with
            | OrderByTags.SeriesCount -> "series_count"
            | OrderByTags.PopularityTags -> "popularity"
            | OrderByTags.Created -> "created"
            | OrderByTags.Name -> "name"
            | OrderByTags.GroupId -> "group_id"
        let sortOrder = 
            match defaultArg sortOrder SortOrder.Ascending with
            | SortOrder.Ascending -> "asc"
            | SortOrder.Descending -> "desc"
        let queryParameters = [
            "series_id", id.ToUpper()
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
            "order_by", orderBy
            "sort_order", sortOrder
        ]

        Helpers.request key Endpoint.SeriesTags queryParameters
        |> Json.SeriesTagsResponse.Parse 

and Fred(key:string) = 
    /// <summary>
    /// Loads the API key from a json file.
    /// </summary>
    /// <param name="keyFile">The path of the json file containing the key.</param>
    /// <returns>Returns a value with the API key, "developer" if no api key is found.</returns>
    static member loadKey(keyFile) =
        let envVars = System.Environment.GetEnvironmentVariables()
        let var = "FRED_KEY"
        if envVars.Contains var then 
            envVars.[var] :?> string
        elif IO.File.Exists(keyFile) then 
            Json.KeyFile.Load(keyFile).FredKey
        else "developer"
    ///   <summary>
    ///   Access Type Series constructors, which are the following: 
    ///   <list>
    ///   Series.Search - Get economic data series that match search text.  
    ///   <code lang="fsharp">myFred.Series.Search("Text for Search Here").Summary()</code>
    ///   </list>
    ///   <list>Series.Info - Get the information for an economic data series.
    ///   <code lang="fsharp">myFred.Series.Info "Economic Data Series Here"</code>
    ///   </list>
    ///   <list>Series.Categories - Get the categories for an economic data series.
    ///   <code lang="fsharp">myFred.Series.Categories "Economic Data Series Here"</code>
    ///   </list>
    ///   <list>Series.Release - Get the release for an economic data series.
    ///   <code lang="fsharp">myFred.Series.Release "Economic Data Series Here"</code>
    ///   </list>
    ///   <list>Series.Tags - Get the tags for an economic data series.
    ///   <code lang="fsharp">myFred.Series.Tags "Economic Data Series Here"</code>
    ///   </list>
    ///   <list>Series.Observations - Get the observations or data values for an economic data series.
    ///   <code lang="fsharp">myFred.Series.Observations "Economic Data Series Here"</code>
    ///   </list>
    ///   </summary>
    member this.Series = Series(key)
    /// Access API key
    member this.Key = key


