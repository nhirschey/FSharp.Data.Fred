module FSharp.Data.Fred.JsonApi

open FSharp.Data

type SearchType = FullText | SeriesId

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

type SearchResponse = JsonProvider<SearchResponseSample>
type SeriesResponse = JsonProvider<SeriesSample>
type SeriesObservationsResponse = JsonProvider<SeriesObservationsSample>

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

module Config =

    /// <summary>
    /// Should be set to your key for the FRED API.
    /// </summary>
    /// <example>
    /// <code>
    /// fred.config("your-api-key")
    /// Config.apiKey // evaluates to "your-api-key"
    /// </code>
    /// </example>
    /// <returns>API key</returns>
    let mutable apiKey = ""

module internal Helpers =
    let request key endpoint query =
        Http.RequestString($"https://api.stlouisfed.org/fred/{endpoint}?",
                   query = query @ [ "api_key", key; "file_type", "json"], 
                   headers = [ HttpRequestHeaders.UserAgent "FSharp.Data.Fred" 
                               HttpRequestHeaders.Accept HttpContentTypes.Json ])
    let request2 endpoint query =
        Http.RequestString($"https://api.stlouisfed.org/fred/{endpoint}?",
                   query = query @ [ "api_key", Config.apiKey; "file_type", "json"], 
                   headers = [ HttpRequestHeaders.UserAgent "FSharp.Data.Fred" 
                               HttpRequestHeaders.Accept HttpContentTypes.Json ])

module Series =
    type search(key:string,searchText:string,?searchType:SearchType,?limit:int) =
        let searchType = 
            match defaultArg searchType SearchType.FullText with
            | FullText -> "full_text"
            | SeriesId -> "series_id"
        let limit = defaultArg limit 20
        let searchText = System.Uri.EscapeUriString(searchText)
        let searchRequest = 
            let queryParameters = [
                "search_text", searchText
                "search_type", searchType 
                "limit", string limit
            ]
            Helpers.request key "series/search" queryParameters
            |> SearchResponse.Parse
        member this.Results = searchRequest
        member this.Summary(?n:int) =
            let shortDate (dt: System.DateTime) = dt.ToString("yyyy-MM-dd")
            // Currently if given n > limit then the returns search summary
            // will be less than n. Probably should print something for the user
            // so that they know why they're getting less than n results.
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
    and series(key:string) =
        do if key = "" then failwith "Provide an API key"
        member this.info(id:string) = 
            Helpers.request key "series" [ "series_id", id.ToUpper() ]
            |> SeriesResponse.Parse
        member this.observations(id:string) =
            Helpers.request key "series/observations" [ "series_id", id.ToUpper() ]
            |> SeriesObservationsResponse.Parse
        member this.search(searchText:string,?searchType:SearchType,?limit:int) = 
            search(key, searchText=searchText, ?searchType=searchType, ?limit=limit)

type Fred1(key:string) =
    member this.series = Series.series(key)


type fred2 =
    static member config(key:string) = Config.apiKey <- key
    static member series = Series.series(Config.apiKey)

