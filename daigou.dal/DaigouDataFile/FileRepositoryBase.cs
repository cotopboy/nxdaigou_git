using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace daigou.dal.DaigouDataFile
{
    public abstract class FileRepositoryBase
    {
        protected FileDB FileDB = new FileDB();
        protected FileDBMgr fileDbmgr;

        public FileRepositoryBase(FileDBMgr fileDbmgr)
        {
            this.fileDbmgr = fileDbmgr;
            FileDB = fileDbmgr.LoadFileDB();
        }

    }
}
