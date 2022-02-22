CREATE procedure [dbo].[usp_fillDyliCBR]
@date date 
--usp_fillDyliCBR '2022-02-15'
with execute as owner
as

-- for loging
declare @dtStart datetime =getdate();
declare @msTime int=-1

if @date >getdate() 
begin -- if request is incorrect do nothing 
		--logging
		 INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime])  VALUES (user_id(system_USER),@@PROCID,DATEDIFF(millisecond,@dtStart,getdate()))
	print 'incorrect date'
	return
end

if exists(select top(1) [dtRequest] from [dbo].[tbl_noRatesDates] where [dtRequest]=@date)
begin 	--if exist the record it means that records in [tbl_dailyCBR] exists as well
		--logging
		 INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime])  VALUES (user_id(system_USER),@@PROCID,DATEDIFF(millisecond,@dtStart,getdate()))
	print 'records exist 1'
	return
end
declare @hDoc INT
declare @xml xml
declare @Object as Int;
declare @ResponseText as Varbinary(8000);
declare @Url as Varchar(MAX);
declare @usd float;
declare @eur float;
declare @dtRate datetime;
if not exists (select [date_req] from [dbo].[tbl_dailyCBR] where [date_req]= @dtRate)
begin
	select @Url = 'http://www.cbr.ru/scripts/XML_daily.asp?date_req='+convert(varchar(100), @date, 103)
	Exec sp_OACreate 'MSXML2.XMLHTTP', @Object OUT;
	Exec sp_OAMethod @Object, 'open', NULL, 'get', @Url, 'false'
	Exec sp_OAMethod @Object, 'send'
	Exec sp_OAMethod @Object, 'responsebody', @ResponseText OUTPUT
	Exec sp_OADestroy @Object
	SELECT @xml = cast (@ResponseText as xml)
	EXEC sp_xml_preparedocument @hDoc OUTPUT,@xml
	
	SELECT @dtRate= convert(date, dt ,104) FROM
	OPENXML(@hDoc, '//ValCurs')
	WITH(dt nvarchar(100) './@Date')

	DROP TABLE IF EXISTS #T_XML
	SELECT			
		@dtRate dt,
		[id],
		[NumCode],
		cast([NumCode] as int)  NumCodeID,
		[CharCode],
		[Nominal],			
		[name],
		cast(replace(value, ',','.') as decimal(8,4)) [value]
	INTO #T_XML
	FROM
		OPENXML(@hDoc, '//Value')
		WITH
		(
		id nvarchar(100) '../@ID',
		name nvarchar(100) '../Name',
		value nvarchar(100) '../Value',
		CharCode nvarchar(10) '../CharCode',
		NumCode nvarchar(10) '../NumCode',
		Nominal int '../Nominal',
		[nameCur] nvarchar(1000) '../Name'		
		)
	WHERE NumCode is not null
	EXEC sp_xml_removedocument @hDoc

	if (@dtRate is null or @dtRate>@date)
	begin
		print 'error http response'
		--logging
	 INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime])  VALUES (user_id(system_USER),@@PROCID,DATEDIFF(millisecond,@dtStart,getdate()))
		return
	end
	IF @dtRate=@date --requested and given dates is same
	BEGIN
	--no records and we have XML for current date
		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
		BEGIN TRANSACTION
			if not exists (select [date_req] from [dbo].[tbl_dailyCBR] where [date_req]= @dtRate)-- check if during http request other transaction already inserted reccords
			BEGIN 
				INSERT INTO [dbo].[tbl_dailyCBR]
					([NumCodeID] ,[value] ,[date_req],[Nominal])
				SELECT NumCodeID, [value], dt, Nominal
				FROM #T_XML	

				print @@rowcount

				--add new currency info
				INSERT INTO [dbo].[tbl_currencyInfo]
				SELECT 
					#T_XML.NumCodeID, #T_XML.NumCode, #T_XML.CharCode, #T_XML.id, #T_XML.[name]
				FROM  #T_XML
				LEFT JOIN  [dbo].[tbl_currencyInfo]  ON [dbo].[tbl_currencyInfo].NumCodeID = #T_XML.NumCodeID
				WHERE [dbo].[tbl_currencyInfo].NumCodeID IS NULL

			END
		COMMIT TRANSACTION
		--logging
		 INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime])  VALUES (user_id(system_USER),@@PROCID,DATEDIFF(millisecond,@dtStart,getdate()))
		return
	end
	
	else begin -- fill [dbo].[tbl_noRatesDates]
		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
		BEGIN TRANSACTION
			if not exists (select [date_req] from [dbo].[tbl_dailyCBR] where [date_req]= @dtRate) -- check if during http request other transaction already inserted reccords
			begin 
				INSERT INTO [dbo].[tbl_dailyCBR] ([NumCodeID] ,[value] ,[date_req],[Nominal])
				SELECT NumCodeID, [value], dt, Nominal
				FROM #T_XML	
				print @@rowcount
			end
			if not exists(select * from [dbo].[tbl_noRatesDates] where [dtRequest]=@date and [dtRate]=@date )
				INSERT INTO [dbo].[tbl_noRatesDates] ([dtRequest] ,[dtRate]) VALUES (@date,@dtRate)

			--add new currency info
			INSERT INTO [dbo].[tbl_currencyInfo]
			SELECT #T_XML.NumCodeID, #T_XML.NumCode, #T_XML.CharCode, #T_XML.id, #T_XML.[name]
			FROM  #T_XML
			LEFT JOIN  [dbo].[tbl_currencyInfo]  ON [dbo].[tbl_currencyInfo].NumCodeID = #T_XML.NumCodeID
			WHERE [dbo].[tbl_currencyInfo] .NumCodeID IS NULL

		COMMIT TRANSACTION
		--logging
		 INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime])  VALUES (user_id(system_USER),@@PROCID,DATEDIFF(millisecond,@dtStart,getdate()))
	end
		DROP TABLE IF EXISTS #T_XML

	

end
else print 'records exist 2'
		--logging
		 INSERT INTO [dbo].[tbl_log] ([UserID],[ProcID],[mstime])  VALUES (user_id(system_USER),@@PROCID,DATEDIFF(millisecond,@dtStart,getdate()))

