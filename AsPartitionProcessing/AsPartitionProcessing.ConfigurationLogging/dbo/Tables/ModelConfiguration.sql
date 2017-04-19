CREATE TABLE [dbo].[ModelConfiguration] (
    [ModelConfigurationID]     INT           NOT NULL,
    [AnalysisServicesServer]   VARCHAR (255) NOT NULL,
    [AnalysisServicesDatabase] VARCHAR (255) NOT NULL,
    [InitialSetUp]             BIT           NOT NULL,
    [IncrementalOnline]        BIT           NOT NULL,
    [IntegratedAuth]           BIT           NOT NULL,
    [MaxParallelism]           INT           NOT NULL,
    [CommitTimeout]            INT           NOT NULL,
    [RetryAttempts]			   TINYINT NOT NULL, 
    [RetryWaitTimeSeconds]     INT NOT NULL, 
    CONSTRAINT [PK_ModelConfiguration] PRIMARY KEY CLUSTERED ([ModelConfigurationID] ASC)
);

