using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDriveStorage.FrameWork;

namespace WorkDriveStorage
{
    public class Config
    {
        public Constant.DocumentFileMoveType DocumentFileMoveType { set; get; }
        public string DatabasePath { get; set; }
        public string FileDataPath { get; set; }
    }
}
