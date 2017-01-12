# AS Trace
SQL Server ships with a tool called SQL Server Profiler that is able to capture trace events from a SQL Server Analysis Services and write those events to a SQL Server table. However, the Profiler GUI consumes unnecessary memory and processor power capturing the trace events and displaying them on the screen. For constant monitoring and logging, the ASTrace tool will capture a Profiler trace and write it to a SQL Server table without requiring a GUI. ASTrace also runs as a Windows service allowing it to restart automatically when the server reboots.

Alternatives to ASTrace include [server side traces](http://blogs.msdn.com/b/karang/archive/2012/09/10/9916124.aspx) (and then loading those .trc files into SQL Server with a [PowerShell script](http://www.bp-msbi.com/2012/02/counting-number-of-queries-executed-in-ssas/) after the fact) and Xevents. 

ASTrace is a great way to capture the text and duration of MDX queries, to log details about processing failures, and to troubleshoot SSAS issues such as locking.

ASTrace 2.0 adds the ability for one ASTrace service to capture events from multiple Analysis Services instances at once.
