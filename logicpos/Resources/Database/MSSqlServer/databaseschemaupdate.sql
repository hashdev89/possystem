GO
if not exists (select * from cfg_configurationpreferenceparameter WHERE [Oid] = 'bc0bc5df-4c01-462a-81b7-9051a7869574')
INSERT INTO cfg_configurationpreferenceparameter (Oid,Disabled,Ord,Code,Token,Value,ValueTip,Required,RegEx,ResourceString,ResourceStringInfo,FormType,FormPageNo,InputType) VALUES ('bc0bc5df-4c01-462a-81b7-9051a7869574',NULL,590,590,'USE_CC_DAILY_TICKET','False','true | false',1,'RegexBoolean','prefparam_use_ccdailyticket',NULL,2,1,3);
GO
