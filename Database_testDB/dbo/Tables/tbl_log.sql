CREATE TABLE [dbo].[tbl_log] (
    [LogID]     INT           IDENTITY (1, 1) NOT NULL,
    [UserID]    INT           NULL,
    [ProcID]    INT           NULL,
    [dt]        SMALLDATETIME CONSTRAINT [DF_tbl_log_dt] DEFAULT (getdate()) NULL,
    [mstime]    INT           NULL,
    [rowscount] INT           CONSTRAINT [DF_tbl_log_rowscount] DEFAULT ((-1)) NULL,
    CONSTRAINT [PK_tbl_log] PRIMARY KEY CLUSTERED ([LogID] ASC)
);

