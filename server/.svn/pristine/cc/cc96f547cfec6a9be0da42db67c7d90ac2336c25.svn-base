using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using libDB;
namespace libDB
{
    /// <summary>
    /// 数据库连接池类.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ConnectionPool<T> where T : IModel, new()
    {
        private int _checkTime = 0;
        private int _checkNum;
        private ConcurrentDictionary<EnumDb, ConcurrentQueue<IDbConnection>> _dictPool;
        public static ConnectionPool<T> Instance;
        private EnumDb _dbType;
        static ConnectionPool()
        {
            Instance = new ConnectionPool<T>();
        }

        public ConnectionPool()
        {
            _dictPool = new ConcurrentDictionary<EnumDb, ConcurrentQueue<IDbConnection>>();
            _dbType = AttributeCache<T>.ModelAttribute.DbType;
        }

        /// <summary>
        /// 根据不同的数据库创建不同的连接池,并从连接池中获取一个可用连接.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetConnection()
        {
            ConcurrentQueue<IDbConnection> pool;
            _dictPool.TryGetValue(_dbType, out pool);
            if (pool == null)
            {
                pool = new ConcurrentQueue<IDbConnection>();
                _dictPool.TryAdd(_dbType, pool);
            }
            IDbConnection conn;
            pool.TryDequeue(out conn);
            if (conn == null)
            {
                switch (_dbType)
                {
                    case EnumDb.MSSQL:
                        conn = new SqlConnection(AttributeCache<T>.Conn);
                        break;
                    case EnumDb.MYSQL:
                        conn = new MySqlConnection(AttributeCache<T>.Conn);
                        break;
                    default:
                        break;
                }
            }

            return conn;
        }
        private int m_freeOpt = 0;
        /// <summary>
        /// 以触发的形式释放空闲的对象.
        /// </summary>
        /// <param name="pool"></param>
        private void DisposeFree(ConcurrentQueue<IDbConnection> pool)
        {
            //每隔一小时以触发的方式清理一次池.
            DateTime now = DateTime.Now;
            if (Environment.TickCount - _checkTime > 1000)
            {
                if (Interlocked.Exchange(ref m_freeOpt, 1) == 0)
                {
                    IDbConnection shouldDispose = null;
                    while (pool.Count >= _checkNum && _checkNum != 0)
                    {
                        pool.TryDequeue(out shouldDispose);
                        shouldDispose.Dispose();
                    }
                    _checkNum = pool.Count;
                    _checkTime = Environment.TickCount;
                    Interlocked.Exchange(ref m_freeOpt, 0);
                }
            }
        }

        /// <summary>
        /// 将一个连接释放到连接池.
        /// </summary>
        /// <param name="conn"></param>
        public void Relsase(IDbConnection conn)
        {
            conn.Close();
            ConcurrentQueue<IDbConnection> pool;
            _dictPool.TryGetValue(_dbType, out pool);
            DisposeFree(pool);
            pool.Enqueue(conn);
        }
    }
    /// <summary>
    /// 数据库Command类.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class CommandPool<T> where T : IModel, new()
    {
        private ConcurrentDictionary<EnumDb, ConcurrentQueue<IDbCommand>> _dictPool;
        public static CommandPool<T> Instance;
        private int _checkTime = 0;
        private int _checkNum;
        private EnumDb _dbType;
        static CommandPool()
        {
            Instance = new CommandPool<T>();
        }

        public CommandPool()
        {
            _dictPool = new ConcurrentDictionary<EnumDb, ConcurrentQueue<IDbCommand>>();
            _dbType = AttributeCache<T>.ModelAttribute.DbType;
        }

        /// <summary>
        /// 根据不同的数据库创建不同的Command连接池，并从连接池中获取一个可用连接.
        /// </summary>
        /// <param name="strsql"></param>
        /// <returns></returns>
        public IDbCommand GetCommand(string strsql = "")
        {
            ConcurrentQueue<IDbCommand> pool;
            _dictPool.TryGetValue(_dbType, out pool);
            if (pool == null)
            {
                pool = new ConcurrentQueue<IDbCommand>();
                _dictPool.TryAdd(_dbType, pool);
            }
            IDbCommand cmd;
            pool.TryDequeue(out cmd);
            if (cmd == null)
            {
                switch (_dbType)
                {
                    case EnumDb.MSSQL:
                        cmd = new SqlCommand(strsql);
                        break;
                    case EnumDb.MYSQL:
                        cmd = new MySqlCommand(strsql);
                        break;
                    default:
                        return null;
                }
            }
            cmd.CommandText = strsql;
            return cmd;
        }
        private int m_freeOpt = 0;
        /// <summary>
        /// 以触发的形式释放空闲的对象.
        /// </summary>
        /// <param name="pool"></param>
        private void DisposeFree(ConcurrentQueue<IDbCommand> pool)
        {
            //每隔一小时以触发的方式清理一次池.
            DateTime now = DateTime.Now;
            if (Environment.TickCount - _checkTime > 1000)
            {
                if (Interlocked.Exchange(ref m_freeOpt, 1) == 0)
                {
                    IDbCommand shouldDispose = null;
                    while (pool.Count >= _checkNum && _checkNum != 0)
                    {
                        pool.TryDequeue(out shouldDispose);
                        shouldDispose.Dispose();
                    }
                    _checkNum = pool.Count;
                    _checkTime = Environment.TickCount;
                    Interlocked.Exchange(ref m_freeOpt, 0);
                }
            }
        }


        /// <summary>
        /// 释放一个Command到对应连接池中.
        /// </summary>
        /// <param name="cmd"></param>
        public void Release(IDbCommand cmd)
        {
            cmd.Connection = null;
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Empty;
            cmd.Transaction = null;
            ConcurrentQueue<IDbCommand> pool;
            _dictPool.TryGetValue(_dbType, out pool);
            DisposeFree(pool);
            pool.Enqueue(cmd);
        }
    }

    internal class StringBuilderPool
    {
        static StringBuilderPool()
        {
            Instance = new StringBuilderPool();
        }

        public static StringBuilderPool Instance;
        private ConcurrentQueue<StringBuilder> _pool;

        public StringBuilderPool()
        {
            _pool = new ConcurrentQueue<StringBuilder>();
            for (int i = 0; i < 25; i++)
            {
                _pool.Enqueue(new StringBuilder());
            }
        }

        public StringBuilder GetStringBuilder()
        {
            StringBuilder sb = null;
            if (_pool.TryDequeue(out sb))
            {
                return sb;
            }
            return new StringBuilder();
        }

        public void Release(StringBuilder sb)
        {
            sb.Clear();
            _pool.Enqueue(sb);
        }
    }

}
