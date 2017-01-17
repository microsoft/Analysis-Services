AsPerfMon can be used to check real-time memory usage during processing. It splits memory usage by database, which is informative when multiple databases share the same server.

This is useful for Azure AS since you can’t use Task Manager or create Performance Monitor counters. Similar functionality is provided by the Metrics section in the control blade for an Azure AS server in the Azure Portal. By using Metrics, you can check usage for the past day or week. AsPerfMon is for real-time monitoring during processing.


![alt text](AsPerfMon.png "Stacked bar by memory allocation")

