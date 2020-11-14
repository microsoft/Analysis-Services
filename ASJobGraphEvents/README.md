# Summary

The Job Graph events are emitted by AS Engine before and after any processing (aka refresh) commands. Both job graphs show the order in which processing jobs execute within the sequence point algorithm, in the Analysis Services engine. The graph associated with the GraphEnd (2) Subclass is annotated, in that it contains the "Blocked duration", "Waiting duration", "Running duration" etc for each job. In addition, the annotated graph shows the critical path. Job Graph events can be used to identify bottlenecks in processing jobs, by highlighting the critical path.

In the cloud instances of Analysis Services, such as Azure Analysis Services or Power BI, the job graph is broken down into smaller chunks, each in their own event. The events can be reassembled with this script.

## Critical Path

When the job graph executes, there is always a job that finishes last before the engine can commit the change. This job that finishes last is the "critical dependency" for the commit; the entire commit needs to wait for this one job to finish before executing. This last job depends on other jobs, one of which finished after all the others. This is the next critical dependency. Tracing this path of critical dependencies forms the critical path, which helps engineers and customers identify why processing takes so long.

The critical path is shown via dark backgrounds and white text in the job graph file.

# Rebuilding the DGML file

## Requirements

* Python 3.8 or later
* Visual Studio

## Usage

1. Start a trace in SQL Server Profiler and select "Job Graph Events".
2. Initate processing (eg. "Process Full" in SQL Server Management Studio).
3. Wait for the processing to finish
4. Once the processing is finished, wait for all job graph events to show up in SQL Profiler. NOTE : The first event in a subclass is always the metadata, which contains, among other things, the number of additional events that'll follow.
5. Save all the events in a trace file, `File > Save As > Trace XML File` 
6. Rebuild the annotated job graph by aiming the script `rebuild.py` at this file like so:

```bash
python rebuild.py path\to\trace.xml output_folder
```

7. Inside `output_folder` there will be two .DGML files, which can be opened in Visual Studio.

# Diagnosing from DGML

Once the Job Graph is rendered, look for the critical path (darker nodes) or for failed jobs (white with a dashed border). Failed jobs include an "Error Code" that specifies why the job failed.

To diagnose slow refresh times, look for the critical path and start at the top. Look at "Blocked duration", "Waiting duration", and "Running duration". If a job has a long blocked duration, it spent a long time waiting on other jobs. If a job has a long waiting duration, it was waiting for an available thread, so increasing maxParallelism could help. If a job has a long running duration, then the model might have to be changed to speed up the job.

# Creating a Gantt Chart

The Gantt Chart can also come in handy when diagnosing processing issues. Power BI Desktop provides first party and 3rd party Gantt Chart visuals, that can be used to visualize the job graph.

We have also provided a script to generate a Gantt Chart as a simple HTML file. Please follow the below instructions to generate the HTML

## Requirements

* Python 3.8 or later
* A valid job graph DGML file (from above)

## Usage

1. Get the reconstructed DGML file corresponding to the annotated graph, i.e the GraphEnd (2) Subclass
2. Run `gantt\script.py` like so:

```bash
python gantt\script.py path\to\file.dgml output_folder
```

3. Inside `output_folder` there will be an .html file that can be opened in a browser.
