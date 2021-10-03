[<AutoOpen>]
module FSharp.Data.Fred.JsonApi

open System
open FSharp.Data

[<Literal>]
let KeyFileSample = """ { "fredKey": "key here in the actual file to be named fredKey.json"} """
type KeyFile = JsonProvider<KeyFileSample>

type SearchType = FullText | SeriesId

[<RequireQualifiedAccessAttribute>]
type SearchOrder = 
    | SearchRank
    | SeriesIdOrder
    | Title
    | Units
    | Frequency
    | SeasonalAdjustment
    | RealTimeStart 
    | RealTimeEnd
    | LastUpdated
    | ObservationStart
    | ObservationEnd
    | Popularity
    | GroupPopularity

[<RequireQualifiedAccessAttribute>]
type SortOrder =
    | Ascending
    | Descending

[<RequireQualifiedAccessAttribute>]
type Units =
    | Levels
    | Change
    | ChangeFromYearAgo
    | PercentChange
    | PercentChangeFromYearAgo
    | CompoundedAnnualRateofChange
    | ContinuouslyCompoundedRateofChange
    | ContinuouslyCompoundedAnnualRateofChange
    | NaturalLog

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

[<RequireQualifiedAccessAttribute>]
type AggMethod =
    | Average
    | Sum
    | EndOfPeriod

[<RequireQualifiedAccessAttribute>]
type Observations =
    {
        Date:DateTime
        Value:float
    }

[<RequireQualifiedAccessAttribute>]
type OrderByTags =
    | SeriesCount
    | PopularityTags
    | Created
    | Name
    | GroupId

let [<Literal>] SearchResponseSample = """
{
  "realtime_start": "2017-08-01",
  "realtime_end": "2017-08-01",
  "order_by": "search_rank",
  "sort_order": "desc",
  "count": 32,
  "offset": 0,
  "limit": 1000,
  "seriess": [
    {
      "id": "MSIM2",
      "realtime_start": "2017-08-01",
      "realtime_end": "2017-08-01",
      "title": "Monetary Services Index: M2 (preferred)",
      "observation_start": "1967-01-01",
      "observation_end": "2013-12-01",
      "frequency": "Monthly",
      "frequency_short": "M",
      "units": "Billions of Dollars",
      "units_short": "Bil. of $",
      "seasonal_adjustment": "Seasonally Adjusted",
      "seasonal_adjustment_short": "SA",
      "last_updated": "2014-01-17 07:16:44-06",
      "popularity": 34,
      "group_popularity": 33,
      "notes": "The MSI measure the flow of monetary services received each period by households and firms from their holdings of monetary assets (levels of the indexes are sometimes referred to as Divisia monetary aggregates).\nPreferred benchmark rate equals 100 basis points plus the largest rate in the set of rates.\nAlternative benchmark rate equals the larger of the preferred benchmark rate and the Baa corporate bond yield.\nMore information about the new MSI can be found at\nhttp:\/\/research.stlouisfed.org\/msi\/index.html."
    },
    {
      "id": "MSIM1P",
      "realtime_start": "2017-08-01",
      "realtime_end": "2017-08-01",
      "title": "Monetary Services Index: M1 (preferred)",
      "observation_start": "1967-01-01",
      "observation_end": "2013-12-01",
      "frequency": "Monthly",
      "frequency_short": "M",
      "units": "Billions of Dollars",
      "units_short": "Bil. of $",
      "seasonal_adjustment": "Seasonally Adjusted",
      "seasonal_adjustment_short": "SA",
      "last_updated": "2014-01-17 07:16:45-06",
      "popularity": 26,
      "group_popularity": 26,
      "notes": "The MSI measure the flow of monetary services received each period by households and firms from their holdings of monetary assets (levels of the indexes are sometimes referred to as Divisia monetary aggregates)."
    }]}"""

