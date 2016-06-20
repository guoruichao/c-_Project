using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libClient;
using libCommon;

namespace ClientTest
{
    public abstract class BaseState
    {
        public static int nStateID;
        public int StateID{get{return nStateID;} set{nStateID= value;}}
        public StateManager sMgr;
        public virtual void OnEnter(object[] pars = null)
        {

        }

        public virtual void OnRun()
        {

        }

        public virtual void OnExit()
        {

        }
        public void ChangeToState(int nStateID, object[] pars = null)
        {
            if (sMgr != null)
            {
                sMgr.ChangeToState(nStateID, pars);
            }
        }

        public IDataCore GetDataCore()
        {
            ClientUser user = sMgr.GetContext<ClientUser>();
            if (user == null)
                return null ;
            IDataCore dataCore = user.GetDataCore();
            if (dataCore == null)
                return null;
            return dataCore;
             
        }
             
    }

    public class StateManager
    {
        public Action<StateManager> OnRegState;
        private BaseState[] arrState;
        private object _Context;
        private int nCurStateID = -1;
        private MyTimeOut _tmDebugTimer;
        public bool Create(object Context,int nMaxStateCount,int nInitStateID = -1)
        {
            _tmDebugTimer = new MyTimeOut(2000);
            arrState = new BaseState[nMaxStateCount];
            _Context = Context;
            if (OnRegState != null)
                OnRegState(this);
            if (nInitStateID != -1)
            {
                ChangeToState(nInitStateID);
            }
            return true;
        }

        public void Update()
        {
            if (_tmDebugTimer.IsTimeOut())
            {
                Console.WriteLine(GetContext<ClientUser>().lnRoleID+"  "+nCurStateID+"  "+arrState[nCurStateID]);
            }
            if (nCurStateID != -1 && arrState[nCurStateID]!=null )
            {
                arrState[nCurStateID].OnRun();
            }
        }
        public void AddState(BaseState state)
        {
            if (arrState != null)
            {
                arrState[state.StateID] = state;
            }
        }

        public void ChangeToState(int nStateID, object[] pars = null)
        {
            if (nCurStateID != -1 && arrState[nCurStateID] != null)
            {
                arrState[nCurStateID].OnExit();
            }
            nCurStateID = nStateID;

            arrState[nCurStateID].OnEnter(pars);
        }

        public T GetState<T>(int nID)
            where T : BaseState 
        {
            return arrState[nID] as T;
        }

        public T GetContext<T>()
             where T : class, new()
        {
            return _Context as T;
        }


    }
}
