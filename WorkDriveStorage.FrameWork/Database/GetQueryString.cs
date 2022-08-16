using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDriveStorage.FrameWork.Database
{
    public class GetQueryString
    {
        public class SQLite
        {
            public const string GetQueryCreateDocument = @"
CREATE TABLE ""TB_DOCUMENT"" (
	""ProjectName""	TEXT,
	""DocumentName""	TEXT,
	""Path""	TEXT UNIQUE,
	""GroupName""	TEXT,
	""CreateTime""	TEXT NOT NULL,
	""LastEventTime""	TEXT NOT NULL
);";
			public const string GetQueryCreateProject = @"
CREATE TABLE ""TB_PROJECT"" (
	""ProjectName""	TEXT,
	""Memo""	TEXT,
	""MemoRtf""	TEXT,
	""CreateTime""	TEXT NOT NULL,
	PRIMARY KEY(""ProjectName"")
);";

			public const string GetQueryCreateSource = @"
CREATE TABLE ""TB_SOURCE"" (
	""ProjectName""	TEXT,
	""SourceName""	TEXT,
	""Type""	TEXT,
	""Path""	TEXT,
	""GroupName""	TEXT,
	""CreateTime""	TEXT NOT NULL,
	""LastEventTime""	TEXT NOT NULL,
	PRIMARY KEY(""ProjectName"",""Path"")
);";
			public const string GetQueryCreateWorkList = @"
CREATE TABLE ""TB_WORKLIST"" (
	""ProjectName""	TEXT,
	""WorkName""	TEXT NOT NULL,
	""WorkContent""	TEXT,
	""CreateTime""	TEXT NOT NULL,
	""CompletionTime""	TEXT,
	PRIMARY KEY(""WorkName"")
);";			
			public const string GetQueryCreateMenu = @"
CREATE TABLE ""TB_MENU"" (
	""Sequence""	INTEGER,
	""Name""	TEXT,
	""GroupName""	TEXT,
	""TabName""	TEXT,
	""Namespace""	TEXT,
	PRIMARY KEY(""Name"")
);";

			public const string GetQueryCreateQuery = @"
CREATE TABLE ""TB_QUERY"" (
	""Name""	TEXT NOT NULL,
	""Version""	TEXT NOT NULL,
	""Query""	TEXT,
	PRIMARY KEY(""Name"")
);";

			public const string GetQueryInsertQueryData = @"
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetProjectAllName', '0001', 'SELECT ProjectName FROM TB_PROJECT');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataProject', '0001', 'SELECT Memo, MemoRtf FROM TB_PROJECT WHERE ProjectName = @ProjectName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataSource', '0001', 'SELECT GroupName, SourceName, Type ,Path FROM TB_SOURCE WHERE ProjectName = @ProjectName ORDER BY GroupName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataDocument', '0001', 'SELECT DocumentName, Path, GroupName FROM TB_DOCUMENT WHERE ProjectName = @ProjectName ORDER BY GroupName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataWorkList', '0001', 'SELECT * FROM TB_WORKLIST');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataWorkItem', '0001', 'SELECT * FROM TB_WORKLIST WHERE WorkName=@WorkName ');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetMemoUpdate', '0001', 'UPDATE TB_PROJECT SET Memo = @MemoString, MemoRtf = @MemoRtf WHERE ProjectName = @ProjectName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetMenu', '0001', 'SELECT * FROM TB_MENU ORDER BY Sequence DESC');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetWorkItemAddAndUpdate', '0001', 'INSERT OR REPLACE INTO TB_WORKLIST (ProjectName, WorkName, WorkContent, CreateTime, CompletionTime) VALUES (@ProjectName, @WorkName, @WorkContent, @CreateTime, @CompletionTime);');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetWorkItemDelete', '0001', 'DELETE FROM TB_WORKLIST WHERE WorkName=@WorkName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetProjectAdd', '0001', 'INSERT INTO TB_PROJECT (ProjectName, CreateTime) VALUES (@ProjectName, @CreateTime) ');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetProjectMemo', '0001', 'SELECT Memo,MemoRtf FROM TB_PROJECT WHERE ProjectName=@ProjectName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetDocumentAdd', '0001', 'INSERT INTO TB_DOCUMENT (ProjectName,DocumentName,Path,GroupName,CreateTime,LastEventTime) VALUES (@ProjectName,@Name,@Path,@GroupName,@CreateTime,@LastEventTime)');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetSourceAdd', '0001', 'INSERT INTO TB_SOURCE (ProjectName,SourceName,Type,Path,GroupName,CreateTime,LastEventTime) VALUES (@ProjectName,@Name,@Type,@Path,@GroupName,@CreateTime,@LastEventTime)');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetDocumentDelete', '0001', 'DELETE FROM TB_DOCUMENT WHERE Path=@Path');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetSourceDelete', '0001', 'DELETE FROM TB_SOURCE WHERE Path=@Path');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetDocumentMove', '0001', 'UPDATE TB_DOCUMENT SET GroupName = @GroupName, Path = @newPath WHERE Path = @oldPath');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetSourceMove', '0001', 'UPDATE TB_SOURCE SET GroupName = @GroupName WHERE Path = @Path');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetSourceLastTimeUpdate', '0001', 'UPDATE TB_SOURCE SET LastEventTime = @LastEventTime WHERE Path = @Path');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetDocumentLastTimeUpdate', '0001', 'UPDATE TB_DOCUMENT SET LastEventTime = @LastEventTime WHERE Path = @Path');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataLastOpenFile', '0001', 'SELECT * FROM 
(
SELECT ''DOCUMENT'' as Type, ProjectName, DocumentName as FileName, Path, GroupName, LastEventTime FROM TB_DOCUMENT 
UNION
SELECT ''SOURCE'' as Type, ProjectName, SourceName as FileName, Path, GroupName, LastEventTime FROM TB_SOURCE
)
ORDER BY LastEventTime DESC');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('SetDocumentFileReName', '0001', 'UPDATE TB_DOCUMENT SET DocumentName=@name, Path = @newPath WHERE Path = @oldPath');

";

			public const string GetQueryString = @"SELECT Query FROM TB_QUERY WHERE Name=@Name AND Version=@Version ";
		}
    }
}
