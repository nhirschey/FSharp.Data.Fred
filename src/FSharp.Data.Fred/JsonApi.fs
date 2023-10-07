namespace FSharp.Data.Fred

open System
open FSharp.Data

module internal Json =

    type KeyFile = JsonProvider<EmbeddedResources.KeyFileSample>
    type SearchResponse = JsonProvider<EmbeddedResources.SearchResponseSample,RootName="SearchResponse">
    type SeriesResponse = JsonProvider<EmbeddedResources.SeriesSample,RootName="InfoResponse">
    type SeriesObservationsResponse = JsonProvider<EmbeddedResources.SeriesObservationsSample,RootName="ObservationsResponse">
    type SeriesCategoriesResponse = JsonProvider<EmbeddedResources.SeriesCategoriesSample,RootName="CategoriesResponse">
    type SeriesReleaseResponse = JsonProvider<EmbeddedResources.SeriesReleaseSample,RootName="ReleaseResponse">
    type SeriesTagsResponse = JsonProvider<EmbeddedResources.SeriesTagsSample,RootName="TagsResponse">

/// <namespacedoc>
///   <summary>
///   Functionality related to accessing the FRED database.  
///   Short for Federal Reserve Economic Data, FRED is an online database consisting 
///   of hundreds of thousands of economic data time series from scores of national, 
///   international, public, and private sources. See <a href="https://fred.stlouisfed.org/">https://fred.stlouisfed.org/</a>.
///   </summary>
/// </namespacedoc>
/// 
/// <summary> Contains types for specifying search query parameters. </summary>
[<AutoOpen>]
module QueryParameters =
    ///   <summary>
    ///   Represents the type of search to perform.
    ///   </summary>
    ///   <category index="1">Unions</category>
    type SearchType = 
        /// <summary>
        /// Searches series attributes title, units, frequency, and tags by parsing words into stems.
        /// This makes it possible for searches like 'Industry' to match series containing related words such as 'Industries'. 
        /// Of course, you can search for multiple words like 'money' and 'stock'. Remember to url encode spaces (e.g. <c>money%20stock</c>).
        /// </summary>
        | FullText
        /// <summary>
        /// Performs a substring search on series IDs. Searching for 'ex' will find series containing 'ex' anywhere in a series ID.
        /// '*' can be used to anchor searches and match 0 or more of any character. Searching for 'ex*' will find series containing 'ex' at the beginning of a series ID.
        /// Searching for '*ex' will find series containing 'ex' at the end of a series ID. It's also possible to put an '*' in the middle of a string.
        /// 'm*sl' finds any series starting with 'm' and ending with 'sl'. 
        /// </summary>
        | SeriesId

    ///   <summary>
    ///    Order results by values of the specified attribute. 
    ///   </summary>
    ///   <category index="2">Unions</category>
    [<RequireQualifiedAccess>]
    type SearchOrder =
        /// Order search by rank. 
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
        /// Order search by most recently updated series
        | LastUpdated
        /// Order search by most recent observation start date
        | ObservationStart
        /// Order search by most recent observation end date
        | ObservationEnd
        /// Order search by popularity
        | Popularity
        /// Order search by group popularity
        | GroupPopularity

    ///   <summary>
    ///    Sort results is ascending or descending observation date order.
    ///   </summary>
    ///   <category index="3">Unions</category>
    [<RequireQualifiedAccessAttribute>]
    type SortOrder =
        /// Sort results in ascending order.
        | Ascending
        /// Sort results in descending order.
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
            | Daily
            | Weekly
            | Biweekly
            | Monthly
            | Quarterly
            | Semiannual
            | Annual
            | WeeklyEndingFriday
            | WeeklyEndingThursday
            | WeeklyEndingWednesday
            | WeeklyEndingTuesday
            | WeeklyEndingMonday
            | WeeklyEndingSunday
            | WeeklyEndingSaturday
            | BiweeklyEndingWednesday
            | BiweeklyEndingMonday
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
        /// Order by number of series per tag.
        | SeriesCount
        /// Order by tag popularity
        | Popularity
        /// Order by created date
        | Created
        /// Order by tag name
        | Name
        /// Order by group id
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
    
    let cstNow =
        let utcNow = DateTime.UtcNow
        let cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
        TimeZoneInfo.ConvertTimeFromUtc(utcNow, cstZone)

type Search(key:string,searchText:string,?searchType:SearchType,?realtimeStart:DateTime,?realtimeEnd:DateTime,?limit:int,?orderBy:SearchOrder,?sortOrder:SortOrder)=
    let searchType = 
        match defaultArg searchType SearchType.FullText with
        | FullText -> "full_text"
        | SeriesId -> "series_id"
    let limit = defaultArg limit 20
    let searchText = System.Uri.EscapeUriString(searchText)
    let realtimeStart = 
        let dt = defaultArg realtimeStart Helpers.cstNow
        dt.ToString("yyyy-MM-dd")
    let realtimeEnd =
        let dt = defaultArg realtimeEnd Helpers.cstNow
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


    /// <summary> A data series.</summary>
