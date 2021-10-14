module internal EmbeddedResources


let [<Literal>] KeyFileSample = """ 
{ "fredKey": "key here in the actual file to be named fredKey.json"} 
"""

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
    ]}
"""

let [<Literal>] SeriesCategoriesSample = """
{
    "categories": [
        {
            "id": 95,
            "name": "Monthly Rates",
            "parent_id": 15
        },
        {
            "id": 275,
            "name": "Japan",
            "parent_id": 158
        }
    ]
}
"""

let [<Literal>] SeriesReleaseSample= """
{
    "realtime_start": "2013-08-14",
    "realtime_end": "2013-08-14",
    "releases": [
        {
            "id": 21,
            "realtime_start": "2013-08-14",
            "realtime_end": "2013-08-14",
            "name": "H.6 Money Stock Measures",
            "press_release": true,
            "link": "http://www.federalreserve.gov/releases/h6/"
        }
    ]
}
"""

let [<Literal>] SeriesTagsSample = """
{
    "realtime_start": "2013-08-14",
    "realtime_end": "2013-08-14",
    "order_by": "series_count",
    "sort_order": "desc",
    "count": 8,
    "offset": 0,
    "limit": 1000,
    "tags": [
        {
            "name": "nation",
            "group_id": "geot",
            "notes": "Country Level",
            "created": "2012-02-27 10:18:19-06",
            "popularity": 100,
            "series_count": 105200
        },
        {
            "name": "stlfsi",
            "group_id": "rls",
            "notes": "St. Louis Financial Stress Index",
            "created": "2012-08-16 15:21:17-05",
            "popularity": 66,
            "series_count": 1
        }
    ]
}
"""