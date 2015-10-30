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
            PagedView pagedView = null;
            ViewBag.Timer = Jagi.Utility.Tools.Timing(() =>
            {
                if (pageInfo == null || pageInfo.PageNumber == 0 || pageInfo.PageSize == 0)
                    pageInfo = InitializePageInfo();
                var patients = GetFilteredPatients(pageInfo);
                var patientListView = Mapper.Map<IEnumerable<PatientListView>>(patients);

                pagedView = new PagedView
                {
                    Data = patientListView,
                    TotalCount = patientListView.Count(),
                    CurrentPage = pageInfo.PageNumber,
                    PageCount = pageInfo.PageSize,
                    Headers = EntityHelper.GetDisplayName(new PatientListView())
                };
            });
            return View(pagedView);
        }

        private IEnumerable<Patient> GetFilteredPatients(PageInfo pageInfo)
        {
            IEnumerable<Patient> patients = _context.Patients;
            if (AsEnumerableField.Contains(pageInfo.SortField) || AsEnumerableField.Contains(pageInfo.SearchField))
                patients = _context.Patients.AsNoTracking().ToList();

            patients = patients.StartWithFieldName(pageInfo.SearchField, pageInfo.SearchKeyword);
            patients = patients.OrderByFieldName(pageInfo.SortField, pageInfo.Sort);
            return patients;
        }

        private IEnumerable<Patient> StartWithFieldName(IEnumerable<Patient> patients, string searchField, string searchKeyword)
        {
            //return patients.Where(Patient.NameStartWith(searchKeyword));
            return patients;
        }

        private IEnumerable<Patient> OrderByFieldName(IEnumerable<Patient> patients, string sortField, string sort)
        {
            return patients.AsQueryable().OrderBy(o => o.Name);
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