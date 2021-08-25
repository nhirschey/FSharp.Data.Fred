
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

let fred = Fred(Secrets.fredApi)

// normal .NET would be fred.Series.Info("GS10")
fred.Series.Info("GS10")

// normal .NET would be fred.Series.Search("10-year", limit=5)
let search = fred.Series.Search("10-year", limit=5)
search.Summary()

search.Results.Seriess
|> Seq.map(fun series -> series.Id, series.Notes)

// normal .NET would be fred.Series.Observations("GS10")
let obs = fred.Series.Observations("GS10")


obs.Observations
|> Seq.take 5
|> Seq.averageBy(fun x -> x.Value)


let cats = fred.Series.Categories("GS10")
cats.Categories