CREATE PROC [dbo].[usp_LastProcessingLogs] AS
	SELECT [Message]
	FROM [dbo].[ProcessingLog]
	WHERE ExecutionID = 
	(	SELECT MAX([ExecutionID]) FROM [dbo].[ProcessingLog]
		WHERE [LogDateTime] = (SELECT MAX([LogDateTime]) FROM [dbo].[ProcessingLog])
	)
	ORDER BY [LogDateTime]