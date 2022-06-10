CREATE TABLE [dbo].[Functions] (
    [FunctionId] [int] NOT NULL IDENTITY,
    [FatherFunctionId] [int],
    [PageTranslation] [varchar](8000),
    [KeyTranslation] [varchar](8000),
    [Description] [varchar](8000),
    [Icon] [varchar](8000),
    [Url] [varchar](8000),
    [Order] [int] NOT NULL,
    [Enabled] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Functions] PRIMARY KEY ([FunctionId])
)
CREATE TABLE [dbo].[Roles] (
    [RoleId] [int] NOT NULL IDENTITY,
    [RoleDescription] [varchar](20),
    CONSTRAINT [PK_dbo.Roles] PRIMARY KEY ([RoleId])
)
CREATE TABLE [dbo].[Users] (
    [UserId] [int] NOT NULL IDENTITY,
    [LoginName] [varchar](40),
    [Password] [varchar](40),
    [CodiceE1] [varchar](256),
    [Name] [varchar](80),
    [RoleId] [int] NOT NULL,
    [Enabled] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY ([UserId])
)
CREATE INDEX [IX_RoleId] ON [dbo].[Users]([RoleId])
CREATE TABLE [dbo].[Languages] (
    [LanguageID] [varchar](2) NOT NULL,
    [LanguageName] [varchar](25) NOT NULL,
    [LanguageEnabled] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Languages] PRIMARY KEY ([LanguageID])
)
CREATE TABLE [dbo].[Translations] (
    [IdTranslation] [int] NOT NULL IDENTITY,
    [Page] [varchar](40),
    [Key] [varchar](40),
    [LanguageID] [varchar](2) NOT NULL,
    [Value] [varchar](8000),
    CONSTRAINT [PK_dbo.Translations] PRIMARY KEY ([IdTranslation])
)
CREATE INDEX [IX_LanguageID] ON [dbo].[Translations]([LanguageID])
CREATE TABLE [dbo].[LogActions] (
    [LogActionID] [smallint] NOT NULL IDENTITY,
    [LogActionName] [varchar](8000),
    [LogAreaID] [smallint] NOT NULL,
    [ItemIdType] [varchar](20),
    [SecondItemIdType] [varchar](20),
    CONSTRAINT [PK_dbo.LogActions] PRIMARY KEY ([LogActionID])
)
CREATE TABLE [dbo].[LogAreas] (
    [LogAreaID] [smallint] NOT NULL IDENTITY,
    [LogAreaName] [varchar](50),
    [FilterName] [varchar](50),
    CONSTRAINT [PK_dbo.LogAreas] PRIMARY KEY ([LogAreaID])
)
CREATE TABLE [dbo].[Logs] (
    [LogID] [int] NOT NULL IDENTITY,
    [LogActionID] [varchar](10),
    [ItemID] [varchar](10),
    [SecondItemID] [varchar](10),
    [AuthorID] [varchar](128),
    [Message] [varchar](8000),
    [Data] [datetime] NOT NULL,
    [Country] [varchar](2),
    CONSTRAINT [PK_dbo.Logs] PRIMARY KEY ([LogID])
)
CREATE TABLE [dbo].[Functions_Roles] (
    [FunctionId] [int] NOT NULL,
    [RoleId] [int] NOT NULL,
    CONSTRAINT [PK_dbo.Functions_Roles] PRIMARY KEY ([FunctionId], [RoleId])
)
CREATE INDEX [IX_FunctionId] ON [dbo].[Functions_Roles]([FunctionId])
CREATE INDEX [IX_RoleId] ON [dbo].[Functions_Roles]([RoleId])
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_dbo.Users_dbo.Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
ALTER TABLE [dbo].[Translations] ADD CONSTRAINT [FK_dbo.Translations_dbo.Languages_LanguageID] FOREIGN KEY ([LanguageID]) REFERENCES [dbo].[Languages] ([LanguageID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Functions_Roles] ADD CONSTRAINT [FK_dbo.Functions_Roles_dbo.Functions_FunctionId] FOREIGN KEY ([FunctionId]) REFERENCES [dbo].[Functions] ([FunctionId]) ON DELETE CASCADE
ALTER TABLE [dbo].[Functions_Roles] ADD CONSTRAINT [FK_dbo.Functions_Roles_dbo.Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]) ON DELETE CASCADE
CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] [nvarchar](150) NOT NULL,
    [ContextKey] [nvarchar](300) NOT NULL,
    [Model] [varbinary](max) NOT NULL,
    [ProductVersion] [nvarchar](32) NOT NULL,
    CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201805140746229_001', N'Ica.SalesOrders.Models.Context.AppContext',  0x1F8B0800000000000400D55D596FE4B8117E0F90FF20E871E175DB9EF5606274EFC2EB2368EC786C4C7B16793368896E0BD1D19128C746905F9687FD49FB1742EAE42D524DB57B302F6D1E5FF1A82A168B559A3FFFF7C7FC97D724F65E605E4459BAF08F0F8F7C0FA6411646E97AE197E8E9C74FFE2F3FFFF52FF3AB3079F57E6FDB7D20ED70CFB458F8CF086DCE66B322788609280E9328C8B3227B42874196CC4098CD4E8E8EFE363B3E9E410CE1632CCF9B7F2D531425B0FA03FF7991A501DCA012C4375908E3A229C735AB0AD5FB0212586C400017FE791E15284B0FAFC2E8B06E7D88BB23F88A7CEF3C8E001ED10AC64FBE07D2344300E1F19E7D2BE00AE559BA5E6D700188EFDF3610B77B0271019B799CF5CD4DA7747442A634EB3BB6504189479858021E7F68D668C6771FB5D27EB7867815AFF06AA33732EB6A2517FE75990635344FECEC22CE4943E942B7DD0EBC65000E572086C56D1E62A6681A1C741C821989FC3BF02ECA1895395CA4B04439880FBCBBF2318E82DFE0DB7DF64F982ED2328EE9B1E2D1E23AA60017DDE5D906E6E8ED2B7CE266B00C7D6FC6F69FF1005D7749DF7AAECB147D38F1BD2F7830E031861D6B50EBB242590EFF0E53980304C33B8010CCF1CE2E43582DAE300A9E2640CF305752D677BE036B789F83B4881B86A8FB629EC662EA7B37E0F5334CD7E879E1E39FBE771DBDC2B02D6966F22D8DB054E34E282FE1103DB23B3B2477098B208F363BA1B50C7640E45B1E4F4EA312BC21FED5435CA5A471C78ABF66590C413A0CF305BC44EB8A3538C0AF18A0F0BDAFB0669CE239DAD4DAB8D31C0F4D93EB3C4BC8CF5E18EB9A875556E60126799F49ABEF41BE86881DD17CD6EB37ADD63BDF6CB0EAA98646D02C951FD77B0F742019C618FDD7F6DB95EE23F4CC64FCE4688C6428F9119FFBB99C1FA9CD248D1E6A7EE8B95256DF315FCB9BD2462D036FCBA104713C8792DE7BC0A164186338B4EDB72B0EFD9CADA394FCD4F0E64FA3785338C98BE2DF591E4E4DE7025BF001BC3AD6C9DAE94707840616ED938BC998E9AB7738EBDCA916FED8D3EA9F51AAE53348D725B6232D754ADB6D0F94493B94E5A5BD42A1FBAA25C288572D19AFA53C202827A7531277CEFDD4F5407EC0520D1E7AD6EBA540562F1CB0D2465B1DB0CCB5C64A10A89E7B200BCB909989AD3870DD7775CCDE554C30EDC987A73E3589F75326BF83B8D4ADE0C8ABA552CC7BC97525E2FC41A7D503E30EBA6C7D3EC6B9D5F5DB03F1EEC632EAACA33B77A27DFC716A0BBA263A70CEB9717E106A3904C3131C70062198605588119CDF49394A2B18646938153D2BD9C0EB36423270AF3D918B66DFC7488511CBB895094C7240224E5DF0D77514E3B1B9A764C359B82C7FB367ADAADB7EF0D648BE62786A7A4F05AFDC253B7DEC82A72A6D3539154A334E4EEBBC44CF59AEA773F2C901A11B58147A5BD7D1CB0940A025827FC3FB28B1372A2FB2B296DC2D6D5913BD715E1459105592207779D6EE1276BC5769E899F84EEAE14B7C307832586944A40C8F69E1FF202CC90089EE622A25517B785812C73EAF716ED34B184304BD5A7EF1BA832200A1B83D78E942B6042B299813FD00E20B6C8263B517A548D468511A441B101B4C84EB6BF1BA40C6D751E26B2EE106A64491196CD77643E828714B37B452F319C5817AC6E49EAC54FCA27ABFEA39A57F6C37E742C5B39739FFFDC0CBA3F1BCA5D73AD538F577BC7EB48CBBC17C19F45EA21EBEBF7EEE9B14EA666022026AA7A79524EAF669FB613897C6FAF020713DB807CCFBA3A20BF511AC4CAC601A43B3680E249EA908E40A224EBC0BDFEB8F2A416205CE64419AD76C014010CE019CE6A15287539F630338ED36C9B07A191900617DBB028ECEF1288CA7B556A503EA1D350630D5B5558A51D50C232886D0DC5AB8FE145FF620F2971DAAA9EE0588171A43A3A69B48C72482F819DA2E1452C3B6BCC663276DB0207C5487B814BA43D4E418A5064D09AC66091407E7049397BB3FC525183E4FCD4F546A1AAC946A56447B86528094EE305E9BD6CCEF347557379FD5A19B4DC17CA688F19CDF80CD06DF37A898CFA6C45BD5019F173FAEEC2320931A6316149240C86EB41D257C27AF5EC998DAEA151D5E477981C83DEB11903BCF459808CDA87349A1855A42FCD123EE5AABA0DA1EE477A37D9481AFFD79251EE50DCE359E5F42CC81CAFD20912AB1AB47A26F410C724DF4E4451697493A1491A94513E222194CA1D61C59089AA481854A735C3E389286E5EBCC5199F8281A92A930C7ABE31C69A0BAC41CA10A62A401AA02F3FE4D80228DD014996374EFD6344A5728E2CC671CBF0BA6AA206182D5CF8AAC9140AB0EB751C2CCDB8EF6323D88A05AEEF6BE4DAFB6EA0EAE4751B2B350B9379B589B57EE37B132DCB7DA443982526E9BD8394674A571783A142A228E06A28A6D34711BF4C6AAE0B6D41CA90F6BA391FA527324716AB6B372232CDF8986EB8D434702A2B2840D2443DD55C9CC94F782E1668D5763184D221D4C8D3DA2941984CABD610AE60AE2882F68F3C99E35B4BD95A652A8B4E7B461563ACC3AD289B7382DCD4CC1B61CC3536E78BE093CA2819AA2BDE146CAD7E44A47750EAA114A4ADD5773E4F64FBBDCA1AB8A0632C2931EE374951D661345C1E349E332745874340C237454B9399A186143638AB57BC5B7C485E990692570862C2BEDB90B666062652468B68C4A07C4301E05AA7C9F58C0A5D2AA5DDAA31840D155B36B92DD1FA9A61CA9BD369444502956286C588A42955821F6C127345A5F6A8ED44597D0405DA185CFA98A1E619C4D5589CD7DAC091D61AF63A5EC4DA5429958BE5847B5DCEBDA3E56587956DB4E2AF7E980CB85B8E3156E54F6C1435C31673ED8EA45BBC5B11E9FF2C97AEBBBB17A5CFCC383B8E5C2FB03DFA463B8EE1D827B6F9837BEFFE10F4F088F017513DFC3637F8942F210B07A2BB05A38240D0E57FF8A2FE208B378DFE006A4D1132C501DA7E89F1C1D9F70DFACD89FEF47CC8A228C256F27B28F48B09BF60E9F6F88C82A0F06565A86C5A9BED85011EBB1CCB23EA5DF6F780179F00C722E61F2E8C8165EFEB90657E892CC6D57D0F4C7185C6152DF5E7005C97C6A81DBFE31C9A78FD110C4E84F1A8C1045375F11984404151F0E90EDEBC9C0AE8E4EC11FB1A26EB2DE27595121D15DB6963F594B089FD6EE06954F6297EEFBE9475BD8A1E97FB21EA8441484F8B4651AC2D785FF9FAACB99B7FCC743DDEBC0ABD4CB9977E4FD778F148B22F2F31D12B5A5BBEE240D5BCE4FAE92AC1D6E86D2ADFD7EC9C29368273A3FD88D0AA1B281DD006AF8D340E4FBDEACD86FC5DA4C52F0389B674472ED18BDE038ABB548401C4F754C8AD9ACAECC492113B19FC7D6F9AB634C23115795AD3AA9E125753FEF3E2D745AA6E2D241650B7A6ABD5D62F2E7185CFB54CF713BB57592E5B412AF373C8EEDEFB84C66A31B4C59CEA41B643E43528A4A5224ED60B97C4867AE092AFD31C47C801CA43F0E9A9B5B488E91B7785B079AD34BBEC2FD6560EAF43DB7BAE1ECE86A357DBE6A9D2CF24E09A4CA1491EDB266B7CD4B55C4B7EE2217F57BCB3F953E1049F25E8C324F27E22D957A930DC92E7D752CAF6912232C55AA39A3193E098E18C2EE994EF6EAB7D7EA6DFF58F09DD4DD365CF8AE6ACF2AFDBCCFBBDB7532B834BF6E7CCAFB28D6D2C450EF2291FCFB491C17F3F8F8CD94E7866B53C3EBE77F7CF178CCF0BED796A53247552021281F8192D04246509ECCAA21561B763A62750B19316902B232075D9B822E835727A0F22418D112A830B53242BAD459750ABB3E835D3A212AFB7DFB0C7715055C69845FFB6C7439F00A0AC3E8CA1355293E46CCDD49D3839CCDDF29499F9E9A34DF4E978DAD104569CAD7F639EA8AACFAA13C7D45349AF4BA21CB25D62DC000AF68A3EA9C2F489B063FB820F2F0B709F8E37D9767A26F1AF047029BC6A55B0E89A617127DCCA76DF1B902312C105B25D4FF5A850DA3225AF710E4FFB04A61C0D8235D9B65FA94B5A61137A2B689E0304520C4C6CA798EA22710205C1DC0A2A83ECC56BDF72DFCABE41186CBF4B6449B12E129C3E43166826E8979A5A35F7D93811DF3FCB60AB4295C4C010F33224ED9DBF4D7328AC36EDCD712EF9B0282D86D8D8F9FEC2522BEFEF55B87F4254B0D819AE5EBCCCD7B986C30F7C0E2365D811738666C58437F866B10BCB5D19D6A90E18D60977D7E1981750E92A2C1E8FBE33F310F87C9EBCFFF078B4D65A2BC6D0000 , N'6.1.3-40302')

