
#I "bin/Debug"
#I "bin/Release"
#r "net5.0/FSharp.Data.Fred.dll"
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET,2.0.0-preview.6"

open FSharp.Data
open FSharp.Data.Fred
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


