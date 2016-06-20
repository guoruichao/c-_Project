using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libClient;
using libCommon;

namespace ClientTest
{
    class RoleState : BaseState
    {
        public long _nRoleCount;
        private RoleData[] _roleArr;
        MyTimeOut _tmSelectRole;
        MyTimeOut _tmSelectTimeOut;

        public RoleState(int nID, StateManager mgr)
        {
            sMgr = mgr;
            StateID = nID;
        }

        public override void OnEnter(object[] pars = null)
        {
            if (pars == null)
            {
                Console.WriteLine("errrrrr RoleState::OnEnter !!! ");
                return;
            }
            object obj = pars[0];
            int nRoleCount = int.Parse(obj.ToString());
            _nRoleCount = nRoleCount;
            _tmSelectTimeOut = new MyTimeOut(5000);
            UpdateRoleArr();
            RandomBehaviour();
//             if (nRoleCount <= 0)
//             {
//                 CreateRole();
//             }
        }

        private void RandomBehaviour()
        {
            int nRandValue = sMgr.GetContext<ClientUser>().random.Next(0, 100);
            if (nRandValue < 0)
            {
                CreateRole();
            }
            else
            {
                SelectRole();
            }
        }

        public override void OnRun()
        {

        }

        public override void OnExit()
        {

        }

        private void SelectRole()
        {
            Console.WriteLine(sMgr.GetContext<ClientUser>().lnRoleID+"   ...send");
            if (_roleArr == null||_roleArr.Length==0)
            {

                Console.WriteLine("没有角色，无法选择，自动调用创建角色" );
                CreateRole();
                return;
            }
            int nRandValue = sMgr.GetContext<ClientUser>().random.Next(0, _roleArr.Length - 1);
            RoleData role = _roleArr[nRandValue];
            _tmSelectRole = null;
            IDataCore dataCore = GetDataCore();
            dataCore.SelectRole(role.lnRoleID);
        }

        private RoleData[] UpdateRoleArr()
        {
            IDataCore dataCore = GetDataCore();
            int nCount = dataCore.GetAllRole(out _roleArr);
            if (nCount <= 0)
            {
                Console.WriteLine("ReadAllRole Error " + nCount.ToString());
            }
            return _roleArr;
        }

        private bool CreateRole()
        {
            
            int nRandValue = sMgr.GetContext<ClientUser>().random.Next(10000, 99999);
            string szRoleName = "随机" + nRandValue.ToString();
            IDataCore dataCore = GetDataCore();
            dataCore.CreateRole(szRoleName, 10011, 1);
            return true;
        }

        public void OnCreateRoleResult(int nResCode, long lnRoleID)
        {
            if (nResCode == 0)//创建角色成功
            {
                UpdateRoleArr();
                _nRoleCount = _roleArr.Length;
                SelectRole();
            }
            else//创建角色失败
            {
                
            }
        }
        public void OnSelectRoleResult(int nResCode, long lnRoleID)
        {
            Console.WriteLine(sMgr.GetContext<ClientUser>().lnRoleID+"  get...");
            long id = sMgr.GetContext<ClientUser>().lnRoleID;
            if (nResCode != 0)//选择角色失败
            {

            }
            else//选人成功
            {
                //Console.WriteLine("选人成功..");
                object [] pars = {lnRoleID};
                ChangeToState((int)enStateID.enStateID_Main, pars);
            }
        }
    }
}
