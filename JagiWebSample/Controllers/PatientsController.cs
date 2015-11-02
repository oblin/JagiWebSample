using AutoMapper;
using Jagi.Database.Mvc;
using Jagi.Helpers;
using Jagi.Interface;
using Jagi.Utility;
using JagiWebSample.Models;
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

        public PatientsController(DataContext context)
        {
            _context = context;
        }

        public ActionResult Index(PageInfo pageInfo = null)
        {
            PagedView pagedView = GetPagedPatients(pageInfo);
            return View(pagedView);
        }

        [HttpGet, OutputCache(Duration = 0)]
        public JsonResult GetPaged(PageInfo pageInfo)
        {
            return GetJsonResult(() =>
            {
                return GetPagedPatients(pageInfo);
            });
        }

        [HttpGet, OutputCache(Duration=0)]
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

        private PagedView GetPagedPatients(PageInfo pageInfo)
        {
            PagedView pagedView = null;
            ViewBag.Timer = Jagi.Utility.Tools.Timing(() =>
            {
                if (pageInfo == null || pageInfo.PageNumber == 0 || pageInfo.PageSize == 0)
                    pageInfo = InitializePageInfo();
                var patients = GetFilteredPatients(pageInfo);
                int count = patients.Count();
                patients = GetPagedSize(patients, pageInfo);
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

        private IEnumerable<Patient> GetFilteredPatients(PageInfo pageInfo)
        {
            IEnumerable<Patient> patients = _context.Patients;
            if (AsEnumerableField.Contains(pageInfo.SortField, StringComparer.OrdinalIgnoreCase)
                || AsEnumerableField.Contains(pageInfo.SearchField, StringComparer.OrdinalIgnoreCase))
                // 必須使用要加密欄位，因此要將所有資料讀入後再進行處理
                patients = _context.Patients.AsNoTracking().ToList();

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
                _context.SaveChanges();
                return View("Info");
            });
        }

        protected override void ExecuteCore()
        {
            throw new NotImplementedException();
        }
    }
}