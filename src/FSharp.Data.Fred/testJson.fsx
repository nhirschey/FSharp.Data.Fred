
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET,2.0.0-preview.6"

open FSharp.Data
open Plotly.NET


let fredApi = "keyhere"


(*
expects secrets.fsx to look like:
let fredApi = "keyhere"
but replace keyhere with your api key.
*)
//#load "../../secrets.fsx"

(* 
open FSharp.Data.JsonExtensions
let interestRates = 
    Http.RequestString($"https://api.stlouisfed.org/fred/series/search?search_text=10+year+rate&api_key={fredApi}&file_type=json", 
                       headers = [ HttpRequestHeaders.UserAgent "FSharp.Data WorldBank Type Provider" 
                                   HttpRequestHeaders.Accept HttpContentTypes.Json ])
    |> JsonValue.Parse

interestRates?seriess.[0]
match interestRates?seriess with
| JsonValue.Array seriess ->
    for series in seriess |> Array.truncate 20 do
        let title = series?title.AsString().Replace("\"","")
        let id = series?id.AsString().Replace("\"","")
        printfn $"{title} ({id})"
| _ -> failwith "nope"
*)
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


type SeriesInfo(id:string) =
    let response =
        Http.RequestString($"https://api.stlouisfed.org/fred/series?series_id={id.ToUpper()}&api_key={fredApi}&file_type=json",
                       headers = [ HttpRequestHeaders.UserAgent "FSharp.Data WorldBank Type Provider" 
                                   HttpRequestHeaders.Accept HttpContentTypes.Json ])
        |> SeriesResponse.Parse
    member this.Info = response.Seriess |> Seq.head

type SeriesObservations(id:string) =
    let response =
        Http.RequestString($"https://api.stlouisfed.org/fred/series/observations?series_id={id.ToUpper()}&api_key={fredApi}&file_type=json",
                       headers = [ HttpRequestHeaders.UserAgent "FSharp.Data.Fred" 
                                   HttpRequestHeaders.Accept HttpContentTypes.Json ])
        |> SeriesObservationsResponse.Parse
    member this.Results = response

let xx = SeriesInfo("GS10")
xx.Info
let xxx = SeriesObservations("DGS10")
xxx.Results
xxx.Results.Observations
xxx.Results.Observations
|> Seq.take 5
|> Seq.map(fun x -> x.Value)

(**
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

module fred2 =
    (*
    // class alternative to record, but shouldn't change structure frequently
    // so probably ok to leave as record or tuple. 
    type SeriesObservation (date:System.DateTime,value:float) =
        member this.Date = date
        member this.Value = value
    *)
    type search(searchText:string,?searchType:SearchType,?limit:int) =
        let searchType = 
            match defaultArg searchType SearchType.FullText with
            | FullText -> "full_text"
            | SeriesId -> "series_id"
        let limit = defaultArg limit 20
        let searchText = System.Uri.EscapeUriString(searchText)
        let searchRequest =
            Http.RequestString($"https://api.stlouisfed.org/fred/series/search?search_text={searchText}&api_key={fredApi}&file_type=json",
                           query = [ "search_type", searchType
                                     "limit", string limit], 
                           headers = [ HttpRequestHeaders.UserAgent "FSharp.Data WorldBank Type Provider" 
                                       HttpRequestHeaders.Accept HttpContentTypes.Json ])
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
                printfn $"""%3i{i}. {series.Title} """
                printfn $"         Id: %-10s{series.Id} Period: {shortDate series.ObservationStart} to {shortDate series.ObservationEnd}  Freq: {series.Frequency} \n"


    type SeriesObservation = { Date: System.DateTime; Value : float}
    type downloadvRecord(id:string) =
        let observations = SeriesObservations(id)
        let info = SeriesInfo(id).Info
        member this.Info = info
        member this.Observations = 
            observations.Results.Observations
            |> Seq.map(fun x -> { Date = x.Date; Value = x.Value })
    
    // I probably prefer this version because returning a tuple of DateTime * float
    // is a bit simpler data structure. But will people used to pandas be confused
    // by the tuple vs. being able to do x.Date and x.Value?
    // 
    // this.Info is a json value. Is this confusing? Would it be better
    // to extract all the info info as we've done here with this.Title?
    type downloadvTuple(id:string) =
        let observations = SeriesObservations(id)
        let info = SeriesInfo(id).Info
        member this.Info = info
        member this.Title = info.Title
        member this.Observations = 
            observations.Results.Observations
            |> Seq.map(fun x -> x.Date, x.Value )

    let downloadvJson (id:string) =
        SeriesObservations(id).Results
        

// Code to experiemnt with API access begins here.

fsi.AddPrinter<System.DateTime>(fun x -> x.ToString("yyyy-MM-dd"))

let v3 = fred2.downloadvRecord "GS10"
v3.Info
v3.Observations
|> Seq.take 6
|> Seq.map(fun x -> x.Date, x.Value)

let v4 = fred2.downloadvTuple "DGS10"
v4.Title
v4.Observations 
|> Seq.take 6

let x = fred2.search("constant maturity")
x.Summary(30)
let x2 = fred2.search "10-year treasury"
x2.Summary()

x.Results.Seriess
|> Seq.map(fun x -> x.Id, x.Title)
let searchExample = fred2.search("constant maturity")
searchExample.Results.Seriess |> Seq.take 5
searchExample.Summary()

let idSearch = fred2.search("gs10",searchType=SearchType.SeriesId,limit=10)
idSearch.Summary()

let vRecord = fred2.downloadvRecord("GS10")

vRecord.Observations
|> Seq.take 5

vRecord.Observations
|> Seq.take 5
|> Seq.averageBy(fun x -> x.Value)

let vTuple = fred2.downloadvTuple("GS10")

vTuple.Observations
|> Seq.take 5

vTuple.Observations
|> Seq.take 5
|> Seq.averageBy(fun (dt, value) -> value)


let vJson = fred2.downloadvJson("GS10")

vJson.Observations
|> Seq.take 5

vJson.Observations
|> Seq.take 5
|> Seq.averageBy(fun x -> x.Value)