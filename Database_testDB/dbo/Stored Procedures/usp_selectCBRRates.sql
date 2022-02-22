

CREATE PROCEDURE [dbo].[usp_selectCBRRates]
@date_req date
as
--[usp_selectCBRRates] '2021-01-01'

-- for loging
declare @dtStartLogging datetime =getdate(),  @msTime int=-1, @rowsCount int=-1

declare @count1 int=0
declare @count2 int=0

select @count1 =count([dtRequest]) from [dbo].[tbl_noRatesDates] (nolock) where [dtRequest]=@date_req
select @count2 =count([date_req]) from [dbo].[tbl_dailyCBR] (nolock) where [date_req]=@date_req
if (@count1=0 and @count2=0) 
exec [dbo].[usp_fillDyliCBR] @date_req

begin tran
if exists(select * from [dbo].[tbl_noRatesDates] where [dtRequest]=@date_req) --notworkingday -> change to last working day
select  @date_req =[dtRate] from [dbo].[tbl_noRatesDates] where [dtRequest]=@date_req
select  [date_req] as [date],[idCur] as ID ,[NumCode],[nameCur] as [Name] ,[CharCode],cast([value] as varchar(10)) as [Value], cast([Nominal] as varchar(10)) as [Nominal]
from [dbo].[tbl_dailyCBR] (nolock) as tr
inner join [dbo].[tbl_currencyInfo] (nolock) as ti on ti.[NumCodeID]=tr.[NumCodeID]
where [date_req] =@date_req
set @rowsCount=@@ROWCOUNT
--WAITFOR DELAY '00:00:05.03'
commit tran

--logging

set @msTime=DATEDIFF(millisecond,@dtStartLogging,getdate())
INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime],rowsCount)
     VALUES (user_id(system_USER),@@PROCID,@msTime,@rowsCount)

GO
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_selectCBRRates] TO [cbrReader]
    AS [dbo];

