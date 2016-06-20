//登陆线程池，登陆使用多线程驱动。只要保证每个session的会话顺序即可

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using libCommon;
namespace GameService
{

    abstract public class MatchRoomProcess
    {
        public virtual long GetKey() { return 0; }
        public virtual void DoProc() { }
        
        private MatchThread _matchThread;
        public void SetThread(MatchThread th = null)
        {
            if (_matchThread!= null && th == null)
            {
                _matchThread.DelProcess(this);
            }
            _matchThread = th;
        }
        
    }
    public class MatchThread 
    {
        private Thread _thread;
        private ConcurrentDictionary<long, MatchRoomProcess> _processMap;
        private void ThreadFunction()
        {
            while (true)
            {
                foreach (var proc in _processMap)
                {
                    proc.Value.DoProc();
                } 
                Thread.Sleep(1);
            }
        }
        public bool StartThread()
        {
            _processMap = new ConcurrentDictionary<long, MatchRoomProcess>();
            _thread = new Thread(ThreadFunction);
            _thread.Start();
            return true;
        }

        public long GetCurCount()
        {
            return _processMap.Count;
        }

        public bool AddProcess(MatchRoomProcess proc)
        {
            MatchRoomProcess oldProc;
            if (_processMap.TryGetValue(proc.GetKey(), out oldProc))
            {
                return false;
            }
            proc.SetThread(this);
            return _processMap.TryAdd(proc.GetKey(), proc);
        }

        public void DelProcess(MatchRoomProcess proc)
        {
            MatchRoomProcess oldProc;
            _processMap.TryRemove(proc.GetKey(),out oldProc);
        }
    }

    public class MatchThreadPool : SingleInst<MatchThreadPool>
    {
        private MatchThread [] _ThreadList;
        public bool Create(int nThreadCount)
        {
            _ThreadList = new MatchThread[nThreadCount];
            for (int n = 0; n < nThreadCount;n++ )
            {
                _ThreadList[n] = new MatchThread();
                _ThreadList[n].StartThread();
            }
            return true;
        }

        public void AddProcss(MatchRoomProcess matchProc)
        {
            int nMinCount = 1000000;
            int nMinIndex = 0;
            for (int n=0;n<_ThreadList.Length;n++)
            {
                MatchThread th = _ThreadList[n];
                if (th.GetCurCount() < nMinCount)
                {
                    nMinIndex = n;
                }
            }
            _ThreadList[nMinIndex].AddProcess(matchProc);
        }
    }
}
