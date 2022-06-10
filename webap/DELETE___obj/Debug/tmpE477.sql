CREATE TABLE [dbo].[MappingColors] (
    [ColorCode] [varchar](2) NOT NULL,
    [ColorDescription] [varchar](8000),
    [ColorHex] [varchar](10),
    CONSTRAINT [PK_dbo.MappingColors] PRIMARY KEY ([ColorCode])
)
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201904301237205_008', N'Ica.SalesOrders.Models.Context.AppContext',  0x1F8B0800000000000400DD5D5B6FE3B8157E2FD0FF20F871311BC7C9CE601AD8BBC83A93AEB13393609CD9ED5BC0488C2354965C499E2628FACBFAD09FD4BF50EACEFB4D94AD2CF21293E27778393C3C240FCFF9DF7FFE3BFFE9791B79DF609A8549BC98CC4E4E271E8CFD2408E3CD62B2CF1FBF7F3FF9E9C73FFF69FE21D83E7BBF35DF9D17DFA19271B6983CE5F9EE623ACDFC27B805D9C936F4D3244B1EF3133FD94E41904CCF4E4FFF329DCDA610414C1096E7CDBFECE33CDCC2F207FAB94C621FEEF23D883E25018CB23A1DE5AC4B54EF33D8C26C077CB898AC7C70B20611CC6ED20055E7A42A71822072F89C4FBCCB2804A8566B183D4E3C10C7490E7254E78BAF195CE769126FD63B9400A2BB971D44DF3D822883755B2EBACF759B757A56346BDA156CA0FC7D96275B43C0D979DD4F53BAB8556F4FDA7E443DF901F578FE52B4BAECCDC5E47A1FFB15344DEC6219A5C587C2CE6E8ABEF1F81FBC6939053154F1F7C65BEEA37C9FC2450CF7790AA237DEEDFE210AFD5FE1CB5DF277182FE27D14E1F54535467944024ABA4D931D4CF3972FF0916AC52A987853B2FC9406688B73CAD6ED8DF3F3B389F71955063C44B0650FAC6FD67992C2BFC218A62087C12DC87398A2D15D05B0EC60A616344D903FC15448595EF8166CE05D0AE22CAA99A22A8BF81A4DD789F7093C7F84F1267F5A4CD0BF13EF3A7C86419352B7E46B1CA2D98D0AE5E91EAAE815A37340725730F3D37077105A2BFF0044BEA6D1E034CA89A7E25F39C487B8F8B865C59F93248220368659053769B809632396BE023998493A69767AEA82B31095B3835039774FE533F8166ECA19C80C7DD5DD5F603541B3A77057AD7CAD84BE6FBEB94E93ED9724C2A45E9D75BF4EF6A98FA8DE25FCFC3B906E60AE5FAB02259357AAFE8453A732475CA52A9B57A3F9B45BE0A4CB5ED321C68B5E5570044B5E37CF4C173CC10C1D70B9ABE80D2AD8858CD8708D8219D533A4E137D10C69D8D58A1F2F773BC41465ED0A740BC6A41046C0A145356C14B2A6DCA1B8B3A0A7C79B676E2537DA8CA47CB6C406B3F8E8BEE2898E3779F90C83723F72C5A505623F2E2D1046C0A545356CB8B42977282EFD98202153FC2BE1CF1F5CE82FB720CBFE99A4C1D074964910FAF0834CED3B7BFBCE012145A7BD77D1183D99358C022ED5C1DC89175A1D93CA202BF1F211C49B3DDADC5AC895A6E808044A5395D595B950C1CB8A678516BF1A325F43593159CEDE0E49DCF90CC0CE2DF80B2DF6C17DC77EDD4CE0E5330B2DF7A35E0B2D71DE623C19B0D223980FAB80688DF96E8573F834FC727B5B32C2B02B206AFAD0248E27507E03D15ED683AEF779DDEC7535CDE9054F2A0BEC16BC6473697BFADE961DC1146FEB62B5E6E185DBE93D7B37B4365D1155AC776E4E670B6A2904EA062A8E5673B845E2102138DFA35294D6D04FE260287A46F303F59BE5EC4025473237EAB1B799195A6CE3765E20928A59F1D6058F5D8711AA9B7B4A26DC85D2D2173BF62A8B8E83BF2C798BE0ABE14F2F6821CFBD1A7171A75748ADC1A9601272705A97FBFC2949E574CEDE3B20F40966995CEF7574C50B72D01041FFC3BB706BAE602E937D357B7BEAB556B2E313D8ED10B965122536C7A078F111C890B21E4B44D55C8E60450FBBBF28091FD256A024F80B7C763BDBB539AE3863B3BEB5EC0A8F80DB5601DE16F3F300BCF4A1D6AF8A6ABFF35DDD7B57853509AA472C579ADCD83168DC03B8215472D3E0D3B7A4B244BA6DC10EC5A2C32E400E2C2E3ADEE4DC283399CC1103FB85A9DD45C5A6F29A55DF70EB559EE44B6A55E6F73AF3F83DAB17BFBB82E92D641909300A7946B7C95CA6D108073BE64CC3242DBFEB23931447044E4E32756FE8DD6C4D6134BCDC5B27698E8FB6ADC5DDEF61903FF51B40A4DBECB7B16218DDC8E15FF26DB48CD0DA3238A53F949DAC48BE5E6659E2872545CAE0A95980C83A7F88034F610158551BB78E42B547B2332CEE7B51151693EF98AE10C3B697651D6CB33292A0A7272733061749599816A20C444BB49C21B91DC6392B92C3D80F77209257812A666455570C404B86CEB9823B18170257DEB37DE9B764A8D545D547F329C62472DEE11A0688465A6E25D00D376335A4CF4C723B272E89CA96812431A3FB7E7E135FC108E6D0AB4EA5900404990F781B605419072C296B880E5FF06DE98CB852365CFDAA7000C6A48C8695C287B2207624D248C3637DFEFB8E96E5DAEDE65E5C8AEA29BFC5EC6A4BAC52FADD20B785E8E0BB0BD6B1CD42590B74A680D8B4C76826CAC6A97F350E301BD95DAE8867245BDE8E61F0031D7D7614EF9595AAC6F1395158F9832929C29179056A0A7D96A1C122E4C18643DE234E44F4B59E51B120DE069DE1E7DB4CDBB21F3E347ACC5775E80158AFDA7315AF8A510998765A72FBD09839BE4275AB4FB0B27A1F47335101B98639FB3EA5DBE131CA0AC3892448D5933C08C1313B0D50BFC5628A338A8D02A77ED220C3E10D1E83D32C713CAC4EBF508090D69F0C8ECC2C91A94F737FCDAD5067C6A501531AB47031CA1C3582A00AB52D83A23C7EF3C803222F3635465BCC7A925B1E1A883CFEE481298E58B1F9CB4EADF6281EFB4AF0FE8B1629EAE392B629C454666493FA8004036AFB9496FC643335BA806FE9CFF6837AE3AFBFF5C71A528B03496F4877F818522DA07A7708FDFA54C2129CADA6CE66D39E21C8EDE5008DE79BC1B25DA0DE75EAEF3BB16690F258D223D29D260688AD12BDFB867373C7768C62E3A3B9F5A1268860B66B6E7606111BCC65A1B42B581D5C4B0BEFD70D84DEAD1239922E680ED65B25AFCD9B4F2BBF3375C27C2A705033AF5726CC614D9DE2AD2B6F35CBEFD7E6AE5BB615C6D4276410AD92B694F2242D5FD010B9E52B3B781DA6595ED85C3D80E29661196C99CF309556B052378468AD951DAC66E56E4A14FFD79ABBD46B4FA7EEB2BB801AEB1AB5715B6C22CA6B4F8EA4658B7A85FB20108154E2F6A5BAFE62D3694E94A0310E5D084C26571F99B9C5C281994C7D5CFAB60A87A5F3F451895B5A1C92C8D0C7AB1CB4E040558A3E42E97D05072813F4CBD79E5570843A491FA37DD786A3B48906BDD11EBB103D223C8C918C53E5708518A12AC90CE38CC5609CBDA830CE598C731EC67C4A490166FFCFC81EE6E08414665AA2AE59971C0A3ABEEEA021E644058767188E0B0F724A30D9A31940911E6F3D7CF4A188F9382A1144C3D05CC2E17D2FBA9893A3088792C91CCD4056AADD3003C9D361CD06928F205C96EA23546265121CAB8A5130A7103810966CA26834F69EA486D1A4EA23759E1D70A42E551F896D9A69ABDC4C18D305FC4813A4DB0F3B9C24A203008DD9212E2A6468EC6A93E068C995A71A8D3343881C73442E433099A3610CE2E4C5216FE0BB0473F6909616AB34C26D8BD4D3800CB37AEC4F6FAC0C7753CC16CA86AFDCF07DFDF61E07AA9346C391D8858A4B59D5DEC458082B7159C9F2DBBD6AA41660D183782D3CEE928E679961D68F88693CEEB3641916FE209C987858BA3E1AFBC81CC7647347C5BBC55D9D63C6E5406AB22DB7E4211882782ECE41336556FC4D38718086A58F890D5C0BAFEA0ED78A0904452523C7E1004B71E548FC35AFA919D1628442BECC16881423C4EEFD358ED6A5EA23B50FAC71A036D1ECD88C3D3533DBA3D5AFA7C92DDA9E674450A21C678E91E60A0E271B61E7603EE1E4C5C59DDE3E9626BB5DF0FC5A89253CC461730D91CB87CE0C62993A1AE6C06F0D1DB20666B762CE18B2C2E22D0D6E2A43EE68C446346A441ED671AE1BBA47CBF479577CB453AAFAD9310E53271962908F8A193C327B34938736B9723881285B2DF349A40210332C6D264632AEDC884CCA76ED3B5982EDDA54039661763AA6FB26D777ACF56B5752DF2E930C94AEF62D2BA170B5A9FA48F56B561CA64E325AC1DAE7ACD41AD6A6EBA3612F5671302CF9B5DFE60F2D8248E31BBE25496390676C2DD21414998428EEDA0AEB2281690869D8C7F69A33BB92D2A4BFC131AE9FD070BFF78588B85EB441153BEC8C5D15FD49CB74754AFBBBB5ABAA6D9AD4D1C01823A7EA938987EAFE2D0C0A03A7F54B86F67D27C50727EB7FA0891B2236EF3EF804E2F0116679E5096272763A3BA382888D27A0D734CB02C2518524AA1739684788A51516BDAC745261E8AE40143EAB24D661F57012F00DA4FE13482947F185DB073378BE4F0057E81C9F18AEA0F1C858AE30B14058AE2089B857D4F0DB38DD7F08CD2168BF4E6535987741AB3880CF8BC9BFCA4217DEEA6FF74DB9375ED98A0BEFD4FBB73107E081B3789D3AB3E02A3C4C9643CCF35E98A6A19D2C249FBB904A83483D611425BBC9641D9BC8A267DD84021AA45705D17F787D7A36408F72DEAF1E306CCD203DCA44AAE1F5E50FC62284F647E706958E42C31DF7B7EF4C6155CD7F6F5C51CE54D05863AA52A215669815D23CDE8B05FFBB8DB2C21D75273154F8FCE42A428AC3C1101AE31C2FCAC720D2090FECE1468460613CDC004AF85363CA77A5C969DF8BB589681E03EB1B02439CE384A2C8B6208A865A26D91014AEF6454CE880AE1DBD834ED8A8462CAE28C4C4A08A17D758E6F0711C86652A2A7E03AF43DF1A0F171BADC106D73C3683DD48F58E8830EC8C972B1E33F3C31A220C811B4C5E800337C87438032E6A11CFC00C960A5EE0EC8C0D8B5510203EC81DC42A50AA9BAE221358CC1E97B100DCA8D5224FFFAE469876EC6FC3E5366EFCADB46CD7AEF3071174A4B77CFDF3D02A24708FBDEA5047B1569EF91D1D9DEA9C7B58C0125EF75D4D25A1937D9EE4EC318B640622C776DA3ECCAE95F2D36EA3C9ABB46CF38DAAF6B9A985BA89796077C4F1B4BF759BBB1EC2D7BACD20B09ED65DCD3DC6B1BA2BE0577445AA2D40B48C43FADE973BBD0A11DC766B2C6E5DC95ECBDB810EA0DD3BBA275502530FF4AC9F2E4B7FD2564E3A8576C8FAB7979E8DEBFAD7E005D6D6597DE5030F1BC3437A8F177ABEEBE732BFAF537A811F0B5DF165C4636AAF0B26640FEE7C9E6B0F682B2606E22DD1F2C6AB9299EF7A5B5E33122C725B49636166C3E7F22A1C9EE978469EA3166FE363C12389BB3E5C7854B167147BA273278A8DEF41224170DD86DAC7BBB0622D898F1413D30523BE92FBDE30253BAAA811DA1AFB408CC41E931AC70A7815DABEF4A5E3E8F47DDDA80F3D14EB3F183FD968DA4E633BBC8E780EAC8F5C7A20496FF59D32DF1D55307A7EF5046531091E1234E8D57187D01FB820A083249E030F5EE4FF990667D42D860AF3058F1CDF2BB9845835CC3262D5173C627CB7CE34B14EE160A874593C78B127719A04A14C3054885C1E21990F7471D40979D0096E83B080151A54144129441450A6167E65CB210B5B21A0A0462703563014C86C1E152A2286821C2EB3196278A690897567297DBBC450A33FE051545C7289249B5A4208BF94CABD7BBECC183E7A073964B82F5D76D9E11DE570DE95B21AC008A373E0A3C7F53C2B6BBE4074731D9FF6EF0C41380D55800EC1F35CDB51D43BC3D07866ECBC439A5807CA0EE1BF071E803F8EDB3D030533A15508D291A9AC3B389A01E3E6B27FB39DC729B1138DEC3A48BBBF71DAD4FE51487A08B3C11A6E107B847D0B8FB641FBB8B0A9A97E5DC12CDC7410738419439FD800B5DFACE2C7A4D98751356A3E618C2B7310A0DDD1659A878FC0CF51B60FB3AC8C6C5EBE0D584C3E6C1F60B08A6FF6F96E9FA326C3ED4344784629F67332FA658015B2CEF39BD2B82473D10454CDB03043BA897FDE878571495DEF6BCE1DB400A2D828D6463DC558E68571CFE6A545FA9CC49A4075F7B5FBDB3BB8DD210901B39B780DBE419BBA2106FC0837C07F695C1A8841D4034176FBFC2A049B146CB31AA32B8F7E221E0EB6CF3FFE1FEE1525D646AE0000 , N'6.1.3-40302')

