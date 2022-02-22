
  CREATE procedure [dbo].[usp_status]
as
--RAISERROR('error from SQL',16,1) to test error handler
--used for web api to test curent user  StatusController
   -- WAITFOR DELAY '00:00:05'; 
select @@SERVERNAME, SYSTEM_USER  

GO
GRANT EXECUTE
    ON OBJECT::[dbo].[usp_status] TO [cbrReader]
    AS [dbo];

