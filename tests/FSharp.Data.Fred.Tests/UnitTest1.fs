module FSharp.Data.Fred.Tests

open NUnit.Framework
open FSharp.Data
open FSharp.Data.Fred
open FsUnit
open System
open System.IO

let envVars = System.Environment.GetEnvironmentVariables()
[<Literal>]
let KeyJson = __SOURCE_DIRECTORY__ + "../../../fredKey.json" 
let apiKey = 
    let var = "FRED_KEY"
    if envVars.Contains var then 
        envVars.[var] :?> string
    elif File.Exists(KeyJson) then 
        KeyFile.Load(KeyJson).FredKey
    else failwith "could not find a key"

let tolerance = 1e-7

(**
*)
let myFred = Fred apiKey


// Observations used in tests.

let GS10obs =
    myFred.Series.Observations("GS10")
    |> fun root -> root.Observations
    |> Seq.find(fun x -> x.Date = DateTime(1961,04,01))

let DTP10J25obs =
    myFred.Series.Observations("DTP10J25", frequency = Frequency.WeeklyEndingFriday)
    |> fun root -> root.Observations
    |> Seq.find(fun x -> x.Date = DateTime(2015,4,3))

let GS10Categories = 
    myFred.Series.Categories("GS10")

let DTP10J25Categories =
    myFred.Series.Categories("DTP10J25")

let GS10Info =
    myFred.Series.Info("GS10")

let DTP10J25Info =
    myFred.Series.Info("DTP10J25")

let GS10Release = 
    myFred.Series.Release "GS10"

let DTP10J25Release = 
    myFred.Series.Release "DTP10J25"

let GS10Tags = 
    myFred.Series.Tags "GS10"

let DTP10J25Tags =
    myFred.Series.Tags "DTP10J25"

// Tests

// Tests on myFred.Series.Observations

[<Test>] 
let ``Test 1961-04-01 GS10 observation`` () =  
    GS10obs.Value |> should (equalWithin tolerance) 3.78

[<Test>] 
let ``Test 2015-04-03 DTP10J25 observation`` () =  
    DTP10J25obs.Value |> should (equalWithin tolerance) 0.089

// Tests on myFred.Series.Categories

[<Test>] 
let ``Test GS10 category id field`` () =  
    GS10Categories |> fun root -> root.Categories |> Array.map(fun x -> x.Id) |> fun x -> x.[0] |> should (equalWithin tolerance) 115

[<Test>] 
let ``Test GS10 category name field`` () =  
    GS10Categories |> fun root -> root.Categories |> Array.map(fun x -> x.Name) |> fun x -> x.[0] |> should equal "Treasury Constant Maturity"

[<Test>] 
let ``Test GS10 category parentId field`` () =  
    GS10Categories |> fun root -> root.Categories |>  Array.map(fun x -> x.ParentId) |> fun x -> x.[0] |> should (equalWithin tolerance) 22

[<Test>] 
let ``Test DTP10J25 category id field`` () =  
    DTP10J25Categories |> fun root -> root.Categories |> Array.map(fun x -> x.Id) |> fun x -> x.[0] |> should (equalWithin tolerance) 82

[<Test>] 
let ``Test DTP10J25 category name field`` () =  
    DTP10J25Categories |> fun root -> root.Categories |> Array.map(fun x -> x.Name) |> fun x -> x.[0] |> should equal "Treasury Inflation-Indexed Securities"

[<Test>] 
let ``Test DTP10J25 category parentId field`` () =  
    DTP10J25Categories |> fun root -> root.Categories |> Array.map(fun x -> x.ParentId) |> fun x -> x.[0] |> should (equalWithin tolerance) 22

// Tests on myFred.Series.Info

[<Test>] 
let ``Test GS10 Info Title field`` () =  
    GS10Info |> fun root -> root.Seriess |> Array.map(fun x -> x.Title) |> fun x -> x.[0] |> should equal "10-Year Treasury Constant Maturity Rate"

