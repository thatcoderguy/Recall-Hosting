Imports System.Threading
Imports System.Diagnostics

Public Class Service1

    Private arrWorkers(20) As worker
    Private objThreads(20) As Thread
    Private blnShutdown As Boolean
    Private objConfig As config
    Private objLog As Diagnostics.EventLog
    Private intCurrentThread As Integer

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        Dim objWorker As worker
        Dim index As Integer
        objLog = New Diagnostics.EventLog()

        If Not EventLog.SourceExists("RecallHostingService") Then
            EventLog.CreateEventSource("RecallHostingService", "RecallHostingService_log")
        End If

        blnShutdown = False

        ''used to keep track of the array index (used later)
        intCurrentThread = 0

        ''first open the config file
        objConfig = New config("C:\Program Files\WidaHost\WorkHorseService\config.txt")

        ''load up array with worker clases
        For index = 0 To 20

            ''create new instance of worker
            objWorker = New worker(objConfig)

            ''put instance of a worker into the array
            arrWorkers(index) = objWorker

            ''set up a thread with the worker method
            objThreads(index) = New Thread(AddressOf arrWorkers(index).doWork)
            objThreads(index).IsBackground = True

        Next

        ''for some really crappy reason, a timer has to be used... 
        ''otherwise the system, doesnt register that the service has started
        Timer1.Interval = 100
        Timer1.Enabled = True

    End Sub

    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.

        Dim objWorker As worker

        Dim index As Integer
        blnShutdown = True
        Timer1.Enabled = False

        For index = 0 To 20

            ''cast the worker object
            objWorker = CType(arrWorkers(index), worker)

            'tell the worker do finish what it is doing, then stop
            objWorker.fireWorker()

            ''don't do anything while the worker is working
            While objWorker.isWorking

            End While

            ''now that the worker has finished, kill the thread
            objThreads(index).Abort()

        Next

    End Sub

    Private Sub Timer1_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles Timer1.Elapsed
        Dim trace As Diagnostics.StackTrace

        EventLog.Source = "RecallHostingService"

        ''rather than use a for... loop, which can cause issues (esp with concurrency)
        ''it's easier just increment an integer and use that as the array index.
        ''That way, we dont have an issue with the for... loop stalling or not finishing
        ''before the next elapsed event is called

        ''main loop
        Try

            ''if the thread is available
            If Not objThreads(intCurrentThread).IsAlive Then

                objThreads(intCurrentThread) = New Thread(AddressOf arrWorkers(intCurrentThread).doWork)
                objThreads(intCurrentThread).IsBackground = True

                ''tell the thread to "GET BACK TO WORK!"
                objThreads(intCurrentThread).Start()

            End If

            intCurrentThread += 1

            If intCurrentThread = 21 Then intCurrentThread = 0

        Catch ex As Exception

            EventLog.WriteEntry("RecallHostingService_log", ex.StackTrace & "||" & ex.Source & "||" & CStr(intCurrentThread) & "||" & Err.Description, EventLogEntryType.Information)

            trace = New System.Diagnostics.StackTrace(ex, True)
            EventLog.WriteEntry("RecallHostingService_log", trace.GetFrame(0).GetMethod().Name, EventLogEntryType.Information)
            EventLog.WriteEntry("RecallHostingService_log", "Line: " & trace.GetFrame(0).GetFileLineNumber(), EventLogEntryType.Information)
            EventLog.WriteEntry("RecallHostingService_log", "Column: " & trace.GetFrame(0).GetFileColumnNumber(), EventLogEntryType.Information)

        End Try

    End Sub
End Class
