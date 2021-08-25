
#I "bin/Debug"
#I "bin/Release"
#r "net5.0/FSharp.Data.Fred.dll"
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET,2.0.0-preview.6"

open FSharp.Data
open FSharp.Data.Fred
open FSharp.Data.Fred.JsonApi
open Plotly.NET

Fred.Load "GS10"

let chart1y10y =
    ["GS1", "1-Year"
     "GS10", "10-Year"]
    |> Seq.map(fun (series, name) -> 
        Fred.Load(series).Rows
        |> Seq.map(fun row -> row.Date, row.Value)
        |> Chart.Line 
        |> Chart.withTraceName name)
    |> Chart.Combine
    |> Chart.withTitle "Treasury Constant Maturity Rates"
    |> Chart.withY_AxisStyle(title = "Interest Rate (%)")
chart1y10y |> Chart.Show

#load "../../secrets.fsx"

// v1 json api

let fred = Fred1(Secrets.fredApi)

// normal .NET would be fred.Series.Info("GS10")
fred.series.info("GS10")

// normal .NET would be fred.Series.Search("10-year", limit=5)
let search = fred.series.search("10-year", limit=5)
search.Summary()

search.Results.Seriess
|> Seq.map(fun series -> series.Id, series.Notes)

// normal .NET would be fred.Series.Observations("GS10")
let obs = fred.series.observations("GS10")


obs.Observations
|> Seq.take 5
|> Seq.averageBy(fun x -> x.Value)


// v2 json api

// normal .NET would be fred2.Config(Secrets.fredApi)
fred2.config(Secrets.fredApi)

// normal .NET would be fred2.Series.Info("GS10")
fred2.series.info("GS10")

// normal .NET would be fred2.Series.Search("10-year", limit=5)
let search2 = fred2.series.search("10-year", limit=5)
search2.Summary()

search2.Results.Seriess
|> Seq.map(fun series -> series.Id, series.Notes)

// normal .NET would be fred2.Series.Observations("GS10")
let obs2 = fred2.series.observations("GS10")


obs2.Observations
|> Seq.take 5
|> Seq.averageBy(fun x -> x.Value)
