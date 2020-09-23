# Job Graph Events in Power BI

Job Graph events can be used to identify bottlenecks in data refreshes by highlighting the critical path. For instances of Analysis Services not running on-premise, the graph is broken into 16 Kb chunks, each in their own event. The events can be reassembled with this script. 

# Rebuilding the DGMl file

## Requirements

* Python 3.8 or later
* Visual Studio

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

# Creating a Gantt Chart

## Requirements

* Python 3.8 or later
* A valid job graph DGML file (from above)

## Usage

1. Get a .DGML file with all the anntoations (running duration, waiting duration, etc.)
2. Run `gantt\script.py` like so:

```bash
python gantt\script.py path\to\file.dgml output_folder
```

3. Inside `output_folder` there will be an .html file that can be opened in a browser.
