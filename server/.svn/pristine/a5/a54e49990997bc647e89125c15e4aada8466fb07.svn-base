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
    [Model(TableName = "T_Account", DbName = "DataBase1", DbType = EnumDb.MYSQL)]
    [Serializable]
    public partial class AccountModel : IModel
    {
        private byte[] _flags = new byte[6];
        public AccountModel(){}
        /// 列名,是否主键,字段类型,是否有默认值,是否自动编号,是否GuId,列索引(可无限扩展)
        private long _accountID;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "lnAccountID,1,12,0,0,0,0")]
        public long lnAccountID
        {
            get { return _accountID; }
            set
            {
                _flags[0] += 1;
                _accountID = value;
            }
        } 
        private string _email;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "szEmail,0,16,0,0,0,1")]
        public string szEmail
        {
            get { return _email; }
            set
            {
                _flags[1] += 1;
                _email = value;
            }
        }
      
        private string _mobile;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "szMobile,0,16,0,0,0,2")]
        public string szMobile
        {
            get { return _mobile; }
            set
            {
                _flags[2] += 1;
                _mobile = value;
            }
        }
      
        private DateTime _onlinetime;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "OnlineTime,0,6,0,0,0,3")]
        public DateTime OnlineTime
        {
            get { return _onlinetime; }
            set
            {
                _flags[3] += 1;
                _onlinetime = value;
            }
        }
        private DateTime _offlinetime;
        /// <summary>
        /// 字段说明:.
        /// </summary>
        [ModelField(Attributes = "OfflineTime,0,6,0,0,0,4")]
        public DateTime OfflineTime
        {
            get { return _offlinetime; }
            set
            {
                _flags[4] += 1;
                _offlinetime = value;
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
