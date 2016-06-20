using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libDB;

namespace GameService.Model
{
    [Model(TableName = "t_mapinfo", DbName = "DataBase1", DbType = EnumDb.MYSQL)]
    [Serializable]
    public partial class MapInfoModel : IModel
    {

        private byte[] _flags = new byte[3];
        public MapInfoModel() { }

        /// 列名,是否主键,字段类型,是否有默认值,是否自动编号,是否GuId,列索引(可无限扩展)
        private int _mapId;

        [ModelField(Attributes = "nMapId,1,11,0,0,0,0")]
        public int MapId
        {
            get { return _mapId; }
            set
            {
                _flags[0] += 1;
                _mapId = value;
            }
        }

        private int _price;
        [ModelField(Attributes = "nPrice,0,11,0,0,0,1")]
        public int Price
        {
            get { return _price; }
            set
            {
                _flags[1] += 1;
                _price = value;
            }
        }

        private int _mapPermission;
        [ModelField(Attributes = "nMapPermission,0,11,0,0,0,2")]
        public int MapPermission
        {
            get { return _mapPermission; }
            set
            {
                _flags[2] += 1;
                _mapPermission = value;
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
