using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDriveStorage.FrameWork
{
    public class ServiceProvider
    {
        private static ServiceProvider staticService;

        private Config _config;
        public Config Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        private MainDatabase _mainDatabase;
        public MainDatabase MainDatabase
        {
            get
            {
                return _mainDatabase;
            }
            set
            {
                _mainDatabase = value;
            }
        }

        public static ServiceProvider StaticService()
        {
            if (staticService == null)
            {
                staticService = new ServiceProvider();
            }

            return staticService;
        }

        public ServiceProvider()
        {
            _config = new Config();
            _mainDatabase = new MainDatabase();
        }
    }
}
