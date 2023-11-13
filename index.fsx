(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/FSharp.Data.Fred/gh-pages?filepath=index.ipynb)&emsp;
[![Script](img/badge-script.svg)](https://nhirschey.github.io/FSharp.Data.Fred//index.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](https://nhirschey.github.io/FSharp.Data.Fred//index.ipynb)

# F# Data FRED

F# Data FRED is a library for
[FRED database](https://fred.stlouisfed.org/) data access
based on FSharp.Data.
Short for Federal Reserve Economic Data, FRED is an online database consisting
of hundreds of thousands of economic data time series from scores of national,
international, public, and private sources.
This library uses the [FRED API](https://fred.stlouisfed.org/docs/api/fred/) to access the data.

You can use `FSharp.Data.Fred` in [dotnet interactive](https://github.com/dotnet/interactive)
notebooks in [Visual Studio Code](https://code.visualstudio.com/)
or [Jupyter](https://jupyter.org/), or in F# scripts (`.fsx` files),
by referencing the package as follows:

    #r "nuget: FSharp.Data.Fred, 0.2.1"   
    #r "nuget: FSharp.Data" //Also load FSharp.Data

*)
open FSharp.Data
open FSharp.Data.Fred
open System //Usefull to access DateTime
(**
## F# Data FRED

First in order to use the methods you'll need to create a value with type `Fred`.
Fred receives an API key as a parameter (`string`). [Get your FRED API key.](https://fred.stlouisfed.org/docs/api/api_key.html)
Example:
let apiKey = "insert API key here"

Now you can use the created value `myFred` to access all the methods in the FRED library.

## FRED.Series Module Functions:

0 [Search.](#F-Data-FRED-Series-Search)

1 [Info.](#F-Data-FRED-Series-Info)

2 [Categories.](#F-Data-FRED-Series-Categories)

3 [Release.](#F-Data-FRED-Series-Release)

4 [Tags.](#F-Data-FRED-Series-Tags)

5 [Observations.](#F-Data-FRED-Series-Observations)

### F# Data FRED: Series.Search

A collection of economic data series that match the parameters.

The parameters are the following:

0 **searchText**, required.
The words to match against economic data series.

1 **searchType**, optional. Default is `SearchType.FullText`.
Determines the type of search to perform.
One of the following: `SearchType.FullText` or `SearchType.SeriesId`.

  * `SearchType.FullText` searches series attributes title, units, frequency, and tags by parsing words into stems. 
This makes it possible for searches like 'Industry' to match series containing related words such as 'Industries'.
Of course, you can search for multiple words like 'money' and 'stock'. Remember to url encode spaces (e.g. `'money%20stock'`).
  
  * `SearchType.SeriesId` performs a substring search on series IDs. Searching for 'ex' will find series containing 'ex' anywhere in a series ID.
'**' can be used to anchor searches and match 0 or more of any character. Searching for 'ex**' will find series containing 'ex' at the beginning of a series ID.
Searching for '**ex' will find series containing 'ex' at the end of a series ID. It's also possible to put an '**' in the middle of a string.
'm*sl' finds any series starting with 'm' and ending with 'sl'.
  

2 **realtimeStart**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

3 **realtimeEnd**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

4 **limit**, optional. Default: `1000`.
The maximum number of results to return.
integer between `1` and `1000`.

5 **orderBy**, optional. Default: If the value of `searchType` is `SearchType.FullText` then the default value of `orderBy` is `SearchOrder.SearchRank`. If the value of `searchType` is `SearchType.SeriesId` then the default value of `orderBy` is `SearchOrder.SeriesId`.
Order results by values of the specified attribute.
One of the following:
`SearchOrder.SearchRank` `SearchOrder.SeriesIdOrder` `SearchOrder.Title` `SearchOrder.Units` `SearchOrder.Frequency` `SearchOrder.SeasonalAdjustment`
`SearchOrder.RealTimeStart` `SearchOrder.RealTimeEnd` `SearchOrder.LastUpdated` `SearchOrder.ObservationStart` `SearchOrder.ObservationEnd` `SearchOrder.Popularity` `SearchOrder.GroupPopularity`

6 **sortOrder**, optional. Default: If `orderBy` is equal to `SearchOrder.SearchRank` or `SearchOrder.Popularity`, then the default value of `sortOrder` is `SortOrder.Descending`. Otherwise, the default `sortOrder` is `SortOrder.Ascending`.
Sort results is ascending or descending order for attribute values specified by `orderBy`.

Examples:

Search for `"10-Year"` text without specifying any optional parameters.

*)
myFred.Series.Search("10-Year").Summary()(* output: 
Count of search hits: 10590
Top 10 results:
  1. 10-Year Treasury Constant Maturity Minus 2-Year Treasury Constant Maturity 
         Id: T10Y2Y     Period: 1976-06-01 to 2023-11-09  Freq: Daily 

  2. 10-Year Treasury Constant Maturity Minus 2-Year Treasury Constant Maturity 
         Id: T10Y2YM    Period: 1976-06-01 to 2023-10-01  Freq: Monthly 

  3. 10-Year Treasury Constant Maturity Minus 3-Month Treasury Constant Maturity 
         Id: T10Y3M     Period: 1982-01-04 to 2023-11-09  Freq: Daily 

  4. Market Yield on U.S. Treasury Securities at 10-Year Constant Maturity, Quoted on an Investment Basis 
         Id: DGS10      Period: 1962-01-02 to 2023-11-08  Freq: Daily 

  5. Market Yield on U.S. Treasury Securities at 10-Year Constant Maturity, Quoted on an Investment Basis 
         Id: GS10       Period: 1953-04-01 to 2023-10-01  Freq: Monthly 

  6. 10-Year Treasury Constant Maturity Minus 3-Month Treasury Constant Maturity 
         Id: T10Y3MM    Period: 1982-01-01 to 2023-10-01  Freq: Monthly 

  7. Market Yield on U.S. Treasury Securities at 10-Year Constant Maturity, Quoted on an Investment Basis 
         Id: WGS10YR    Period: 1962-01-05 to 2023-11-03  Freq: Weekly, Ending Friday 

  8. Market Yield on U.S. Treasury Securities at 10-Year Constant Maturity, Quoted on an Investment Basis 
         Id: RIFLGFCY10NA Period: 1962-01-01 to 2022-01-01  Freq: Annual 

  9. 10-Year Breakeven Inflation Rate 
         Id: T10YIE     Period: 2003-01-02 to 2023-11-09  Freq: Daily 

 10. 10-Year Breakeven Inflation Rate 
         Id: T10YIEM    Period: 2003-01-01 to 2023-10-01  Freq: Monthly*)
(**
Search for `"10-Year"` text specifying some optional parameters.

*)
myFred.Series.Search("10-Year", 
                     searchType = SearchType.FullText, 
                     limit = 3, 
                     orderBy = SearchOrder.GroupPopularity, 
                     sortOrder = SortOrder.Descending).Summary()(* output: 
Count of search hits: 10590
Top 3 results:
  1. 10-Year Treasury Constant Maturity Minus 2-Year Treasury Constant Maturity 
         Id: T10Y2Y     Period: 1976-06-01 to 2023-11-09  Freq: Daily 

  2. 10-Year Treasury Constant Maturity Minus 2-Year Treasury Constant Maturity 
         Id: T10Y2YM    Period: 1976-06-01 to 2023-10-01  Freq: Monthly 

  3. 10-Year Treasury Constant Maturity Minus 3-Month Treasury Constant Maturity 
         Id: T10Y3M     Period: 1982-01-04 to 2023-11-09  Freq: Daily*)
(**
Search for `"Hello World"` text that should not match any series.

*)
myFred.Series.Search("Hello World").Summary()(* output: 
Count of search hits: 0
Top 0 results:*)
(**
### F# Data FRED: Series.Info

Get the information for an economic data series.

This method only asks for the series id as a parameter (`string`)

Examples:

Get the information for series id = `"GS10"`

*)
myFred.Series.Info "GS10"(* output: 
val it: JsonProvider<...>.InfoResponse =
  {
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "seriess": [
    {
      "id": "GS10",
      "realtime_start": "2023-11-13",
      "realtime_end": "2023-11-13",
      "title": "Market Yield on U.S. Treasury Securities at 10-Year Constant Maturity, Quoted on an Investment Basis",
      "observation_start": "1953-04-01",
      "observation_end": "2023-10-01",
      "frequency": "Monthly",
      "frequency_short": "M",
      "units": "Percent",
      "units_short": "%",
      "seasonal...*)
(**
### F# Data FRED: Series.Categories

Get the categories for an economic data series.

The parameters are the following:

0 **id**, required.
The id for a series (`string`).

1 **realtimeStart**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

2 **realtimeEnd**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

Examples:

Get the category information for series id = `"GS10"`

*)
myFred.Series.Categories "GS10"(* output: 
val it: JsonProvider<...>.CategoriesResponse =
  {
  "categories": [
    {
      "id": 115,
      "name": "Treasury Constant Maturity",
      "parent_id": 22
    }
  ]
}*)
(**
You can also access the category fields with `Array.map`:

*)
myFred.Series.Categories("GS10").Categories
|> Array.map(fun x -> x.Name)(* output: 
val it: string array = [|"Treasury Constant Maturity"|]*)
(**
### F# Data FRED: Series.Release

Get the release for an economic data series.

The parameters are the following:

0 **id**, required.
The id for a series (`string`).

1 **realtimeStart**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

2 **realtimeEnd**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

Examples:

Get the release information for series id = `"GS10"`

*)
myFred.Series.Release "GS10"(* output: 
val it: JsonProvider<...>.ReleaseResponse =
  {
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "releases": [
    {
      "id": 18,
      "realtime_start": "2023-11-13",
      "realtime_end": "2023-11-13",
      "name": "H.15 Selected Interest Rates",
      "press_release": true,
      "link": "http://www.federalreserve.gov/releases/h15/"
    }
  ]
}*)
(**
### F# Data FRED: Series.Tags

Get the FRED tags for a series.

The parameters are the following:

0 **id**, required.
The id for a series (`string`).

1 **realtimeStart**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

2 **realtimeEnd**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

3 **orderBy**, optional. Default: `OrderByTags.SeriesCount`.
Order results by values of the specified attribute.
One of the following:
`OrderByTags.SeriesCount`, `OrderByTags.Popularity`, `OrderByTags.Created`, `OrderByTags.Name`, `OrderByTags.GroupId`.

4 **sortOrder**, optional. Default: `SortOrder.Ascending`.
Sort results is ascending or descending order for attribute values specified by `orderBy`.

Examples:

Get the first 3 tags information for series id = `"GS10"` without specifying any optional parameters.

*)
myFred.Series.Tags("GS10").Tags
|> Array.truncate 3 (* output: 
val it: JsonProvider<...>.Tag array =
  [|{
  "name": "h15",
  "group_id": "rls",
  "notes": "H.15 Selected Interest Rates",
  "created": "2012-08-16 15:21:17-05",
  "popularity": 56,
  "series_count": 270
};
    {
  "name": "10-year",
  "group_id": "gen",
  "notes": "",
  "created": "2012-02-27 10:18:19-06",
  "popularity": 53,
  "series_count": 288
};
    {
  "name": "interest rate",
  "group_id": "gen",
  "notes": "",
  "created": "2012-05-29 10:14:19-05",
  "popularity": 70,
  "series_count": 1596
}|]*)
(**
Get the first 3 tags information for series id = `"GS10"` specifying some optional parameters.

*)
myFred.Series.Tags("GS10", orderBy = OrderByTags.Popularity, sortOrder = SortOrder.Descending).Tags
|> Array.truncate 3 (* output: 
val it: JsonProvider<...>.Tag array =
  [|{
  "name": "h15",
  "group_id": "rls",
  "notes": "H.15 Selected Interest Rates",
  "created": "2012-08-16 15:21:17-05",
  "popularity": 56,
  "series_count": 270
};
    {
  "name": "10-year",
  "group_id": "gen",
  "notes": "",
  "created": "2012-02-27 10:18:19-06",
  "popularity": 53,
  "series_count": 288
};
    {
  "name": "interest rate",
  "group_id": "gen",
  "notes": "",
  "created": "2012-05-29 10:14:19-05",
  "popularity": 70,
  "series_count": 1596
}|]*)
(**
### F# Data FRED: Series.Observations

Get the observations or data values for an economic data series.

The parameters are the following:

0 **id**, required.
The id for a series (`string`).

1 **realtimeStart**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

2 **realtimeEnd**, optional. Default: today's date.
The start of the real-time period. For more information, see [Real-Time Periods](https://fred.stlouisfed.org/docs/api/fred/realtime_period.html).

3 **limit**, optional. Default: `1000`.
The maximum number of results to return.
integer between `1` and `1000`.

4 **sortOrder**, optional. Default: `SortOrder.Ascending`.
Sort results is ascending or descending observation date order.

5 **observationStart**, optional. Default: `DateTime(1776,07,04)` (earliest available).
The start of the observation period. `DateTime(yyyy,MM,dd)` formatted `DateTime`.

6 **observationEnd**, optional. Default: today's date.   `DateTime(9999,12,31)` (latest available).
The end of the observation period. `DateTime(yyyy,MM,dd)` formatted `DateTime`.

7 **units**, optional. Default: `Units.Levels` (No transformation).
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

8 **frequency**, optional.
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

9 **aggMethod**, optional. Default: `AggMethod.Average.`
A key that indicates the aggregation method used for frequency aggregation.
This parameter has no affect if the [frequency parameter](https://fred.stlouisfed.org/docs/api/fred/series_observations.html#frequency) is not set.
One of the following:
`AggMethod.Average`, `AggMethod.Sum`, `AggMethod.EndOfPeriod`.

Examples:

Get the observations for `"GS10"` series without specifying any optional parameters.

*)
myFred.Series.Observations("GS10").Observations
|> Seq.truncate 3 (* output: 
val it: JsonProvider<...>.Observation seq =
  seq
    [{
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "date": "1953-04-01",
  "value": "2.83"
};
     {
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "date": "1953-05-01",
  "value": "3.05"
};
     {
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "date": "1953-06-01",
  "value": "3.11"
}]*)
(**
Get the observations for `"DTP10J25"` with a semi-annual frequency.

*)
myFred.Series.Observations("DTP10J25", frequency = Frequency.Semiannual).Observations
|> Seq.truncate 3 (* output: 
val it: JsonProvider<...>.Observation seq =
  seq
    [{
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "date": "2015-01-01",
  "value": "."
};
     {
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "date": "2015-07-01",
  "value": "0.624"
};
     {
  "realtime_start": "2023-11-13",
  "realtime_end": "2023-11-13",
  "date": "2016-01-01",
  "value": "0.315"
}]*)