and Series(key:string) =
    /// <summary> Get the information for an economic data series.</summary>
    /// <param name="id">The id for a series. String, required.</param>
    /// <returns>The information for an economic data series.</returns>
    member this.Info(id:string) = 
        Helpers.request key Endpoint.SeriesInfo [ "series_id", id.ToUpper() ]
        |> Json.SeriesResponse.Parse

    /// <summary> Get the observations or data values for an economic data series.</summary>
    /// <param name="id">The id for a series. String, required.</param>
    /// <param name="observationStart">
    /// The start of the observation period.
    /// Optional, default: <c>DateTime(1776,07,04)</c> (earliest available).
    /// </param>
    /// <param name="observationEnd">
    /// The end of the observation period.
    /// Optional, default: <c>DateTime(9999,12,31)</c> (latest available).
    /// </param>
    /// <param name="limit">
    /// The maximum number of results to return.
    /// integer between `1` and `1000`, optional, default: `1000`.
    /// </param>
    /// <param name="sortOrder">
    /// Sort results is ascending or descending observation date order.  
    /// optional, default: asc.
    /// </param>
    /// <param name="units">
    /// A key that indicates a data value transformation. 
    /// Optional, default: <see cref="T:FSharp.Data.Fred.QueryParameters.Units.Levels" /> (No transformation).
    /// See unit transformation formulas <a href="https://alfred.stlouisfed.org/help#growth_formulas">here</a>.
    /// </param>
    /// <param name="frequency">
    /// An optional parameter that indicates a lower frequency to aggregate values to. 
    /// The FRED frequency aggregation feature converts higher frequency data series into lower frequency 
    /// data series (e.g. converts a monthly data series into an annual data series). 
    /// In FRED, the highest frequency data is daily, and the lowest frequency data is annual. 
    /// There are 3 aggregation methods available- average, sum, and end of period. 
    /// See the aggregation_method parameter <a href="https://research.stlouisfed.org/docs/api/fred/series_observations.html#aggregation_method">here</a>.
    /// </param>
    /// <param name="realtimeStart">
    /// The start of the real-time period, "when facts were true or when information was known until it changed." For more information, see <a href="https://research.stlouisfed.org/docs/api/fred/realtime_period.html">Real-time periods</a>.
    /// <c>DateTime(yyyy,MM,dd)</c> formatted <c>DateTime</c>, optional, default: today's date.
    /// </param>
    /// <param name="realtimeEnd">
    /// The start of the real-time period, "when facts were true or when information was known until it changed." For more information, see <a href="https://research.stlouisfed.org/docs/api/fred/realtime_period.html">Real-time periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <param name="aggMethod">
    /// A key that indicates the aggregation method used for frequency aggregation. 
    /// This parameter has no affect if the <a href="https://fred.stlouisfed.org/docs/api/fred/series_observations.html#frequency">frequency parameter</a> is not set.
    /// Optional, default: AggMethod.Average.
    /// </param>
    /// <returns>Observations or data values for an economic data series.</returns>    
    member this.Observations(id:string,?observationStart:DateTime,?observationEnd:DateTime,?limit:int,?sortOrder:SortOrder,?units:Units,?frequency:Frequency,?aggMethod:AggMethod,?realtimeStart:DateTime,?realtimeEnd:DateTime) =
        let realtimeStart = 
            let dt = defaultArg realtimeStart Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd =
            let dt = defaultArg realtimeEnd Helpers.cstNow
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

    /// <summary> Get economic data series that match search text.</summary>
    /// <param name="searchText">The words to match against economic data series.</param>
    /// <param name="searchType">
    /// Determines the type of search to perform. Default is <see cref="T:FSharp.Data.Fred.QueryParameters.SearchType.FullText"/>
    /// </param>
    /// <param name="realtimeStart">
    /// The start of the real-time period. For more information, see <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <param name="realtimeEnd">
    /// The start of the real-time period. For more information, see <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <param name="limit">
    /// The maximum number of results to return.
    /// integer between `1` and `1000`, optional, default: `1000`.
    /// </param>
    /// <param name="orderBy">
    /// Order results by values of the specified attribute.
    /// Optional, default: If the search type is <see cref="T:FSharp.Data.Fred.QueryParameters.SearchType.FullText"/> 
    /// then the default order is <see cref="T:FSharp.Data.Fred.QueryParameters.SearchOrder.SearchRank"/>.
    /// If the value of search type is <see cref="T:FSharp.Data.Fred.QueryParameters.SearchOrder.SeriesIdOrder"/>.
    /// </param>
    /// <param name="sortOrder">
    /// Sort results is ascending or descending order for attribute values.  
    /// Optional, default: If order by is 
    /// <see cref="T:FSharp.Data.Fred.QueryParameters.SearchOrder.SearchRank"/> or 
    /// <see cref="T:FSharp.Data.Fred.QueryParameters.SearchOrder.Popularity"/>
    /// then the default descending, otherwise the default is ascending.
    /// </param>
    /// <returns>A collection of economic data series that match the parameters.</returns>                
    member this.Search(searchText:string,?searchType:SearchType,?realtimeStart:DateTime,?realtimeEnd:DateTime,?limit:int,?orderBy:SearchOrder,?sortOrder:SortOrder) = 
        Search(key, searchText=searchText, ?searchType=searchType, ?realtimeStart=realtimeStart, ?realtimeEnd=realtimeEnd, ?limit=limit, ?orderBy=orderBy, ?sortOrder=sortOrder)

    /// <summary> Get the categories for an economic data series.</summary>
    /// <param name="id">The id for a series. String, required.</param>
    /// <param name="realtimeStart">
    /// The start of the real-time period. For more information, see <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
    /// </param>
    /// <param name="realtimeEnd">
    /// The start of the real-time period. For more information, see 
    /// <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <returns>A collection of the series categories.</returns>
    member this.Categories(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime) =
        let realtimeStart = 
            let dt = defaultArg realtimeStart Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd =
            let dt = defaultArg realtimeEnd Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let queryParameters = [
            "series_id", id.ToUpper()
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
        ]

        Helpers.request key Endpoint.SeriesCategories queryParameters
        |> Json.SeriesCategoriesResponse.Parse

    /// <summary> Get the release for an economic data series.</summary>
    /// <param name="id">The id for a series. String, required.</param>
    /// <param name="realtimeStart">
    /// The start of the real-time period. For more information, see 
    /// <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <param name="realtimeEnd">
    /// The start of the real-time period. For more information, see 
    /// <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <returns>A collection of fields describing the series release.</returns>
    member this.Release(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime) =
        let realtimeStart =
            let dt = defaultArg realtimeStart Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd = 
            let dt = defaultArg realtimeEnd Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let queryParameters = [
            "series_id", id.ToUpper()
            "realtime_start", realtimeStart
            "realtime_end", realtimeEnd
        ]
            
        Helpers.request key Endpoint.SeriesRelease queryParameters
        |> Json.SeriesReleaseResponse.Parse 

    /// <summary> Get the FRED tags for a series.</summary>
    /// <param name="id">The id for a series. String, required.</param>
    /// <param name="realtimeStart">
    /// The start of the real-time period. For more information, see
    /// <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <param name="realtimeEnd">
    /// The end of the real-time period. For more information, see 
    /// <a href="https://fred.stlouisfed.org/docs/api/fred/realtime_period.html">Real-Time Periods</a>.
    /// Optional, default: today's date.
    /// </param>
    /// <param name="orderBy">
    /// Order results by values of the specified attribute. Optional, default:
    /// <see cref="T:FSharp.Data.Fred.QueryParameters.OrderByTags.SeriesCount"/>. 
    /// </param>
    /// <param name="sortOrder">
    /// Sort results is ascending or descending order.  
    /// Optional, default is ascending.
    /// </param>
    /// <returns></returns>
    member this.Tags(id:string,?realtimeStart:DateTime,?realtimeEnd:DateTime,?orderBy:OrderByTags,?sortOrder:SortOrder) =
        let realtimeStart =
            let dt = defaultArg realtimeStart Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let realtimeEnd = 
            let dt = defaultArg realtimeEnd Helpers.cstNow
            dt.ToString("yyyy-MM-dd")
        let orderBy = 
            match defaultArg orderBy OrderByTags.SeriesCount with
            | OrderByTags.SeriesCount -> "series_count"
            | OrderByTags.Popularity -> "popularity"
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

///  <summary>
///   Represents a type for accessing the Fred API.
///   You must provide an api key.
///  </summary>
///  <example>
///  <code lang="fsharp">
///   let myFred = Fred "insert your API key here"
///  </code> 
///   or
///  <code lang="fsharp">
///   let myFred = Fred.loadKey "fredKey.json"
///  </code>
///  </example>
and Fred(key:string) = 
    /// <summary> Loads a Fred api key from a json file.</summary>
    /// The key file should have the format
    /// <code>
    /// { "fredKey": "key here in the actual file that you should name fredKey.json and NOT commit to git"}
    /// </code>
    /// <example>
    /// <code lang="fsharp">
    /// let myFred = Fred.loadKey @"c:\my\fredKey.json"
    /// </code>
    /// </example>
    static member loadKey keyFile =
        let envVars = System.Environment.GetEnvironmentVariables()
        let var = "FRED_KEY"
        if envVars.Contains var then 
            envVars.[var] :?> string
        elif IO.File.Exists(keyFile) then 
            Json.KeyFile.Load(keyFile).FredKey
        else "developer"
    /// <summary> Represents a Fred data series. </summary>                                                                      
    member this.Series = Series(key)
    /// Fred API key
    member this.Key = key


