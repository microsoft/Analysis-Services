CREATE PROC [dbo].[usp_LastProcessingLogs] 
	@ExecutionCount tinyint = 1,
	@ErrorsOnly bit = 0
AS
	SELECT --l.ExecutionID,
		l.[LogDateTime],
		l.[Message]
	FROM [dbo].[ProcessingLog] l
	INNER JOIN 
	(	SELECT TOP (@ExecutionCount) [ExecutionID], MAX([LogDateTime]) [MaxLogDateTime]
		FROM [dbo].[ProcessingLog] 
		GROUP BY ExecutionID
		ORDER BY [MaxLogDateTime] DESC
	) dt ON l.ExecutionID = dt.ExecutionID
	WHERE @ErrorsOnly = 0 OR (@ErrorsOnly = 1 AND l.MessageType = 'Error')
	ORDER BY [LogDateTime]
