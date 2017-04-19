CREATE TABLE [dbo].[ProcessingLog] (
    [PartitioningLogID]    INT            IDENTITY (1, 1) NOT NULL,
    [ModelConfigurationID] INT            NOT NULL,
    [ExecutionID]          CHAR (36)      NOT NULL,
    [LogDateTime]          DATETIME       NOT NULL,
    [Message]              VARCHAR (8000) NOT NULL,
    [MessageType]              NVARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_ProcessingLog] PRIMARY KEY CLUSTERED ([PartitioningLogID] ASC),
    CONSTRAINT [FK_ProcessingLog_ModelConfiguration] FOREIGN KEY ([ModelConfigurationID]) REFERENCES [dbo].[ModelConfiguration] ([ModelConfigurationID])
);

