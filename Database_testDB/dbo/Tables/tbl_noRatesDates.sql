CREATE TABLE [dbo].[tbl_noRatesDates] (
    [dtRequest] DATE NOT NULL,
    [dtRate]    DATE NOT NULL,
    CONSTRAINT [PK_tbl_noRatesDates] PRIMARY KEY CLUSTERED ([dtRequest] ASC, [dtRate] ASC)
);

