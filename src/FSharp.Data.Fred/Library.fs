namespace FSharp.Data.Fred

open FSharp.Data


/// <summary>
///   A module
/// </summary>
///
/// <namespacedoc>
///   <summary>A namespace to remember</summary>
///
///   <remarks>More on that</remarks>
/// </namespacedoc>
///
[<RequireQualifiedAccess>]
module Fred =
    type Series = CsvProvider<Sample="Date (date),Value (float)",MissingValues=".">
    let private fredUrl series = $"https://fred.stlouisfed.org/graph/fredgraph.csv?id={series}"
    
    ///<summary>Loads a FRED data series as a <c>FSharp.Data.CsvFile</c></summary>
    /// <param name="series">The series name such as GS10, EXUSEU, etc.</param>
    let Load (series:string) =  
        // I'm using "Load" instead of "load" to mimic CsvFile.Load
        Series.Load(fredUrl series)
    