let [<Literal>] SeriesSample = """
{
    "realtime_start": "2013-08-14",
    "realtime_end": "2013-08-14",
    "seriess": [
        {
            "id": "GNPCA",
            "realtime_start": "2013-08-14",
            "realtime_end": "2013-08-14",
            "title": "Real Gross National Product",
            "observation_start": "1929-01-01",
            "observation_end": "2012-01-01",
            "frequency": "Annual",
            "frequency_short": "A",
            "units": "Billions of Chained 2009 Dollars",
            "units_short": "Bil. of Chn. 2009 $",
            "seasonal_adjustment": "Not Seasonally Adjusted",
            "seasonal_adjustment_short": "NSA",
            "last_updated": "2013-07-31 09:26:16-05",
            "popularity": 39,
            "notes": "BEA Account Code: A001RX1"
        }
    ]
}
"""
let [<Literal>] SeriesObservationsSample = """
{
    "realtime_start": "2013-08-14",
    "realtime_end": "2013-08-14",
    "observation_start": "1776-07-04",
    "observation_end": "9999-12-31",
    "units": "lin",
    "output_type": 1,
    "file_type": "json",
    "order_by": "observation_date",
    "sort_order": "asc",
    "count": 84,
    "offset": 0,
    "limit": 100000,
    "observations": [
        {
            "realtime_start": "2013-08-14",
            "realtime_end": "2013-08-14",
            "date": "1929-01-01",
            "value": "1065.9"
        },
        {
            "realtime_start": "2013-08-14",
            "realtime_end": "2013-08-14",
            "date": "1930-01-01",
            "value": "1e5"
        }
    ]}"""

let [<Literal>] SeriesCategoriesSample = """
{
    "categories": [
        {
            "id": 95,
            "name": "Monthly Rates",
            "parent_id": 15
        },
        {
            "id": 275,
            "name": "Japan",
            "parent_id": 158
        }
    ]
}
"""

let [<Literal>] SeriesReleaseSample= """
{
    "realtime_start": "2013-08-14",
    "realtime_end": "2013-08-14",
    "releases": [
        {
            "id": 21,
            "realtime_start": "2013-08-14",
            "realtime_end": "2013-08-14",
            "name": "H.6 Money Stock Measures",
            "press_release": true,
            "link": "http://www.federalreserve.gov/releases/h6/"
        }
    ]
}
"""

let [<Literal>] SeriesTagsSample = """
{
    "realtime_start": "2013-08-14",
    "realtime_end": "2013-08-14",
    "order_by": "series_count",
    "sort_order": "desc",
    "count": 8,
    "offset": 0,
    "limit": 1000,
    "tags": [
        {
            "name": "nation",
            "group_id": "geot",
            "notes": "Country Level",
            "created": "2012-02-27 10:18:19-06",
            "popularity": 100,
            "series_count": 105200
        },
        {
            "name": "stlfsi",
            "group_id": "rls",
            "notes": "St. Louis Financial Stress Index",
            "created": "2012-08-16 15:21:17-05",
            "popularity": 66,
            "series_count": 1
        }
    ]
}
"""


type SearchResponse = JsonProvider<SearchResponseSample>
type SeriesResponse = JsonProvider<SeriesSample>
type SeriesObservationsResponse = JsonProvider<SeriesObservationsSample>
type SeriesCategoriesResponse = JsonProvider<SeriesCategoriesSample>
type SeriesReleaseResponse = JsonProvider<SeriesReleaseSample>
type SeriesTagsResponse = JsonProvider<SeriesTagsSample>

(**

API interface ideas?

Fred.download
Fred.searchIds
Fred.searchFullText

// maybe more natural for python people if:
fred.download
fred.search

// possible results of fred.download returns
 - Info
 - Observations
*)

module internal Helpers =
    let request key endpoint query =
        Http.RequestString($"https://api.stlouisfed.org/fred/{endpoint}?",
                   query = query @ [ "api_key", key; "file_type", "json"], 
                   headers = [ HttpRequestHeaders.UserAgent "FSharp.Data.Fred" 
                               HttpRequestHeaders.Accept HttpContentTypes.Json ])

