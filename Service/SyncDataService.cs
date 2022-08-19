using Microsoft.EntityFrameworkCore;

namespace Recorder
{
    public class SyncDataService
    {
        private readonly EntityMySqlDBContext dbMySql = new();
        private readonly EntityOracleDBContext dbOracle = new();

        /// <summary>
        /// 查询酒检信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TestingData>> GetTestingDataItemAsync()
        {
            try
            {
                return await dbOracle.TestingDatas.AsNoTracking().Select(x => ItemToDTO(x)).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询酒检信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<TData>> GetTDataItemAsync()
        {
            try
            {
                return await dbMySql.Datas.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 查询醉驾信息
        /// </summary>
        /// <returns></returns>
        public List<TData> GetTDataItemByParam()
        {
            return dbMySql.Datas.ToList().FindAll(x => x.AlcoholValueState.Equals(5) && x.DeptId > 0 && x.CreateTime >= DateTime.Now.AddDays(-7));
        }

        /// <summary>
        /// 添加酒检信息
        /// </summary>
        /// <param name="datas">酒检信息</param>
        /// <returns></returns>
        public async Task<bool> AddTData(TData[] datas)
        {
            try
            {
                if (datas != null)
                {
                    dbMySql.Datas.AddRange(datas);
                    await dbMySql.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改酒检信息
        /// </summary>
        /// <param name="datas">酒检信息</param>
        /// <returns></returns>
        public async Task<bool> UpdateTData(TData[] datas)
        {
            try
            {
                if (datas != null)
                {
                    dbMySql.Datas.UpdateRange(datas);
                    await dbMySql.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 酒检信息格式转换处理
        /// </summary>
        /// <param name="data">酒检信息</param>
        /// <returns></returns>
        private static TestingData ItemToDTO(TestingData data) =>
            new()
            {
                TId = data.TId,
                TestPoliceNo = !string.IsNullOrEmpty(data.TestPoliceNo) ? data.TestPoliceNo.Trim() : data.TestPoliceNo,
                DeviceNo = !string.IsNullOrEmpty(data.DeviceNo) ? data.DeviceNo.Trim() : data.DeviceNo,
                TestTime = data.TestTime,
                TestType = !string.IsNullOrEmpty(data.TestType) ? data.TestType.Trim() : data.TestType,
                TestUnit = !string.IsNullOrEmpty(data.TestUnit) ? data.TestUnit.Trim() : "mg/100ml",
                DeviceTestNo = data.DeviceTestNo,
                TestResult = data.TestResult,
                TransDate = data.TransDate,
                LeaderPolice = data.LeaderPolice,
                TestClass = !string.IsNullOrEmpty(data.TestClass) ? data.TestClass.Trim() : data.TestClass,
                DutyType = data.DutyType,
                WirelessUploadFlag = data.WirelessUploadFlag,
                DisposalType = data.DisposalType,
                TestPlace = data.TestPlace,
                TestPoliceSecond = data.TestPoliceSecond,
                TestPoliceSta = data.TestPoliceSta,
                SubjectName = !string.IsNullOrEmpty(data.SubjectName) ? data.SubjectName.Trim() : data.SubjectName,
                SubjectIdNo = !string.IsNullOrEmpty(data.SubjectIdNo) ? data.SubjectIdNo.Trim() : data.SubjectIdNo,
                LicenseNumber = !string.IsNullOrEmpty(data.LicenseNumber) ? data.LicenseNumber.Trim() : data.LicenseNumber
            };

        /// <summary>
        /// 酒检值状态信息
        /// </summary>
        /// <param name="testType"></param>
        /// <param name="testClass"></param>
        /// <param name="testResult"></param>
        /// <returns></returns>
        public int AlcoholValueStateByParams(string testType, string testClass, string testResult)
        {
            int alcohol_value_state = 0;
            if (testType == "定性排查" && testClass == "无酒精")
            {
                alcohol_value_state = 1;
            }
            else if (testType == "定性排查" && testClass == "有酒精")
            {
                alcohol_value_state = 2;
            }
            else if ((testType == "主动测试" || testType == "定量测试") && Convert.ToInt32(testResult) < 20)
            {
                alcohol_value_state = 3;
            }
            else if ((testType == "主动测试" || testType == "定量测试") && Convert.ToInt32(testResult) >= 20 && Convert.ToInt32(testResult) < 80)
            {
                alcohol_value_state = 4;
            }
            else if ((testType == "主动测试" || testType == "定量测试") && Convert.ToInt32(testResult) >= 80)
            {
                alcohol_value_state = 5;
            }
            return alcohol_value_state;
        }

        /// <summary>
        /// 执勤类型信息
        /// </summary>
        /// <param name="dutyType"></param>
        /// <returns></returns>
        public int DutyTypeByParams(string dutyType)
        {
            int duty_type = dutyType switch
            {
                "事故" => 2,
                "其他-乘客" => 3,
                "其他-驾驶员" => 4,
                _ => 1,
            };
            return duty_type;
        }

        /// <summary>
        /// 记录方式状态信息
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public int RecordModeByParams(string mode)
        {
            int record_mode = mode switch
            {
                "y" => 1,
                "p" => 2,
                _ => 3,
            };
            return record_mode;
        }

        /// <summary>
        /// 测试模式信息
        /// </summary>
        /// <param name="testtype"></param>
        /// <returns></returns>
        public int TestModeByParams(string testtype)
        {
            int test_mode = testtype switch
            {
                "主动测试" => 1,
                "定量测试" => 1,
                "定性排查" => 2,
                "1" => 2,
                "2" => 2,
                "被动测试" => 3,
                "拒绝测试" => 4,
                _ => 5,
            };
            return test_mode;
        }
    }
}
