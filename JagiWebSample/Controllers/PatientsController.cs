using AutoMapper;
using Jagi.Database.Cache;
using Jagi.Database.Mvc;
using Jagi.Helpers;
using Jagi.Interface;
using Jagi.Utility;
using JagiWebSample.Areas.Admin.Models;
using JagiWebSample.Models;
using JagiWebSample.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JagiWebSample.Controllers
{
    public class PatientsController : JagiController
    {
        private readonly string[] AsEnumerableField = { "Name", "IdCard", "BirthDay" };
        private DataContext _context;
        private CodeCache _codes;

        public PatientsController(DataContext context)
        {
            _context = context;
            _codes = new CodeCache();
        }

        [RetrieveLog]
        public ActionResult Index(PageInfo pageInfo = null, string status = null)
        {
            PagedView pagedView = GetPagedPatients(pageInfo, status);
            ViewBag.Codes = GetUIDisplayCodes();
            ViewBag.Status = GetStatusSelections(status);
            ViewBag.CurrentStatus = status;
            ViewBag.Counties = GetAllCounties();
            return View(pagedView);
        }

        [HttpGet, RetrieveLog, OutputCache(Duration = 0)]
        public JsonResult GetPaged(PageInfo pageInfo, string status = null)
        {
            return GetJsonResult(() =>
            {
                return GetPagedPatients(pageInfo, status);
            });
        }

        [HttpGet, OutputCache(Duration = 0)]
        public JsonResult Get(int id)
        {
            if (id == 0)
                return JsonError("病患 id 不可以為零～");

            return GetJsonResult(() =>
            {
                var patient = _context.Patients.Find(id);
                if (patient == null)
                    throw new NullReferenceException("Id: {0} 找不對應的病人".FormatWith(id));

                return Mapper.Map<PatientEditView>(patient);
            });
        }

        [HttpPost]
        public JsonResult Save(PatientEditView model)
        {
            if (!ModelState.IsValid || !ColumnsValid(model))
                return JsonValidationError();

            Patient patient = null;
            if (model.Id > 0)
            {
                patient = _context.Patients.Find(model.Id);
                Mapper.Map(model, patient);
            }
            else
            {
                patient = Mapper.Map<Patient>(model);
                _context.Patients.Add(patient);
            }

            return GetJsonResult(() =>
            {
                _context.Save();
                return patient;
            });
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            Patient patient = _context.Patients.Find(id);
            if (patient == null)
                return JsonError("找不到 id = {0} 的病人".FormatWith(id));

            return GetJsonResult(() =>
            {
                _context.Patients.Remove(patient);
                _context.Save();
                return JsonSuccess();
            });
        }

        [HttpGet]
        public JsonResult GetAddrByZip(string id)
        {
            return GetJsonResult(() =>
            {
                using (var context = new AdminDataContext())
                {
                    var result = context.Address.Where(p => p.Zip == id);
                    if (result.Any())
                        return new
                        {
                            County = result.First().County,
                            Realm = result.First().Realm,
                            Villages = result.Select(s => s.Street).ToArray()
                        };

                    throw new NullReferenceException("找不到此 zip: {0} 的資料".FormatWith(id));
                }
            });
        }

        [HttpGet]
        public JsonResult GetAddrByCounty(string id)
        {
            return GetJsonResult(() =>
            {
                using (var context = new AdminDataContext())
                {
                    var result = context.Address.Where(p => p.County == id);
                    if (result.Any())
                    {
                        var realms = result.GroupBy(g => g.Realm).Select(s => s.Key).ToArray();
                        return new
                        {
                            County = result.First().County,
                            Realms = realms,
                            Villages = result.Select(s => s.Street).ToArray()
                        };
                    }

                    throw new NullReferenceException("找不到此 county: {0} 的資料".FormatWith(id));
                }
            });
        }

        [HttpGet]
        public JsonResult GetAddrByRealm(string id)
        {
            return GetJsonResult(() =>
            {
                using (var context = new AdminDataContext())
                {
                    var result = context.Address.Where(p => p.Realm == id);
                    if (result.Any())
                    {
                        return new
                        {
                            Villages = result.Select(s => s.Street).ToArray()
                        };
                    }

                    throw new NullReferenceException("找不到此 realm: {0} 的資料".FormatWith(id));
                }
            });
        }

        [HttpGet]
        public JsonResult GetAddrByVillage(string county, string realm, string village)
        {
            return GetJsonResult(() =>
            {
                using (var context = new AdminDataContext())
                {
                    var result = context.Address
                                    .Where(p => p.Realm == realm && p.County == county && p.Street == village);

                    if (result.Any())
                    {
                        if (result.Count() > 1)
                            throw new IndexOutOfRangeException("Street: {0} 傳回的 zip 大於 1".FormatWith(village));
                        return new
                        {
                            zip = result.First().Zip
                        };
                    }

                    throw new NullReferenceException("找不到此 Village: {0} 的資料".FormatWith(village));
                }
            });
        }

        private string[] GetAllCounties()
        {
            using (AdminDataContext context = new AdminDataContext())
            {
                var group = context.Address.GroupBy(g => g.County).Select(g => g.Key);
                return group.ToArray();
            }
        }

        private List<SelectListItem> GetStatusSelections(string status)
        {
            var statusCodes = _codes.GetCodeDetails("Status").OrderBy(o => o.ItemCode);
            var result = new List<SelectListItem> { new SelectListItem {
                    Selected = string.IsNullOrEmpty(status),
                    Text = "全部",
                    Value = ""
                }};

            foreach (var code in statusCodes)
                result.Add(new SelectListItem
                {
                    Selected = code.ItemCode == status,
                    Value = code.ItemCode,
                    Text = code.Description
                });

            return result;
        }

        private Dictionary<string, Dictionary<string, string>> GetUIDisplayCodes()
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            // key value 一律小寫，避免不必要的錯誤
            result.Add("status", _codes.GetDetails("Status"));
            result.Add("ab", _codes.GetDetails("AB"));

            return result;
        }

        private PagedView GetPagedPatients(PageInfo pageInfo, string status)
        {
            PagedView pagedView = null;
            IEnumerable<Patient> patients = _context.Patients.AsNoTracking();
            if (!string.IsNullOrEmpty(status))
                patients = patients.Where(p => p.Status == status);

            ViewBag.Timer = Jagi.Utility.Tools.Timing(() =>
            {
                if (pageInfo == null || pageInfo.PageNumber == 0 || pageInfo.PageSize == 0)
                    pageInfo = InitializePageInfo();
                patients = GetFilteredPatients(pageInfo, patients);
                int count = patients.Count();
                patients = TakePagedResult(patients, pageInfo);
                var patientListView = Mapper.Map<IEnumerable<PatientListView>>(patients);

                pagedView = new PagedView
                {
                    Data = patientListView,
                    TotalCount = count,
                    CurrentPage = pageInfo.PageNumber,
                    PageCount = pageInfo.PageSize,
                    Headers = EntityHelper.GetDisplayName(new PatientListView())
                };
            });
            return pagedView;
        }

        private IEnumerable<Patient> GetFilteredPatients(PageInfo pageInfo, IEnumerable<Patient> patients)
        {
            if (AsEnumerableField.Contains(pageInfo.SortField, StringComparer.OrdinalIgnoreCase)
                || AsEnumerableField.Contains(pageInfo.SearchField, StringComparer.OrdinalIgnoreCase))
                // 必須使用要加密欄位，因此要將所有資料讀入後再進行處理
                patients = patients.ToList();

            if (!string.IsNullOrEmpty(pageInfo.SearchKeyword))
                if (pageInfo.SearchField == "ChartId")
                    patients = patients.ContainsWithFieldName(pageInfo.SearchField, pageInfo.SearchKeyword);
                else
                    patients = patients.StartWithFieldName(pageInfo.SearchField, pageInfo.SearchKeyword);

            patients = OrderByFieldName(patients, pageInfo.SortField, pageInfo.Sort);
            return patients;
        }

        private IEnumerable<Patient> OrderByFieldName(IEnumerable<Patient> patients, string sortField, string sort)
        {
            if (string.IsNullOrEmpty(sortField))
                return patients;
            sort = sort ?? "asc";
            switch (sortField.ToLower())
            {
                case "name":
                    if (sort.ToLower() == "desc")
                        return patients.OrderByDescending(o => o.Name);
                    return patients.OrderBy(o => o.Name);
                case "idcard":
                    if (sort.ToLower() == "desc")
                        return patients.OrderByDescending(o => o.IdCard);
                    return patients.OrderBy(o => o.IdCard);
                case "birthday":
                    if (sort.ToLower() == "desc")
                        return patients.OrderByDescending(o => o.BirthDate);
                    return patients.OrderBy(o => o.BirthDate);
            }
            return patients.OrderByFieldName(sortField, sort);
        }

        private PageInfo InitializePageInfo()
        {
            return new PageInfo
            {
                PageNumber = 1,
                PageSize = 25,
                //SortField = "Name"
            };
        }

        public ActionResult ConvertDescToAsc()
        {
            var cryptoSetting = new CryptoSetting("JagiHope", "Kidit123");
            var cryptoProvider = new DESCryptoProvider(cryptoSetting);

            var patients = _context.Patients;
            foreach (var patient in patients)
            {
                patient.IdCard = cryptoProvider.Decrypt(patient.EncryptIdCard);
                patient.MobileNo = cryptoProvider.Decrypt(patient.EncryptMobileNo);
                patient.Name = cryptoProvider.Decrypt(patient.EncryptName);
                patient.StreetNo = cryptoProvider.Decrypt(patient.EncryptStreetNo);
                patient.Telno = cryptoProvider.Decrypt(patient.EncryptTelno);
            }

            return GetActionResult(() =>
            {
                _context.Save();
                return View("Info");
            });
        }

        protected override void ExecuteCore()
        {
            throw new NotImplementedException();
        }
    }
}