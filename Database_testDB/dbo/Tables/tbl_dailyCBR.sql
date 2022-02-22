CREATE TABLE [dbo].[tbl_dailyCBR] (
    [id]        INT            IDENTITY (1, 1) NOT NULL,
    [NumCodeID] INT            NOT NULL,
    [value]     DECIMAL (8, 4) NOT NULL,
    [date_req]  DATE           NOT NULL,
    [Nominal]   INT            NOT NULL,
    CONSTRAINT [PK_tbl_dailyCBR] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [date_req_index]
    ON [dbo].[tbl_dailyCBR]([date_req] ASC);


GO
CREATE NONCLUSTERED INDEX [NumCodeID_index]
    ON [dbo].[tbl_dailyCBR]([NumCodeID] ASC);

