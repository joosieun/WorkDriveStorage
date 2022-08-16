using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDriveStorage.FrameWork
{
    public class Constant
    {
        public enum PageName
        {
            MainView,
            Calendar,
            Setting,
            Project,
            Memo
        }

        public enum FileType
        {
            Document,
            Source
        }

        public enum DocumentFileMoveType
        {
            Copy,
            Move
        }

    }
}
