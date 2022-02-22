CREATE TABLE [dbo].[tbl_currencyInfo] (
    [NumCodeID] INT             NOT NULL,
    [NumCode]   CHAR (3)        NULL,
    [CharCode]  CHAR (3)        NULL,
    [idCur]     VARCHAR (10)    NULL,
    [nameCur]   NVARCHAR (1000) NULL,
    CONSTRAINT [PK_tbl_currencyInfo] PRIMARY KEY CLUSTERED ([NumCodeID] ASC)
);

