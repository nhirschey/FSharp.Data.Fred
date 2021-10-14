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

[<AutoOpen>]
module QueryParameters =
    type SearchType = 
        | FullText 
        | SeriesId

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
    type OrderByTags =
            | SeriesCount
            | PopularityTags
            | Created
            | Name
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
    static member loadKey(keyFile) =
        let envVars = System.Environment.GetEnvironmentVariables()
        let var = "FRED_KEY"
        if envVars.Contains var then 
            envVars.[var] :?> string
        elif IO.File.Exists(keyFile) then 
            Json.KeyFile.Load(keyFile).FredKey
        else "developer"
                                                                                            
    member this.Series = Series(key)
    member this.Key = key


