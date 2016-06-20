using System;

using libDB;

namespace GameService.Model
{
    [Model(TableName = "t_levelinfo", DbName = "DataBase1", DbType = EnumDb.MYSQL)]
    [Serializable]
    public partial class LevelInfoModel : IModel
    {
        private byte[] _flags = new byte[2];
        public LevelInfoModel() { }
        /// 列名,是否主键,字段类型,是否有默认值,是否自动编号,是否GuId,列索引(可无限扩展)
        private int _level;

        [ModelField(Attributes = "nLevel,1,11,0,0,0,0")]
        public int Level
        {
            get { return _level; }
            set
            {
                _flags[0] += 1;
                _level = value;
            }
        }

        private long _levelUpExp;
        [ModelField(Attributes = "lnLvUpExp,0,12,0,0,0,1")]
        public long LvUpExp
        {
            get { return _levelUpExp; }
            set
            {
                _flags[1] += 1;
                _levelUpExp = value;
            }
        }



        public int Change(int index)
        {
            return _flags[index];
        }
        public bool IsNew()
        {
            return _isnew;
        }
        private bool _isnew = true;
        public void Init()
        {
            _isnew = false;
            for (int i = 0; i < _flags.Length; i++)
            {
                if (_flags[i] == 0)
                    _flags[i]++;
            }
        }

    }
}
