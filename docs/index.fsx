(**

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/FSharp.Data.Fred/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

(*** hide ***)
//#r "FSharp.Data.dll"
#r "../src/FSharp.Data.Fred/bin/Release/net5.0/FSharp.Data.Fred.dll"
#r "nuget: FSharp.Data"
ignore <| FSharp.Data.WorldBankData.GetDataContext() // Force fsi to load F# Data

(**
F# Data FRED
===================

F# Data FRED is a library for
[FRED database](https://fred.stlouisfed.org/) data access
based on FSharp.Data. 

Short for Federal Reserve Economic Data, FRED is an online database consisting 
of hundreds of thousands of economic data time series from scores of national, 
international, public, and private sources.

This library uses the [FRED API](https://fred.stlouisfed.org/docs/api/fred/) to access the data.
*)

(**
You can use `FSharp.Data.Fred` in [dotnet interactive](https://github.com/dotnet/interactive) 
notebooks in [Visual Studio Code](https://code.visualstudio.com/) 
or [Jupyter](https://jupyter.org/), or in F# scripts (`.fsx` files), 
by referencing the package as follows:

    // Use one of the following two lines
    #r "nuget: FSharp.Data.Fred" // Use the latest version
    #r "nuget: FSharp.Data.Fred,{{fsdocs-package-version}}" // Use a specific version   

    #r "nuget: FSharp.Data" //Also load FSharp.Data
*)

(*** condition: fsx ***)
#if FSX
#r "nuget: FSharp.Data"
#r "nuget: FSharp.Data.Fred,{{fsdocs-package-version}}"
#endif // FSX
(*** condition: ipynb ***)
#if IPYNB
#r "nuget: FSharp.Data"
#r "nuget: FSharp.Data.Fred,{{fsdocs-package-version}}"

// Set dotnet interactive formatter to plaintext
Formatter.SetPreferredMimeTypesFor(typeof<obj>, "text/plain")
Formatter.Register(fun (x:obj) (writer: TextWriter) -> fprintfn writer "%120A" x )
#endif // IPYNB

(** Open namespaces *)
open FSharp.Data
open FSharp.Data.Fred
open System // Usefull to access DateTime

(**
## F# Data FRED
First in order to use the methods you'll need to create a value with type `Fred`. 

Fred receives an API key as a parameter (`string`). [Get your FRED API key.](https://fred.stlouisfed.org/docs/api/api_key.html)

Example:

    let apiKey = "insert API key here"
    let myFred = Fred apiKey
    
*)

(***do-not-eval,condition:ipynb,fsx***)
#if IPYNB 
let apiKey = "insert API key here"
let myFred = Fred apiKey
#endif // IPYNB

(***do-not-eval,condition:fsx***)
#if FSX 
let apiKey = "insert API key here"
let myFred = Fred apiKey
#endif // FSX

(**
*)

(***hide***)
open System.IO
let envVars = System.Environment.GetEnvironmentVariables()
[<Literal>]
let KeyJson = __SOURCE_DIRECTORY__ + "/../fredKey.json" 

let hiddenApiKey = Fred.loadKey KeyJson

let myFred = Fred hiddenApiKey

(**
Now you can use the created value `myFred` to access all the methods in the FRED library.
*)

(**
## FRED.Series Module Functions:

1. [Search.](#F-Data-FRED-Series-Search)
1. [Info.](#F-Data-FRED-Series-Info)
1. [Categories.](#F-Data-FRED-Series-Categories)
1. [Release.](#F-Data-FRED-Series-Release)
1. [Tags.](#F-Data-FRED-Series-Tags)
1. [Observations.](#F-Data-FRED-Series-Observations)
*)

(**
### F# Data FRED: Series.Search
A collection of economic data series that match the parameters.

The parameters are the following:

1. **searchText**, required.  
    The words to match against economic data series.
1. **searchType**, optional. Default is `SearchType.FullText`.  
    Determines the type of search to perform.
    One of the following: `SearchType.FullText` or `SearchType.SeriesId`.
    - `SearchType.FullText` searches series attributes title, units, frequency, and tags by parsing words into stems. 
    This makes it possible for searches like 'Industry' to match series containing related words such as 'Industries'. 
    Of course, you can search for multiple words like 'money' and 'stock'. Remember to url encode spaces (e.g. `'money%20stock'`).
    - `SearchType.SeriesId` performs a substring search on series IDs. Searching for 'ex' will find series containing 'ex' anywhere in a series ID.
    '*' can be used to anchor searches and match 0 or more of any character. Searching for 'ex*' will find series containing 'ex' at the beginning of a series ID.
    Searching for '*ex' will find series containing 'ex' at the end of a series ID. It's also possible to put an '*' in the middle of a string.
    'm*sl' finds any series starting with 'm' and ending with 'sl'.
1. **realtimeStart**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **realtimeEnd**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **limit**, optional. Default: `1000`.  
    The maximum number of results to return.
    integer between `1` and `1000`.
1. **orderBy**, optional. Default: If the value of `searchType` is `SearchType.FullText` then the default value of `orderBy` is `SearchOrder.SearchRank`. If the value of `searchType` is `SearchType.SeriesId` then the default value of `orderBy` is `SearchOrder.SeriesId`.  
    Order results by values of the specified attribute.
    One of the following:  
    `SearchOrder.SearchRank` `SearchOrder.SeriesIdOrder` `SearchOrder.Title` `SearchOrder.Units` `SearchOrder.Frequency` `SearchOrder.SeasonalAdjustment` 
    `SearchOrder.RealTimeStart` `SearchOrder.RealTimeEnd` `SearchOrder.LastUpdated` `SearchOrder.ObservationStart` `SearchOrder.ObservationEnd` `SearchOrder.Popularity` `SearchOrder.GroupPopularity`  
1. **sortOrder**, optional. Default: If `orderBy` is equal to `SearchOrder.SearchRank` or `SearchOrder.Popularity`, then the default value of `sortOrder` is `SortOrder.Descending`. Otherwise, the default `sortOrder` is `SortOrder.Ascending`.  
    Sort results is ascending or descending order for attribute values specified by `orderBy`.  

Examples:
*)

(**
Search for `"10-Year"` text without specifying any optional parameters.
*)

myFred.Series.Search("10-Year").Summary()
(***include-output***)

(**
Search for `"10-Year"` text specifying some optional parameters.
*)
myFred.Series.Search("10-Year", 
                     searchType = SearchType.FullText, 
                     limit = 3, 
                     orderBy = SearchOrder.GroupPopularity, 
                     sortOrder = SortOrder.Descending).Summary()
(***include-output***)

(**
Search for `"Hello World"` text that should not match any series.
*)
myFred.Series.Search("Hello World").Summary()
(***include-output***)

(**
### F# Data FRED: Series.Info
Get the information for an economic data series.

This method only asks for the series id as a parameter (`string`)

Examples:
*)

(**
Get the information for series id = `"GS10"`
*)

myFred.Series.Info "GS10"
(***include-fsi-output***)

(**
### F# Data FRED: Series.Categories
Get the categories for an economic data series.

The parameters are the following:

1. **id**, required.  
    The id for a series (`string`).
1. **realtimeStart**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **realtimeEnd**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).


Examples:
*)

(**
Get the category information for series id = `"GS10"`
*)
myFred.Series.Categories "GS10"
(***include-fsi-output***)

(**
You can also access the category fields with `Array.map`:
*)
myFred.Series.Categories("GS10").Categories
|> Array.map(fun x -> x.Name)
(***include-fsi-output***)

(**
### F# Data FRED: Series.Release
 Get the release for an economic data series.

The parameters are the following:

1. **id**, required.  
    The id for a series (`string`).
1. **realtimeStart**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **realtimeEnd**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

Examples:
*)

(**
Get the release information for series id = `"GS10"`
*)
myFred.Series.Release "GS10"
(***include-fsi-output***)

(**
### F# Data FRED: Series.Tags
Get the FRED tags for a series.

The parameters are the following:

1. **id**, required.  
    The id for a series (`string`).
1. **realtimeStart**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **realtimeEnd**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **orderBy**, optional. Default: `OrderByTags.SeriesCount`.
    Order results by values of the specified attribute.
    One of the following:  
    `OrderByTags.SeriesCount`, `OrderByTags.Popularity`, `OrderByTags.Created`, `OrderByTags.Name`, `OrderByTags.GroupId`.
1. **sortOrder**, optional. Default: `SortOrder.Ascending`.
    Sort results is ascending or descending order for attribute values specified by `orderBy`.  

Examples:
*)

(**
Get the first 3 tags information for series id = `"GS10"` without specifying any optional parameters.
*)
myFred.Series.Tags("GS10").Tags
|> Array.truncate 3 
(***include-fsi-output***)

(**
Get the first 3 tags information for series id = `"GS10"` specifying some optional parameters.
*)
myFred.Series.Tags("GS10", orderBy = OrderByTags.PopularityTags, sortOrder = SortOrder.Descending).Tags
|> Array.truncate 3 
(***include-fsi-output***)

(**
### F# Data FRED: Series.Observations
Get the observations or data values for an economic data series.

The parameters are the following:

1. **id**, required.  
    The id for a series (`string`).
1. **realtimeStart**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **realtimeEnd**, optional. Default: today's date.  
    The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).
1. **limit**, optional. Default: `1000`.  
    The maximum number of results to return.
    integer between `1` and `1000`.
1. **sortOrder**, optional. Default: `SortOrder.Ascending`.  
    Sort results is ascending or descending observation date order.
1. **observationStart**, optional. Default: `DateTime(1776,07,04)` (earliest available).  
    The start of the observation period. `DateTime(yyyy,MM,dd)` formatted `DateTime`.
1. **observationEnd**, optional. Default: today's date.   `DateTime(9999,12,31)` (latest available).  
    The end of the observation period. `DateTime(yyyy,MM,dd)` formatted `DateTime`.
1. **units**, optional. Default: `Units.Levels` (No transformation).  
    A key that indicates a data value transformation.
    One of the following:   
    `Units.Levels`
    `Units.Change`
    `Units.ChangefromYearAgo`
    `Units.PercentChange`
    `Units.PercentChangefromYearAgo`
    `Units.CompoundedAnnualRateofChange`
    `Units.ContinuouslyCompoundedRateofChange`
    `Units.ContinuouslyCompoundedAnnualRateofChange`
    `Units.NaturalLog`.  
    [Unit transformation formulas](https://alfred.stlouisfed.org/help#growth_formulas)
1. **frequency**, optional.  
    An optional parameter that indicates a lower frequency to aggregate values to. 
    The FRED frequency aggregation feature converts higher frequency data series into lower frequency 
    data series (e.g. converts a monthly data series into an annual data series). 
    In FRED, the highest frequency data is daily, and the lowest frequency data is annual. 
    There are 3 aggregation methods available- average, sum, and end of period. 
    [See the aggregation_method parameter.](https://fred.stlouisfed.org/docs/api/fred/series_observations.html#aggregation_method)  
    One of the following: 
    `Frequency.Daily`, `Frequency.Weekly`, `Frequency.Biweekly`, `Frequency.Monthly`, `Frequency.Quarterly`, 
    `Frequency.Semiannual`, `Frequency.Annual`, `Frequency.WeeklyEndingFriday`,
    `Frequency.WeeklyEndingThursday`, `Frequency.WeeklyEndingWednesday`, `Frequency.WeeklyEndingTuesday`, 
    `Frequency.WeeklyEndingMonday`, `Frequency.WeeklyEndingSunday`, `Frequency.WeeklyEndingSaturday`, 
    `Frequency.BiweeklyEndingWednesday`, `Frequency.BiweeklyEndingMonday`.
1. **aggMethod**, optional. Default: `AggMethod.Average.`  
    A key that indicates the aggregation method used for frequency aggregation. 
    This parameter has no affect if the [frequency parameter](https://fred.stlouisfed.org/docs/api/fred/series_observations.html#frequency) is not set.
    One of the following:
    `AggMethod.Average`, `AggMethod.Sum`, `AggMethod.EndOfPeriod`.

Examples:
*)

(**
Get the observations for `"GS10"` series without specifying any optional parameters.
*)
myFred.Series.Observations("GS10").Observations
|> Seq.truncate 3 
(***include-fsi-output***)

(**
Get the observations for `"DTP10J25"` with a semi-annual frequency.
*)
myFred.Series.Observations("DTP10J25", frequency = Frequency.Semiannual).Observations
|> Seq.truncate 3 
(***include-fsi-output***)
