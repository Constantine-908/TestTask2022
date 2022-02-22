CREATE ROLE [cbrReader]
    AUTHORIZATION [dbo];


GO
ALTER ROLE [cbrReader] ADD MEMBER [SRV-2022\webUser1];


GO
ALTER ROLE [cbrReader] ADD MEMBER [SRV-2022\webUser2];


GO
ALTER ROLE [cbrReader] ADD MEMBER [SRV-2022\webUser3];


GO
ALTER ROLE [cbrReader] ADD MEMBER [SRV-2022\webUser4];