[<Test>] 
let ``Test GS10 Info Frequency field`` () =  
    GS10Info |> fun root -> root.Seriess |> Array.map(fun x -> x.Frequency) |> fun x -> x.[0] |> should equal "Monthly"

[<Test>] 
let ``Test GS10 Info Units field`` () =  
    GS10Info |> fun root -> root.Seriess |> Array.map(fun x -> x.Units) |> fun x -> x.[0] |> should equal "Percent"

[<Test>] 
let ``Test GS10 Info SeasonalAdjustment field`` () =  
    GS10Info |> fun root -> root.Seriess |> Array.map(fun x -> x.SeasonalAdjustment) |> fun x -> x.[0] |> should equal "Not Seasonally Adjusted"

[<Test>] 
let ``Test DTP10J25 Info Title field`` () =  
    DTP10J25Info |> fun root -> root.Seriess |> Array.map(fun x -> x.Title) |> fun x -> x.[0] |> should equal "10-Year 0-1/4% Treasury Inflation-Indexed Note, Due 1/15/2025"

[<Test>] 
let ``Test DTP10J25 Info Frequency field`` () =  
    DTP10J25Info |> fun root -> root.Seriess |> Array.map(fun x -> x.Frequency) |> fun x -> x.[0] |> should equal "Daily"

[<Test>] 
let ``Test DTP10J25 Info Units field`` () =  
    DTP10J25Info |> fun root -> root.Seriess |> Array.map(fun x -> x.Units) |> fun x -> x.[0] |> should equal "Percent"

[<Test>] 
let ``Test DTP10J25 Info SeasonalAdjustment field`` () =  
    DTP10J25Info |> fun root -> root.Seriess |> Array.map(fun x -> x.SeasonalAdjustment) |> fun x -> x.[0] |> should equal "Not Seasonally Adjusted"

// Tests on myFred.Series.Release

[<Test>] 
let ``Test GS10 Release Id field`` () =  
    GS10Release |> fun root -> root.Releases |> Array.map(fun x -> x.Id) |> fun x -> x.[0] |> should (equalWithin tolerance) 18

[<Test>] 
let ``Test GS10 Release Name field`` () =  
    GS10Release |> fun root -> root.Releases |> Array.map(fun x -> x.Name) |> fun x -> x.[0] |> should equal "H.15 Selected Interest Rates"

[<Test>] 
let ``Test DTP10J25 Release Id field`` () =  
    DTP10J25Release |> fun root -> root.Releases |> Array.map(fun x -> x.Id) |> fun x -> x.[0] |> should (equalWithin tolerance) 72

[<Test>] 
let ``Test DTP10J25 Release Name field`` () =  
    DTP10J25Release |> fun root -> root.Releases |> Array.map(fun x -> x.Name) |> fun x -> x.[0] |> should equal "Daily Treasury Inflation-Indexed Securities"

// Tests on myFred.Series.Tags

[<Test>]
let ``Test GS10 Tag Name field Contains`` () =  
    GS10Tags |> fun root -> root.Tags |> Array.map(fun x -> x.Name) |> fun x -> (x |> Array.contains "h15" && x |> Array.contains "10-year" ) |> fun boolean -> Assert.True(boolean)

[<Test>]
let ``Test GS10 Tag GroupId field Contains`` () =  
    GS10Tags |> fun root -> root.Tags |> Array.map(fun x -> x.GroupId) |> fun x -> (x |> Array.contains "rls" && x |> Array.contains "gen" ) |> fun boolean -> Assert.True(boolean)

[<Test>]
let ``Test DTP10J25 Tag Name field Contains`` () =  
    DTP10J25Tags |> fun root -> root.Tags |> Array.map(fun x -> x.Name) |> fun x -> (x |> Array.contains "nyt" && x |> Array.contains "treasury" ) |> fun boolean -> Assert.True(boolean)

[<Test>]
let ``Test DTP10J25 Tag GroupId field Contains`` () =  
    DTP10J25Tags |> fun root -> root.Tags |> Array.map(fun x -> x.GroupId) |> fun x -> (x |> Array.contains "src" && x |> Array.contains "freq" ) |> fun boolean -> Assert.True(boolean)

