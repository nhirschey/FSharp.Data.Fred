#if INTERACTIVE
#r "nuget: FSharp.Data,4.2.3"
#r "nuget: NUnit"
#r "nuget: FsUnit"
#r "../../src/FSharp.Data.Fred/bin/Debug/netstandard2.0/FSharp.Data.Fred.dll"
ignore <| FSharp.Data.WorldBankData.GetDataContext() // Force fsi to load F# Data
#else
module FSharp.Data.Fred.Tests
#endif

open NUnit.Framework
open FSharp.Data.Fred
open FsUnit
open System

[<Literal>]
let KeyJson = __SOURCE_DIRECTORY__ + "/../../fredKey.json" 

let apiKey = "developer"

let tolerance = 1e-7

(**
*)
let Tests =
    match apiKey with 
    | "developer" ->
        let sampleFred = Fred "developer"
        //Sample Observations (Without API KEY)
        let seriesObservationTest = 
            sampleFred.Series.Observations ""
            |> fun root -> root.Observations
            |> Seq.find(fun x -> x.Date = DateTime(1929,01,01))
        let seriesCategoriesTest = sampleFred.Series.Categories ""
        let seriesInfoTest = sampleFred.Series.Info ""
        let seriesReleaseTest = sampleFred.Series.Release ""
        let seriesTagsTest = sampleFred.Series.Tags ""

        {|
            ObservationTest = seriesObservationTest.Value,  1065.9
            CategoriesTestId = seriesCategoriesTest.Categories |> Array.map(fun x -> x.Id) |> Array.head, 95
            CategoriesTestName = seriesCategoriesTest.Categories |> Array.map(fun x -> x.Name) |> Array.head, "Monthly Rates"
            CategoriesTestParentId = seriesCategoriesTest.Categories |> Array.map(fun x -> x.ParentId) |> Array.head, 15
            InfoTestTitle = seriesInfoTest.Seriess |> Array.map(fun x -> x.Title) |> Array.head, "Real Gross National Product"
            InfoTestFrequency = seriesInfoTest.Seriess |> Array.map(fun x -> x.Frequency) |> Array.head, "Annual"
            InfoTestUnits = seriesInfoTest.Seriess |> Array.map(fun x -> x.Units) |> Array.head, "Billions of Chained 2009 Dollars"
            InfoTestSeasonalAdjustment =  seriesInfoTest.Seriess |> Array.map(fun x -> x.SeasonalAdjustment) |> Array.head, "Not Seasonally Adjusted"
            ReleaseTestId = seriesReleaseTest.Releases |> Array.map(fun x -> x.Id) |> Array.head, 21
            ReleaseTestName = seriesReleaseTest.Releases |> Array.map(fun x -> x.Name) |> Array.head, "H.6 Money Stock Measures"
            TagsTestName = 
                let elements = seriesTagsTest.Tags |> Array.map(fun x -> x.Name) 
                (elements |> Array.contains  "nation") && (elements |> Array.contains "stlfsi" )
            TagsTestGroupId = 
                let elements = seriesTagsTest.Tags |> Array.map(fun x -> x.GroupId) 
                (elements |> Array.contains "geot" && elements |> Array.contains "rls" )
        |}
            
    | _ -> 
        let myFred = Fred apiKey
        //Real Observations (With API KEY)
        let seriesObservationTest =
            myFred.Series.Observations("GS10")
            |> fun root -> root.Observations
            |> Seq.find(fun x -> x.Date = DateTime(1961,04,01))

        let seriesCategoriesTest = myFred.Series.Categories "GS10"
        let seriesInfoTest = myFred.Series.Info "GS10"
        let seriesReleaseTest =  myFred.Series.Release "GS10"
        let seriesTagsTest = myFred.Series.Tags "GS10"

        {|
            ObservationTest = seriesObservationTest.Value,  3.78
            CategoriesTestId = seriesCategoriesTest.Categories |> Array.map(fun x -> x.Id) |> Array.head, 115
            CategoriesTestName = seriesCategoriesTest.Categories |> Array.map(fun x -> x.Name) |> Array.head, "Treasury Constant Maturity"
            CategoriesTestParentId = seriesCategoriesTest.Categories |> Array.map(fun x -> x.ParentId) |> Array.head, 22
            InfoTestTitle = seriesInfoTest.Seriess |> Array.map(fun x -> x.Title) |> Array.head, "Market Yield on U.S. Treasury Securities at 10-Year Constant Maturity, Quoted on an Investment Basis"
            InfoTestFrequency = seriesInfoTest.Seriess |> Array.map(fun x -> x.Frequency) |> Array.head, "Monthly"
            InfoTestUnits = seriesInfoTest.Seriess |> Array.map(fun x -> x.Units) |> Array.head, "Percent"
            InfoTestSeasonalAdjustment =  seriesInfoTest.Seriess |> Array.map(fun x -> x.SeasonalAdjustment) |> Array.head, "Not Seasonally Adjusted"
            ReleaseTestId = seriesReleaseTest.Releases |> Array.map(fun x -> x.Id) |> Array.head, 18
            ReleaseTestName = seriesReleaseTest.Releases |> Array.map(fun x -> x.Name) |> Array.head, "H.15 Selected Interest Rates"
            TagsTestName = seriesTagsTest.Tags |> Array.map(fun x -> x.Name) |> fun elements -> (elements |> Array.contains "h15" && elements |> Array.contains "10-year" )
            TagsTestGroupId = seriesTagsTest.Tags |> Array.map(fun x -> x.GroupId) |> fun elements -> (elements |> Array.contains "rls" && elements |> Array.contains "gen" )
        |}

[<Test>] 
let ``Test observations`` () =  
    Tests.ObservationTest |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Categories Id`` () =  
    Tests.CategoriesTestId |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Categories Name`` () =  
    Tests.CategoriesTestName |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Categories ParentId`` () =  
    Tests.CategoriesTestParentId |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Info Title`` () =  
    let actual, expected = Tests.InfoTestTitle
    if actual <> expected then
        Assert.Fail($"expected:\n{expected}\nactual:\n{actual}")

[<Test>] 
let ``Test Info Frequency`` () =  
    Tests.InfoTestFrequency |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Info Units`` () =  
    Tests.InfoTestUnits |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Info SeasonalAdjustment`` () =  
    Tests.InfoTestSeasonalAdjustment |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Releases Id`` () =  
    Tests.ReleaseTestId |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Releases Name`` () =  
    Tests.ReleaseTestName |> fun (realResult, expectedResult) -> realResult |> should (equalWithin tolerance) expectedResult

[<Test>] 
let ``Test Tags Name`` () =  
    Assert.True(Tests.TagsTestName)

[<Test>] 
let ``Test Tags GroupId`` () =  
    Assert.True(Tests.TagsTestGroupId)