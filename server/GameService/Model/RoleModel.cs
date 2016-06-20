using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libDB;

namespace GameService.Model
{
    /// <summary>
    /// 【用户账号表】实体.
    /// </summary>
    [Model(TableName = "T_RoleInfo", DbName = "DataBase1", DbType = EnumDb.MYSQL)]
    [Serializable]
    public partial class RoleModel : IModel
    {
        private byte[] _flags = new byte[10];
        public RoleModel() { }
        /// 列名,是否主键,字段类型,是否有默认值,是否自动编号,是否GuId,列索引(可无限扩展)
        private long _roleID;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "lnRoleID,1,12,0,0,0,0")]
        public long lnRoleID 
        {
            get { return _roleID; }
            set
            {
                _flags[0] += 1;
                _roleID = value;
            }
        }

        private long _accountID;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "lnAccountID,0,12,0,0,0,1")]
        public long lnAccountID
        {
            get { return _accountID; }
            set
            {
                _flags[1] += 1;
                _accountID = value;
            }
        }  
        private string _name;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "szName,0,16,0,0,0,2")]
        public string szName
        {
            get { return _name; }
            set
            {
                _flags[2] += 1;
                _name = value;
            }
        }
       
        private short _sex;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "bSex,0,10,0,0,0,3")]
        public short bSex
        {
            get { return _sex; }
            set
            {
                _flags[3] += 1;
                _sex = value;
            }
        }
       
        private int _score;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "nScore,0,11,0,0,0,4")]
        public int nScore
        {
            get { return _score; }
            set
            {
                _flags[4] += 1;
                _score = value;
            }
        }
        private int _gold;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "nGold,0,11,0,0,0,5")]
        public int nGold
        {
            get { return _gold; }
            set
            {
                _flags[5] += 1;
                _gold = value;
            }
        }

        private int _headId;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "nHeadID,0,11,0,0,0,6")]
        public int nHeadID
        {
            get { return _headId; }
            set
            {
                _flags[6] += 1;
                _headId = value;
            }
        }

        private int _level;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "nLevel,0,11,0,0,0,7")]
        public int nLevel
        {
            get { return _level; }
            set
            {
                _flags[7] += 1;
                _level = value;
            }
        }

        private long _lnExp;
        [ModelField(Attributes = "lnExp,0,12,0,0,0,8")]
        public long lnExp
        {
            get { return _lnExp; }
            set
            {
                _flags[8] += 1;
                _lnExp = value;
            }
        }
        private int _nMapPermission;
        [ModelField(Attributes = "nMapPermission,0,11,0,0,0,9")]
        public int nMapPermission
        {
            get { return _nMapPermission; }
            set
            {
                _flags[9] += 1;
                _nMapPermission = value;
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