module Series =
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
            Helpers.request key "series/search" queryParameters
            |> SearchResponse.Parse
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
        /// <summary>
        /// Get the information for an economic data series.
        /// </summary>
        /// <param name="id">The id for a series. String, required.</param>
        /// <returns>The information for an economic data series.</returns>
        member this.Info(id:string) = 
            Helpers.request key "series" [ "series_id", id.ToUpper() ]
            |> SeriesResponse.Parse

        /// <summary>
        /// Get the observations or data values for an economic data series.
        /// </summary>
        /// <param name="id">The id for a series. String, required.</param>
        /// <param name="realtimeStart">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="realtimeEnd">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="limit">
        /// The maximum number of results to return.
        /// integer between `1` and `1000`, optional, default: `1000`.
        /// </param>
        /// <param name="sortOrder">
        /// Sort results is ascending or descending observation date order.  
        /// optional, default: asc.
        /// </param>
        /// <param name="observationStart">
        /// The start of the observation period.
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: `DateTime(1776,07,04)` (earliest available).
        /// </param>
        /// <param name="observationEnd">
        /// The end of the observation period.
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: `DateTime(9999,12,31)` (latest available).
        /// </param>
        /// <param name="units">
        /// A key that indicates a data value transformation.
        /// One of the following:   
        /// `Units.Levels`
        /// `Units.Change`
        /// `Units.ChangefromYearAgo`
        /// `Units.PercentChange`
        /// `Units.PercentChangefromYearAgo`
        /// `Units.CompoundedAnnualRateofChange`
        /// `Units.ContinuouslyCompoundedRateofChange`
        /// `Units.ContinuouslyCompoundedAnnualRateofChange`
        /// `Units.NaturalLog`.  
        /// Optional, default: `Units.Levels` (No transformation).
        /// [Unit transformation formulas](https://alfred.stlouisfed.org/help#growth_formulas)
        /// </param>
        /// <param name="frequency">
        /// An optional parameter that indicates a lower frequency to aggregate values to. 
        /// The FRED frequency aggregation feature converts higher frequency data series into lower frequency 
        /// data series (e.g. converts a monthly data series into an annual data series). 
        /// In FRED, the highest frequency data is daily, and the lowest frequency data is annual. 
        /// There are 3 aggregation methods available- average, sum, and end of period. 
        /// [See the aggregation_method parameter.](https://fred.stlouisfed.org/docs/api/fred/series_observations.html#aggregation_method)  
        /// One of the following: 
        /// `Frequency.Daily`, `Frequency.Weekly`, `Frequency.Biweekly`, `Frequency.Monthly`, `Frequency.Quarterly`, 
        /// `Frequency.Semiannual`, `Frequency.Annual`, `Frequency.WeeklyEndingFriday`,
        /// `Frequency.WeeklyEndingThursday`, `Frequency.WeeklyEndingWednesday`, `Frequency.WeeklyEndingTuesday`, 
        /// `Frequency.WeeklyEndingMonday`, `Frequency.WeeklyEndingSunday`, `Frequency.WeeklyEndingSaturday`, 
        /// `Frequency.BiweeklyEndingWednesday`, `Frequency.BiweeklyEndingMonday`.
        /// </param>
        /// <param name="aggMethod">
        /// A key that indicates the aggregation method used for frequency aggregation. 
        /// This parameter has no affect if the [frequency parameter](https://fred.stlouisfed.org/docs/api/fred/series_observations.html#frequency) is not set.
        /// Optional, default: AggMethod.Average.
        /// One of the following:
        /// `AggMethod.Average`, `AggMethod.Sum`, `AggMethod.EndOfPeriod`.
        /// </param>
        /// <returns>Observations or data values for an economic data series.</returns>
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
            Helpers.request key "series/observations" queryParameters
            |> SeriesObservationsResponse.Parse
                
        /// <summary>
        /// Get economic data series that match search text.
        /// </summary>
        /// <param name="searchText">The words to match against economic data series.</param>
        /// <param name="searchType">
        /// Determines the type of search to perform.
        /// One of the following: `SearchType.FullText` or `SearchType.SeriesId`.
        /// - `SearchType.FullText` searches series attributes title, units, frequency, and tags by parsing words into stems. 
        /// This makes it possible for searches like 'Industry' to match series containing related words such as 'Industries'. 
        /// Of course, you can search for multiple words like 'money' and 'stock'. Remember to url encode spaces (e.g. `'money%20stock'`).
        /// - `SearchType.SeriesId` performs a substring search on series IDs. Searching for 'ex' will find series containing 'ex' anywhere in a series ID.
        /// '*' can be used to anchor searches and match 0 or more of any character. Searching for 'ex*' will find series containing 'ex' at the beginning of a series ID.
        /// Searching for '*ex' will find series containing 'ex' at the end of a series ID. It's also possible to put an '*' in the middle of a string.
        /// 'm*sl' finds any series starting with 'm' and ending with 'sl'.
        /// Default is `SearchType.FullText`
        /// </param>
        /// <param name="realtimeStart">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="realtimeEnd">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="limit">
        /// The maximum number of results to return.
        /// integer between `1` and `1000`, optional, default: `1000`.
        /// </param>
        /// <param name="orderBy">
        /// Order results by values of the specified attribute.
        /// One of the following:  
        /// `SearchOrder.SearchRank` `SearchOrder.SeriesIdOrder` `SearchOrder.Title` `SearchOrder.Units` `SearchOrder.Frequency` `SearchOrder.SeasonalAdjustment` 
        /// `SearchOrder.RealTimeStart` `SearchOrder.RealTimeEnd` `SearchOrder.LastUpdated` `SearchOrder.ObservationStart` `SearchOrder.ObservationEnd` `SearchOrder.Popularity` `SearchOrder.GroupPopularity`  
        /// Optional, default: If the value of `searchType` is `SearchType.FullText` then the default value of `orderBy` is `SearchOrder.SearchRank`. If the value of `searchType` is `SearchType.SeriesId` then the default value of `orderBy` is `SearchOrder.SeriesId`.
        /// </param>
        /// <param name="sortOrder">
        /// Sort results is ascending or descending order for attribute values specified by `orderBy`.  
        /// optional, default: If `orderBy` is equal to `SearchOrder.SearchRank` or `SearchOrder.Popularity`, 
        /// then the default value of `sortOrder` is `SortOrder.Descending`. Otherwise, the default `sortOrder` is `SortOrder.Ascending`.
        /// </param>
        /// <returns>A collection of economic data series that match the parameters.</returns>
        member this.Search(searchText:string,?searchType:SearchType,?realtimeStart:DateTime,?realtimeEnd:DateTime,?limit:int,?orderBy:SearchOrder,?sortOrder:SortOrder) = 
            Search(key, searchText=searchText, ?searchType=searchType, ?realtimeStart=realtimeStart, ?realtimeEnd=realtimeEnd, ?limit=limit, ?orderBy=orderBy, ?sortOrder=sortOrder)

        /// <summary>
        /// Get the categories for an economic data series.
        /// </summary>
        /// <param name="id">The id for a series. String, required.</param>
        /// <param name="realtimeStart">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="realtimeEnd">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <returns>A collection of the series categories.</returns>
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
            Helpers.request key "series/categories" queryParameters
            |> SeriesCategoriesResponse.Parse

        /// <summary>
        /// Get the release for an economic data series.
        /// </summary>
        /// <param name="id">The id for a series. String, required.</param>
        /// <param name="realtimeStart">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="realtimeEnd">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <returns>A collection of fields describing the series release.</returns>
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
            Helpers.request key "series/release" queryParameters
            |> SeriesReleaseResponse.Parse 

        /// <summary>
        /// Get the FRED tags for a series.
        /// </summary>
        /// <param name="id">The id for a series. String, required.</param>
        /// <param name="realtimeStart">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="realtimeEnd">
        /// The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
        /// `DateTime(yyyy,MM,dd)` formatted `DateTime`, optional, default: today's date.
        /// </param>
        /// <param name="orderBy">
        /// Order results by values of the specified attribute.
        /// One of the following: 
        /// `OrderByTags.SeriesCount`, `OrderByTags.Popularity`, `OrderByTags.Created`, `OrderByTags.Name`, `OrderByTags.GroupId`.
        /// optional, default: `OrderByTags.SeriesCount`
        /// </param>
        /// <param name="sortOrder">
        /// Sort results is ascending or descending order for attribute values specified by `orderBy`.  
        /// optional, default: `SortOrder.Ascending`.
        /// </param>
        /// <returns></returns>
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
            Helpers.request key "series/tags" queryParameters
            |> SeriesTagsResponse.Parse 


/// <summary>
/// FRED module.  
/// Short for Federal Reserve Economic Data, FRED is an online database consisting of hundreds 
/// of thousands of economic data time series from scores of national, international, public, and private sources.
/// This module contains a set of methods that allow acess to FRED data.
/// </summary>
/// <param name="key">
/// 32 character alpha-numeric lowercase string, required.  
/// [Get you FRED API key.](https://fred.stlouisfed.org/docs/api/api_key.html)
/// </param>
type Fred(key:string) =
    member this.Series = Series.Series(key)
    
