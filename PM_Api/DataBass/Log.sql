--DROP TABLE Log;
CREATE TABLE Log ( 
ID INTEGER PRIMARY KEY,
UserId VARCHAR (500),  --操作员
Method VARCHAR (500),  --操作方法
RequestParm text, --请求参数
ResponseParm text, --响应参数
ResponseTime VARCHAR (500), --响应时间
IP VARCHAR (500), --请求IP
CreateTime datetime --操作时间
)
