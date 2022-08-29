using Quartz;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Recorder
{
    public class SynchronizeJob : IJob
    {
        private readonly string licenses = AppSettingsHelper.ReadAppSettings("License", "Key");
        private readonly string synchronize = AppSettingsHelper.ReadAppSettings("OpenLock", "Synchronize");
        private readonly string sms = AppSettingsHelper.ReadAppSettings("OpenLock", "Sms");
        private readonly string wireless = AppSettingsHelper.ReadAppSettings("OpenLock", "Wireless");

        private readonly SyncDeviceService _device = new();
        private readonly SyncDeptService _dept = new();
        private readonly SyncPoliceService _police = new();
        private readonly SyncDataService _data = new();
        private readonly SyncDriverService _driver = new();
        private readonly SyncMessageService _message = new();
        private readonly EncryptionService _encryption = new();
        private readonly SmsDataService _smsData = new();
        private readonly SmsService _sms = new();

        public async Task Execute(IJobExecutionContext context)
        {
            // 许可到期验证
            if (true)
            {
                bool result = await _encryption.LicenseAsync(licenses);
                if (result)
                {
                    await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  license expiration");
                    return;
                }
            }

            // 短信服务
            if (sms.Equals("0"))
            {
                // 短信记录
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<TMessage> messages = new();
                        // 查询短信信息
                        List<TMessage> iList = await _message.GetMessageItemAsync();
                        List<SmsData> sms = await _smsData.GetSmsItemAsync();
                        // 较对重复
                        sms.ForEach(item =>
                        {
                            if (!string.IsNullOrEmpty(item.Mobiles) && iList.Find(x => x.Id.Equals(item.Id)) == null)
                            {
                                string title = item.WineCheckValues >= 80 ? "醉驾" : "酒驾";
                                TMessage message = new()
                                {
                                    Id = item.Id,
                                    Mobiles = item.Mobiles,
                                    Content = Convert.ToDateTime(item.CreateTime).ToString("F") + "" + item.DeptName + "" + item.PoliceName + "查处一宗" + title + item.WineCheckValues + "" + item.Util,
                                    Flag = 0,
                                    CreateTime = DateTime.Now,
                                    CreateBy = 1,
                                    DelFlag = 0,
                                    Remark = ""
                                };
                                messages.Add(message);
                            }
                        });
                        if (messages.Count > 0)
                        {
                            result = await _message.AddMessage(messages.OrderBy(x => x.Id).ToArray());
                            if (result)
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  add message " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                        }
                        else
                        {
                            await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new message data");
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });

                // 定时推送
                await Task.Run(async () =>
                {
                    string clock = AppSettingsHelper.ReadAppSettings("Sms", "clock");
                    // 每天早上9:00执行
                    if (clock.Equals("0"))
                    {
                        if (DateTime.Now.Hour != 9)
                        {
                            return;
                        }
                    }
                    // API接口标识
                    string api_id = AppSettingsHelper.ReadAppSettings("Sms", "api_id");
                    // 短信签名ID
                    int s_id = Convert.ToInt32(AppSettingsHelper.ReadAppSettings("Sms", "s_id"));
                    // 发送类型
                    int sms_type = 4;
                    // API接口秘钥
                    string key = AppSettingsHelper.ReadAppSettings("Sms", "key");
                    // 回执通知标识
                    int is_notify = 0;
                    // 自定义内容
                    string extend = string.Empty;
                    // API接口地址
                    string url = "http://" + AppSettingsHelper.ReadAppSettings("Sms", "ip") + ":" + AppSettingsHelper.ReadAppSettings("Sms", "port") + "/api/sms";
                    // 时间戳，精确到毫秒
                    DateTime dt = DateTime.Now;
                    DateTimeOffset dto = new(dt);
                    long time_stamp = dto.ToUnixTimeMilliseconds();
                    // 校验码
                    string splicing = api_id + "&" + s_id + "&" + sms_type + "&" + time_stamp + "&" + key;
                    var md5 = MD5.Create();
                    var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(splicing));
                    var strResult = BitConverter.ToString(bytes);
                    string sign = strResult.Replace("-", "");
                    // 类型
                    string content_type = "application/json";

                    while (true)
                    {
                        TMessage? message = await _message.GetMessageAsync();
                        if (message != null)
                        {
                            List<TMessage> updateList = new();
                            // 参数
                            string parameter = "{\"api_id\":\"" + api_id + "\",\"content\":\"" + message.Content + "\",\"mobiles\":\"" + message.Mobiles + "\",\"s_id\":" + s_id + ",\"sms_type\":" + sms_type + ",\"time_stamp\":" + time_stamp + ",\"sign\":\"" + sign + "\",\"send_time\":\"" + dt.ToString("G") + "\",\"is_notify\":" + is_notify + ",\"extend\":\"" + extend + "\"}";
                            // 调用sms接口
                            string result = await _sms.DoPostRequest(url, parameter, content_type);
                            // 读取发送标记
                            if (!string.IsNullOrEmpty(result))
                            {
                                dynamic? returned = JsonConvert.DeserializeObject<dynamic>(result);
                                if (returned != null)
                                {
                                    if (Convert.ToString(returned.code).Equals("00000"))
                                    {
                                        message.SendTime = dt;
                                        message.Flag = 1;
                                        await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  id:" + message.Id + " sms succ");
                                    }
                                    else
                                    {
                                        message.SendTime = dt;
                                        message.Flag = 2;
                                        await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  id:" + message.Id + " sms fail");
                                    }
                                    message.LastUpdateTime = DateTime.Now;
                                    message.LastUpdateBy = 1;
                                }
                                updateList.Add(message);
                                await _message.UpdateMessage(updateList.ToArray());
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                });
            }

            // 同步数据服务 OracleToMySQL
            if (synchronize.Equals("0"))
            {
                // 部门信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<PoliceStation> iList = await _dept.GetPoliceStationItemAsync();
                        if (iList.Count > 0)
                        {
                            List<SysDept> pArray = new();
                            // 读取基础信息
                            List<SysDept> depts = await _dept.GetSysDeptItemAsync();
                            // 较对同步
                            iList.ForEach(item =>
                            {
                                if (item.Policestano != 0)
                                {
                                    // 查找判断
                                    SysDept? dept = depts.Find(x => x.Id.Equals(item.Policestano));
                                    if (dept == null)
                                    {
                                        var entity = new SysDept
                                        {
                                            Id = item.Policestano,
                                            Name = item.Policestation,
                                            ParentId = item.Policestaaff,
                                            CreateTime = DateTime.Now,
                                            DelFlag = 0,
                                            IsDuty = 0,
                                            CreateBy = 0,
                                            Disabled = 0,
                                            UserId = 1,
                                            SynchId = item.Policestano.ToString()
                                        };
                                        pArray.Add(entity);
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _dept.AddSysDept(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} synchronize dept data " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new dept data synchronize");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });

                // 警员信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<TestPolice> iList = await _police.GetTestPoliceItemAsync();
                        if (iList.Count > 0)
                        {
                            List<TPolice> pArray = new();
                            // 读取基础信息
                            List<TPolice> polices = await _police.GetTPolicetItemAsync();
                            List<SysDept> depts = await _dept.GetSysDeptItemAsync();
                            // 较对同步
                            iList.ForEach(item =>
                            {
                                if (!string.IsNullOrEmpty(item.TestPoliceNo))
                                {
                                    // 查找判断
                                    TPolice? police = polices.Find(x => x.PoliceNum.Equals(item.TestPoliceNo));
                                    if (police == null)
                                    {
                                        var entity = new TPolice
                                        {
                                            Name = item.TestPoliceName,
                                            PoliceNum = item.TestPoliceNo,
                                            Mobile = item.Phone,
                                            Gender = 1,
                                            Status = 0,
                                            CreateTime = DateTime.Now,
                                            CreateBy = 0,
                                            DelFlag = 0
                                        };

                                        SysDept? model = depts.Find(x => x.Id.Equals(item.PoliceStaNo));
                                        if (model != null)
                                        {
                                            entity.DeptId = model.Id;
                                        }

                                        pArray.Add(entity);
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _police.AddTPolice(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} synchronize police data " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new police data synchronize");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });

                // 设备信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<DeviceList> iList = await _device.GetSyncDeviceAsync();
                        if (iList.Count > 0)
                        {
                            List<TDevice> pArray = new();
                            // 读取基础信息
                            List<TDevice> devices = await _device.GetDeviceItemAsync();
                            List<SysDept> depts = await _dept.GetSysDeptItemAsync();
                            List<SysDeviceType> deviceTypes = await _device.GetDeviceTypeItemAsync();
                            // 较对同步
                            iList.ForEach(item =>
                            {
                                if (!string.IsNullOrEmpty(item.Instruno))
                                {
                                    // 查找判断
                                    TDevice? sysDevice = devices.Find(x => x.DeviceSn.Equals(item.Instruno));
                                    if (sysDevice == null)
                                    {
                                        var entity = new TDevice
                                        {
                                            DeviceSn = item.Instruno,
                                            CreateDate = DateTime.Now,
                                            LastUpdateDate = DateTime.Now.AddDays(-2),
                                            CreateBy = 0,
                                            Status = 0,
                                            DelFlag = 0
                                        };

                                        SysDeviceType? sysDeviceType = deviceTypes.Find(x => x.DeviceCode.Equals(item.DeviceType));
                                        if (sysDeviceType != null)
                                        {
                                            entity.DeviceTypeId = sysDeviceType.Id;
                                        }

                                        SysDept? dept = depts.Find(x => x.Id.Equals(Convert.ToInt32(item.Instruaff)));
                                        if (dept != null)
                                        {
                                            entity.DeptId = dept.Id;
                                        }

                                        pArray.Add(entity);
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _device.AddSysDevice(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} synchronize device data " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new device data synchronize");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });

                // 驾驶人信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<TDriverInfo> iList = await _driver.GetDriverItemAsync();
                        if (iList.Count > 0)
                        {
                            List<TDriverInfo> pArray = new();
                            // 读取基础信息
                            List<TDriverInfo> drivers = await _driver.GetDriverInfoItemAsync();
                            // 较对同步
                            iList.ForEach(item =>
                            {
                                if (!string.IsNullOrEmpty(item.DriverIdentificationName) && !string.IsNullOrEmpty(item.DriverIdentificationNumber) && !string.IsNullOrEmpty(item.LicensePlateNumber))
                                {
                                    // 查找判断
                                    TDriverInfo? driver = drivers.Find(x => item.CreateTime.Equals(x.CreateTime) && item.DriverIdentificationName.Equals(x.DriverIdentificationName) && item.DriverIdentificationNumber.Equals(x.DriverIdentificationNumber) && item.LicensePlateNumber.Equals(x.LicensePlateNumber));
                                    if (driver == null)
                                    {
                                        var entity = new TDriverInfo
                                        {
                                            DriverIdentificationName = item.DriverIdentificationName,
                                            DriverIdentificationNumber = item.DriverIdentificationNumber,
                                            LicensePlateNumber = item.LicensePlateNumber,
                                            DrivingCar = item.DrivingCar,
                                            Dissent = item.Dissent,
                                            DriverIdentificationType = item.DriverIdentificationType,
                                            Remark = item.Remark,
                                            CreateTime = item.CreateTime,
                                            DelFlag = item.DelFlag
                                        };
                                        pArray.Add(entity);
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _driver.AddTDriver(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} synchronize driver data " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new driver data synchronize");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });

                // 酒检信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<TestingData> iList = await _data.GetTestingDataItemAsync();
                        if (iList.Count > 0)
                        {
                            List<TData> pArray = new();
                            // 读取基础信息
                            List<TData> datas = await _data.GetTDataItemAsync();
                            List<TDevice> devices = await _device.GetDeviceItemAsync();
                            List<SysDept> depts = await _dept.GetSysDeptItemAsync();
                            List<TPolice> polices = await _police.GetTPolicetItemAsync();
                            List<TDriverInfo> drivers = await _driver.GetDriverInfoItemAsync();
                            // 校对同步
                            iList.ForEach(item =>
                            {
                                if (item.TId != 0)
                                {
                                    // 查找判断
                                    TData? data = datas.Find(x => x.Id.Equals(item.TId));
                                    if (data == null)
                                    {
                                        var entity = new TData
                                        {
                                            Id = item.TId,
                                            RecordNum = Convert.ToString(item.DeviceTestNo),
                                            WineCheckValues = item.TestResult,
                                            Util = item.TestUnit,
                                            CreateTime = item.TestTime,
                                            UploadTime = item.TransDate,
                                            LeaderName = item.LeaderPolice,
                                            AlcoholValueState = _data.AlcoholValueStateByParams(item.TestType, item.TestClass, item.TestResult.ToString()),
                                            DutyType = !string.IsNullOrEmpty(item.DutyType) ? _data.DutyTypeByParams(item.DutyType) : 1,
                                            DeviceStatus = 0,
                                            RecordMode = _data.RecordModeByParams(item.WirelessUploadFlag),
                                            RecordStatus = 0,
                                            ImposeMeasuresCode = item.DisposalType,
                                            DelFlag = 0,
                                            Address = item.TestPlace,
                                            TestMode = _data.TestModeByParams(item.TestType),
                                            CopsName = item.TestPoliceSecond,
                                            UploadExternalStatus = 0
                                        };

                                        TPolice? police = polices.Find(x => x.PoliceNum == item.TestPoliceNo);
                                        if (police != null)
                                        {
                                            entity.PoliceId = police.Id;
                                        }

                                        TDevice? device = devices.Find(x => x.DeviceSn == item.DeviceNo);
                                        if (device != null)
                                        {
                                            entity.DeviceId = device.Id;
                                            entity.DeviceTypeId = device.DeviceTypeId;
                                        }

                                        SysDept? dept = depts.Find(x => x.Id.Equals(item.TestPoliceSta));
                                        if (dept != null)
                                        {
                                            entity.DeptId = dept.Id;
                                        }

                                        if (!string.IsNullOrEmpty(item.SubjectName) && !string.IsNullOrEmpty(item.SubjectIdNo) && !string.IsNullOrEmpty(item.LicenseNumber))
                                        {
                                            TDriverInfo? driver = drivers.Find(x => item.TestTime.Equals(x.CreateTime) && item.SubjectName.Equals(x.DriverIdentificationName) && item.SubjectIdNo.Equals(x.DriverIdentificationNumber) && item.LicenseNumber.Equals(x.LicensePlateNumber));
                                            if (driver != null)
                                            {
                                                entity.DriverInfoId = driver.Id;
                                            }
                                        }
                                        pArray.Add(entity);
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _data.AddTData(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} synchronize alcohol data " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new alcohol data synchronize");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });

                // 酒检信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<TestingData> iList = await _data.GetTestingDataItemAsync();
                        if (iList.Count > 0)
                        {
                            List<TData> pArray = new();
                            // 读取基础信息
                            List<TData> datas = await _data.GetTDataItemAsync();
                            List<TDevice> devices = await _device.GetDeviceItemAsync();
                            List<SysDept> depts = await _dept.GetSysDeptItemAsync();
                            List<TPolice> polices = await _police.GetTPolicetItemAsync();
                            List<TDriverInfo> drivers = await _driver.GetDriverInfoItemAsync();
                            // 校对同步
                            iList.ForEach(item =>
                            {
                                // 查找判断
                                TData? data = datas.Find(x => x.Id.Equals(item.TId));
                                if (data != null)
                                {
                                    if ((data.UploadExternalStatus is 2 or 3) && !string.IsNullOrEmpty(data.UploadExternalMsg) && (data.UploadExternalMsg.Contains("被测人信息不能为空") || data.UploadExternalMsg.Contains("执勤民警信息不能为空")))
                                    {
                                        if (data.AlcoholValueState is 3 or 4 or 5)
                                        {
                                            TPolice? police = polices.Find(x => x.PoliceNum == item.TestPoliceNo);
                                            if (police != null)
                                            {
                                                data.PoliceId = police.Id;
                                            }

                                            if (!string.IsNullOrEmpty(item.SubjectName) && !string.IsNullOrEmpty(item.SubjectIdNo) && !string.IsNullOrEmpty(item.LicenseNumber))
                                            {
                                                TDriverInfo? driver = drivers.Find(x => item.TestTime.Equals(x.CreateTime) && item.SubjectName.Equals(x.DriverIdentificationName) && item.SubjectIdNo.Equals(x.DriverIdentificationNumber) && item.LicenseNumber.Equals(x.LicensePlateNumber));
                                                if (driver != null)
                                                {
                                                    data.DriverInfoId = driver.Id;
                                                }
                                            }

                                            data.ImposeMeasuresCode = item.DisposalType;
                                            data.Address = item.TestPlace;
                                            data.CopsName = item.TestPoliceSecond;

                                            data.UploadExternalStatus = 0;
                                            data.UploadExternalTime = null;
                                            data.UploadExternalMsg = "";

                                            pArray.Add(data);
                                        }
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _data.UpdateTData(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} update alcohol data " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new alcohol data update");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });
            }

            // 同步数据服务 MySQLToOracle
            if (wireless.Equals("0"))
            {
                // 酒检信息
                await Task.Run(async () =>
                {
                    bool result = false;
                    try
                    {
                        List<TData> iList = await _data.GetTDataItemByMonthAsync();
                        // 同步校对
                        if (iList.Count > 0)
                        {
                            List<TestingData> pArray = new();
                            // 读取基础信息
                            List<TDevice> devices = await _device.GetDeviceItemAsync();
                            List<TPolice> polices = await _police.GetTPolicetItemAsync();
                            List<TDriverInfo> drivers = await _driver.GetDriverInfoItemAsync();
                            List<TestingData> testings = await _data.GetTestingDataItemAsync();
                            List<DeviceList> devs = await _device.GetSyncDeviceAsync();
                            // 校对同步
                            iList.ForEach(item =>
                            {
                                if (item.DeviceId != 0)
                                {
                                    // 查找判断
                                    TDevice? device = devices.Find(x => x.Id.Equals(item.DeviceId));
                                    if (device != null)
                                    {
                                        string device_sn = device.DeviceSn;
                                        // 查找判断
                                        TestingData? testing = testings.Find(x => x.TestTime.Equals(item.CreateTime) && x.DeviceTestNo.Equals(Convert.ToInt32(item.RecordNum)) && x.TransDate.Equals(item.UploadTime) && x.DeviceNo.Equals(device_sn));
                                        if (testing == null)
                                        {
                                            TestingData entity = new()
                                            {
                                                TestserialNo = Guid.NewGuid().ToByteArray(),
                                                DeviceNo = device_sn,
                                                TestDate = Convert.ToDateTime(Convert.ToDateTime(item.CreateTime.ToString()).ToString("d")),
                                                TestTime = item.CreateTime,
                                                TestType = _data.TestTypeByParams(item.TestMode),
                                                TestClass = _data.TestClassByParams(item.TestMode, item.AlcoholValueState),
                                                TestResult = (float)item.WineCheckValues,
                                                TestUnit = !string.IsNullOrEmpty(item.Util) ? item.Util.ToLower() : item.Util,
                                                TestUser = "admin",
                                                TransDate = item.UploadTime,
                                                DeviceTestNo = !string.IsNullOrEmpty(item.RecordNum) ? Convert.ToInt32(item.RecordNum) : 0,
                                                ObjectionFlag = "N",
                                                TestPoliceSecond = item.LeaderName,
                                                TestPlace = item.Address,
                                                LeaderPolice = item.LeaderName,
                                                DataType = "Normal",
                                                WirelessUploadFlag = "y",
                                                DutyType = _data.DutyTypeByParams(item.DutyType),
                                                TId = 1000000000 + item.Id
                                            };
                                            // 当事人
                                            if (item.DriverInfoId != 0)
                                            {
                                                TDriverInfo? driver = drivers.Find(x => x.Id.Equals(item.DriverInfoId));
                                                if (driver != null)
                                                {
                                                    entity.SubjectName = driver.DriverIdentificationName;
                                                    entity.SubjectIdType = !string.IsNullOrEmpty(driver.DrivingCar) ? driver.DrivingCar.ToLower() : "";
                                                    entity.SubjectIdNo = driver.DriverIdentificationNumber;
                                                    entity.LicenseNumber = driver.LicensePlateNumber;
                                                }
                                            }
                                            // 警员
                                            if (item.PoliceId != 0)
                                            {
                                                TPolice? police = polices.Find(x => x.Id.Equals(item.PoliceId));
                                                if (police != null)
                                                {
                                                    entity.TestPolice = police.PoliceNum;
                                                    entity.TestPoliceNo = police.PoliceNum;
                                                }
                                            }
                                            // 部门
                                            DeviceList? deviceList = devs.Find(x => x.Instruno.Equals(device.DeviceSn));
                                            if (deviceList != null)
                                            {
                                                entity.TestPoliceSta = Convert.ToInt32(deviceList.Instruaff);
                                                entity.DutyDaduiId = deviceList.Instruaff;
                                                entity.DutyZhongDuiId = deviceList.Instruaff;
                                            }
                                            pArray.Add(entity);
                                        }
                                    }
                                }
                            });
                            if (pArray.Count > 0)
                            {
                                result = await _data.AddTestingData(pArray.ToArray());
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u} synchronize testingdata " + (result.Equals(true) ? "succ" : "fail") + "");
                            }
                            else
                            {
                                await Console.Out.WriteLineAsync($"info：{DateTime.Now:u}  no new testingdata data synchronize");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await Console.Out.WriteLineAsync($"warning：{DateTime.Now:u}  " + ex.Message);
                    }
                });
            }
        }
    }
}
