/*============================================================================
  Summary: Contains class implementiong xEvent data logging for Azure Analysis Services
  Copyright (C) Microsoft Corporation.  


  This source code is intended only as a supplement to Microsoft
  Development Tools and/or on-line documentation. 

  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/
using System;
using System.Xml;
using System.Threading;
using System.IO;
using System.Text;
using Microsoft.AnalysisServices; 
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.SqlServer.XEvent.Linq;        // Referenced from the GAC version of Microsoft.SqlServer.XEvent.Linq

namespace TracingSample
{
    public class Worker
    {
        private string UserName; 
        private string AsServer; 
        private string AsDatabase; 
        private string eventTmsl; 
        private string logFile;

        public Worker(string user, string server, string db, string events, string log)
        {
            UserName = user;
            AsServer = server;
            AsDatabase = db;
            eventTmsl = events;
            logFile = log;
        }

        // This method will be called when the thread is started. 
        public void DoWork()
        {
            try
            {
                using (Server server = new Server())
                {
                    //Connect and get main objects
                    string serverConnectionString;

                    // Assume integratedAuth
                    // otherwise serverConnectionString = $"Provider=MSOLAP;Data Source={AsServer};User ID={UserName};Password={Password};Impersonation Level=Impersonate;";
                    serverConnectionString = $"Provider=MSOLAP;Data Source={AsServer};Integrated Security=SSPI";
                    server.Connect(serverConnectionString);

                    Database database = server.Databases.FindByName(AsDatabase);

                    if (database == null)
                    {
                        throw new Microsoft.AnalysisServices.ConnectionException($"Could not connect to database {AsDatabase}.");
                    }

                    //Register the events you want to trace
                    string queryString = System.IO.File.ReadAllText(eventTmsl);
                    server.Execute(queryString);

                    // Now you need to subscribe to the xEvent stream and execute a data reader
                    // You need to have the same name as in the TMSL file!
                    // NOTE calls to the reader will block until new values show up!
                    string sessionId = "SampleXEvents";
                    AdomdConnection conn = new AdomdConnection(serverConnectionString);
                    conn.Open();

                    var cmd = conn.CreateCommand();

                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.CommandText =
                    "<Subscribe xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" +
                        "<Object xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" +
                            "<TraceID>" + sessionId + "</TraceID>" +
                         "</Object>" +
                    "</Subscribe>";

                    XmlReader inputReader = XmlReader.Create(cmd.ExecuteXmlReader(), new XmlReaderSettings() { Async = true });

                    //Connect to this with QueryableXEventData
                    using (QueryableXEventData data =
                        new QueryableXEventData(inputReader, EventStreamSourceOptions.EventStream, EventStreamCacheOptions.CacheToDisk))
                    {

                        using (FileStream fs = new FileStream(logFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            //Write out the data in a long format for illustration
                            // this could would be adpated for your specific needs
                            foreach (PublishedEvent evt in data)
                            {
                                StringBuilder s = new StringBuilder(); 
                                s.Append($"Event: {evt.Name}\t");
                                s.Append(Environment.NewLine);
                                s.Append($"Timestamp: {evt.Timestamp}\t");
                                s.Append(Environment.NewLine);

                                foreach (PublishedEventField fld in evt.Fields)
                                {
                                    s.Append($"Field: {fld.Name} = {fld.Value}\t");
                                    s.Append(Environment.NewLine);
                                }

                                foreach (PublishedAction act in evt.Actions)
                                {
                                    s.Append($"Action: {act.Name}  = {act.Value}\t");
                                    s.Append(Environment.NewLine);
                                }
                                s.Append(Environment.NewLine);
                                
                                //Write the data to a log file
                                // the format and sink should be changed for your proposes
                                byte[] bytes = Encoding.ASCII.GetBytes(s.ToString().ToCharArray());
                                fs.Write(bytes, 0, s.Length);

                                if (_shouldStop == true)
                                {
                                    break;
                                }
                                // Writing a . to show progress
                                Console.Write(".");
                                
                                //Uncomment this to output to the Console
                                //Console.WriteLine(s);

                            }
                            //TODO stop the trace !
                            fs.Close();
                        }
                        conn.Close();

                        //clean up the trace on exit -- or you can keep it running
                        //var stopCommand = conn.CreateCommand();

                        //stopCommand.CommandType = System.Data.CommandType.Text;
                        var stopCommand = 
                            "<Execute xmlns = \"urn:schemas-microsoft-com:xml-analysis\">" +
                                "<Command>" +
                                    "<Batch …>" +
                                        "<Delete …>" +
                                            // You need to have the same name as in the TMSL file!
                                            "<Object><TraceID>"+sessionId+"</TraceID></Object>" +
                                        "</Delete>" +
                                    "<Batch …>" +
                                "<Command>" +
                                "<Properties></Properties>" +
                            "</Execute>";

                        server.Execute(queryString);
                        server.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: handle exceptions :-)
                Console.WriteLine(e.ToString());
                Console.WriteLine("There was an error. Verify the command-line parmaters. Press any key to exit.");
            }
            //Worker thread: terminating gracefully.

        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Volatile is used as hint to the compiler that this data 
        // member will be accessed by multiple threads. 
        private volatile bool _shouldStop;
    }

    class Program
    {
        static void Main(string[] args)
        {
            string UserName = "user@contoso.com";
            string AsServer = "asazure://region.asazure.windows.net/myinstance";
            string AsDatabase = "SalesBI";
            string eventTmsl = @"C:\AsXEventSample\eventTmsl.xmla"; // location of the xEvents TMSL file you want to collect, you can create this with SSMS script out
            string logFile = @"C:\AsXEventSample\aslog.txt";        //location of the outputfile

            // Using a thread here as data comes in asychronously 
            // Create the thread object. This does not start the thread.
            Worker workerObject = new Worker(UserName, AsServer, AsDatabase, eventTmsl, logFile);
            Thread workerThread = new Thread(workerObject.DoWork);
                
            // Start the worker thread.
            workerThread.Start();

            // For monitoring, this would be upgraded to a windows service
            Console.WriteLine("Listening for trace events which are sent when there is trace activity.");
            Console.WriteLine("Type the letter q and enter to quit.  The process will exit after the next trace event is received.");

            bool cont = true;
            while (cont)
            {
               Thread.Sleep(1);
               if (ConsoleKey.Q == Console.ReadKey().Key)
               {
                        workerObject.RequestStop();
                        Console.WriteLine("\nStopping reader on next trace event received...");
                        cont = false;
                }
             }

             //wait for the worker to exit
             workerThread.Join();
             Console.WriteLine($"File is stored at: {logFile}");
      }
   }
}       

