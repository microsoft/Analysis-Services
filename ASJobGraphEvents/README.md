# Job Graph Events in Power BI / Azure Analysis Services

Job Graph events can be used to identify bottlenecks in data refreshes by highlighting the critical path. For instances of Analysis Services not running on-premise, the graph is broken into 16 Kb chunks, each in their own event. The events can be reassembled with this script. 

# Summary

The Job Graph event is emitted by AS Engine before and after processing. Both graphs show the order that jobs execute within the sequence point algorithm. The final (annotated) graph shows execution times for each job. In addition, the annotated graph shows the critical path.

## Critical Path

When the job graph executes, there is always a job that finishes last before the engine can commit the change. This job that finishes last is the "critical dependency" for the commit; the entire commit needs to wait for this one job to finish before executing. This last job depends on other jobs, one of which finished after all the others. This is the next critical dependency. Tracing this path of critical dependencies forms the critical path, which helps engineers and customers identify why processing takes so long.

The critical path is shown via dark backgrounds and white text in the job graph file.

# Rebuilding the DGML file

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

# Diagnosing from DGML

Now that the graph is open, look for the critical path (darker nodes) or for failed jobs (white with a dashed border). Failed jobs include an "Error Code" that specifies why the job failed.

To diagnose slow refresh times, look for the critical path and start at the top. Look at "Blocked duration", "Waiting duration", and "Running duration". If a job has a long blocked duration, it spent a long time waiting on other jobs. If a job has a long waiting duration, it was waiting for an available thread, so increasing maxParallelism could help. If a job has a long running duration, then the model might have to be changed to speed up the job.

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
