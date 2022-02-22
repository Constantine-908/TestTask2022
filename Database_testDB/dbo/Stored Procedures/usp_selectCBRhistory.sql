
CREATE PROCEDURE [dbo].[usp_selectCBRhistory]
@NumCodeID int=0,
@CharCode char(3)='000',
@DtStart date=null,
@DtEnd date=null,
@Days int=60
AS

--usp_selectCBRhistory @CharCode='USD'

declare @dtStartLogging datetime =getdate(),  @msTime int=-1, @rowsCount int=-1

if @NumCodeID=0
select @NumCodeID =[NumCodeID] from [dbo].[tbl_currencyInfo]with (nolock) where [CharCode]=@CharCode
if @dtEnd is null set @dtEnd=getdate()
if @dtStart is null set @dtStart=dateadd(day,@days*-1,@dtend)

--select @NumCodeID,@dtEnd,@dtStart
select [date_req] as [Date],[value] as [Value],[Nominal] from [dbo].[tbl_dailyCBR] with (nolock)
where [NumCodeID]=@NumCodeID
and [date_req] between @dtStart and @dtEnd
union all
select nr.[dtRequest] as [Date],[value] as [Value],[Nominal] from [dbo].[tbl_dailyCBR] dr with (nolock) 
inner join [dbo].[tbl_noRatesDates] nr with (nolock) on dr.date_req=nr.dtRate
where [NumCodeID]=@NumCodeID and [dtRequest] between @dtStart and @dtEnd
order by [date_req]
set @rowsCount=@@ROWCOUNT

set @msTime=DATEDIFF(millisecond,@dtStartLogging,getdate())
INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime],rowsCount)
     VALUES (user_id(system_USER),@@PROCID,@msTime,@rowsCount)
GO
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_selectCBRhistory] TO [cbrReader]
    AS [dbo];

