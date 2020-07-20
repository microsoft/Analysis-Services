# Job Graph Events in Power BI

Job Graph events can be used to identify bottlenecks in data refreshes by highlighting the critical path. For instances of Analysis Services not running on-premise, the graph is broken into 16 Kb chunks, each in their own event. The events can be reassembled with this script. 

## Usage

1. Start a trace in SQL Server Profiler and select "Job Graph Events".
2. Start a data refresh ("Process Full" in SQL Server Management Studio).
3. Wait for all trace events to arrive in Profiler.
4. `File > Save As > Trace XML File` 
5. Aim `rebuild.py` at this file like so:

```bash
python rebuild.py path\to\trace.xml output_folder
```

6. Inside `output_folder` there will be two .DGML files, which can be opened in Visual Studio.
