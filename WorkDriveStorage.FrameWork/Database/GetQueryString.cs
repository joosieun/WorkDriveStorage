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

			public const string GetQueryCreateMemo = @"
CREATE TABLE ""TB_MEMO"" (
	""Sequence""	INTEGER,
	""Contents""	TEXT,
	""ContentsRTF""	TEXT,
	""WallPaper""	TEXT,
	""Location""	TEXT,
	""Size""	TEXT,
	PRIMARY KEY(""Sequence"" AUTOINCREMENT)
);
";

			public const string GetQueryInsertQueryData1 = @"
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataProject', '0001', 'SELECT Memo, MemoRtf FROM TB_PROJECT WHERE ProjectName = @ProjectName');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataWorkList', '0001', 'SELECT * FROM TB_WORKLIST');
INSERT INTO TB_QUERY
(Name, Version, ""Query"")
VALUES('GetDataWorkItem', '0001', 'SELECT * FROM TB_WORKLIST WHERE WorkName=@WorkName ');

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
VALUES('GetDataLastOpenFile', '0001', 'SELECT * FROM 
(
SELECT ''DOCUMENT'' as Type, ProjectName, DocumentName as FileName, Path, GroupName, LastEventTime FROM TB_DOCUMENT 
UNION
SELECT ''SOURCE'' as Type, ProjectName, SourceName as FileName, Path, GroupName, LastEventTime FROM TB_SOURCE
)
ORDER BY LastEventTime DESC');




";

			public const string GetQueryString = @"SELECT Query FROM TB_QUERY WHERE Name=@Name AND Version=@Version ";
			public const string GetColumnCheck = @"SELECT sql FROM sqlite_master WHERE name=@TableName AND sql LIKE @Value ";
			public const string GetColumnAdd = @"ALTER TABLE @@TableName ADD COLUMN @@ColumName TEXT; ";

			//Memo
			public const string GetDataStickerMemoAll = @"SELECT * FROM TB_MEMO";
			public const string SetStickerMemoAdd = @"INSERT INTO TB_MEMO (WallPaper) VALUES (@WallPaper) ";
			public const string SetStickerMemoUpdate = @"UPDATE TB_MEMO SET Contents = @Contents, ContentsRTF = @ContentRTF WHERE Sequence = @Sequence";
			public const string GetDataStickerMemoMaxSequence = @"SELECT MAX(Sequence) as Sequence, Contents FROM TB_MEMO";
			public const string SetStickerMemoWallPaperChanged = @"UPDATE TB_MEMO SET WallPaper=@WallPaper WHERE Sequence = @Sequence";
			public const string SetStickerMemoLocationChanged = @"UPDATE TB_MEMO SET Location=@Location WHERE Sequence = @Sequence";
			public const string SetStickerMemoSizeChanged = @"UPDATE TB_MEMO SET Size=@Size WHERE Sequence = @Sequence";
			public const string SetStickerMemoDelete = @"DELETE FROM TB_MEMO WHERE Sequence=@Sequence";

			//Project
			public const string SetMemoUpdate = @"UPDATE TB_PROJECT SET Memo = @MemoString, MemoRtf = @MemoRtf WHERE ProjectName = @ProjectName";
			public const string SetDocumentAdd = @"INSERT INTO TB_DOCUMENT (ProjectName,DocumentName,Path,GroupName,CreateTime,LastEventTime) VALUES (@ProjectName,@Name,@Path,@GroupName,@CreateTime,@LastEventTime)";
			public const string SetSourceAdd = @"INSERT INTO TB_SOURCE (ProjectName,SourceName,Type,Path,GroupName,CreateTime,LastEventTime) VALUES (@ProjectName,@Name,@Type,@Path,@GroupName,@CreateTime,@LastEventTime)";
			public const string GetProjectAllName = @"SELECT ProjectName FROM TB_PROJECT";
			public const string GetProjectMemo = @"SELECT Memo,MemoRtf FROM TB_PROJECT WHERE ProjectName=@ProjectName";
			public const string GetDataDocument = @"SELECT DocumentName, Path, GroupName FROM TB_DOCUMENT WHERE ProjectName = @ProjectName ORDER BY GroupName";
			public const string GetDataSource = @"SELECT GroupName, SourceName, Type ,Path FROM TB_SOURCE WHERE ProjectName = @ProjectName ORDER BY GroupName";
			public const string SetDocumentFileReName = @"UPDATE TB_DOCUMENT SET DocumentName=@name, Path = @newPath WHERE Path = @oldPath";
			public const string SetDocumentDelete = @"DELETE FROM TB_DOCUMENT WHERE Path=@Path";
			public const string SetSourceDelete = @"DELETE FROM TB_SOURCE WHERE Path=@Path";
			public const string SetDocumentLastTimeUpdate = @"UPDATE TB_DOCUMENT SET LastEventTime = @LastEventTime WHERE Path = @Path";
			public const string SetSourceLastTimeUpdate = @"UPDATE TB_SOURCE SET LastEventTime = @LastEventTime WHERE Path = @Path";
			public const string SetDocumentMove = @"UPDATE TB_DOCUMENT SET GroupName = @GroupName, Path = @newPath WHERE Path = @oldPath";
			public const string SetSourceMove = @"UPDATE TB_SOURCE SET GroupName = @GroupName WHERE Path = @Path";
			public const string SetProjectAdd = @"INSERT INTO TB_PROJECT (ProjectName, CreateTime) VALUES (@ProjectName, @CreateTime) ";
		}
	}
}
