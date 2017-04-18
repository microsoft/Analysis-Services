CREATE TABLE [dbo].[TableConfiguration] (
    [TableConfigurationID]  INT           NOT NULL,
    [ModelConfigurationID]  INT           NOT NULL,
    [AnalysisServicesTable] VARCHAR (255) NOT NULL,
    [DoNotProcess]          BIT           CONSTRAINT [DF_TableConfiguration_DoNotProcess] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TableConfiguration] PRIMARY KEY CLUSTERED ([TableConfigurationID] ASC),
    CONSTRAINT [FK_TableConfiguration_ModelConfiguration] FOREIGN KEY ([ModelConfigurationID]) REFERENCES [dbo].[ModelConfiguration] ([ModelConfigurationID])
);

