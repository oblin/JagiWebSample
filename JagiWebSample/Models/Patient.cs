using Jagi.Interface;
using Jagi.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace JagiWebSample.Models
{
    public class Patient : Entity
    {
        private static AESCryptoProvider _provider = null;

        public Patient()
        {
            if (_provider == null)
                _provider = new AESCryptoProvider();
        }

        [StringLength(11)]
        public string ChartId { get; set; }

        [StringLength(5)]
        public string ChId { get; set; }

        [StringLength(2)]
        public string Type { get; set; }

        public DateTime BirthDate { get; set; }

        [StringLength(1)]
        public string Sex { get; set; }

        [StringLength(1)]
        public string Marry { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(10)]
        public string CureDoc { get; set; }

        [StringLength(6)]
        public string County0 { get; set; }

        [StringLength(6)]
        public string County { get; set; }

        [StringLength(8)]
        public string Realm { get; set; }

        [StringLength(5)]
        public string Mailno { get; set; }

        [StringLength(8)]
        public string Village { get; set; }

        [StringLength(16)]
        public string Street { get; set; }

        [StringLength(2)]
        public string Educate { get; set; }

        [StringLength(2)]           // 職業
        public string Vocation { get; set; }

        [StringLength(20)]          // 聯絡人
        public string Father { get; set; }

        [StringLength(1)]           // 關係
        public string Relation { get; set; }

        [StringLength(3)]           // 血型
        public string Ab { get; set; }

        [StringLength(10)]          // 重大傷病卡
        public string HeartCard { get; set; }

        // 原住民
        public bool IsNative { get; set; }

        // 福保身分
        public bool IsWelfare { get; set; }

        [Required, StringLength(1)] // 狀態
        public string Status { get; set; }


        [StringLength(1)]           // 前一狀態
        public string Status1 { get; set; }

        // 狀態改變日期，沒有使用，以 Drop 內容為準
        public DateTime? Date { get; set; }

        // 首次治療日期，沒有使用，以 Start 內容為準
        public DateTime? FirstDate { get; set; }

        // 本院開始日期，沒有使用，以 Start 內容為準
        public DateTime BeginDate { get; set; }

        // 退出日期，沒有使用，以 Drop 內容為準
        public DateTime? DropDate { get; set; }

        // 死亡日期，沒有使用，以 Drop 內容為準
        public DateTime? DieDate { get; set; }

        // 原發病大類，沒有使用，以 Start 內容為準
        [StringLength(1)]
        public string Idiopa { get; set; }

        // 原發病細類，沒有使用，以 Start 內容為準
        [StringLength(8)]
        public string Idiopathy { get; set; }

        // 副甲狀腺切除，沒有使用
        public bool IsPTX { get; set; }

        // 副甲狀腺切除日，沒有使用
        public DateTime? PtxDate { get; set; }

        // 沒有使用
        public DateTime? HcvDate { get; set; }
    
        [StringLength(4)]    // 等候腎移植院所
        public string NephHosp { get; set; }

        // 首次登錄日期
        public DateTime? NephStartDate { get; set; }

        // 最近一次迴診日期
        public DateTime? NephLastDate { get; set; }

        // 簽署安寧緩和日期
        public DateTime? PeaceDate { get; set; }

        [Column("NAME"), StringLength(32)]
        public string EncryptName { get; set; }

        [Required]
        public virtual string Name
        {
            get { return _provider.Decrypt(EncryptName); }
            set { EncryptName = _provider.Encrypt(value); }
        }

        [Column("STREETNO"), StringLength(72)]
        public string EncryptStreetNo { get; set; }

        public string StreetNo 
        {
            get { return _provider.Decrypt(EncryptStreetNo); }
            set { EncryptStreetNo = _provider.Encrypt(value); }
        }

        [Column("TELNO"), StringLength(64)]
        public string EncryptTelno { get; set; }

        public string Telno
        {
            get { return _provider.Decrypt(EncryptTelno); }
            set { EncryptTelno = _provider.Encrypt(value); }
        }

        [Column("MOBILENO"), StringLength(64)]
        public string EncryptMobileNo { get; set; }

        public string MobileNo
        {
            get { return _provider.Decrypt(EncryptMobileNo); }
            set { EncryptMobileNo = _provider.Encrypt(value); }
        }

        [Column("IDCARD"), StringLength(48)]
        public string EncryptIdCard { get; set; }


        public bool? IsForeign { get; set; }

        [Required]              // 身分證號
        public string IdCard
        {
            get { return _provider.Decrypt(EncryptIdCard); }
            set { EncryptIdCard = _provider.Encrypt(value); }
        }

        public static Expression<Func<Patient, bool>> NameStartWith(string name)
        {
            return p => p.Name.StartsWith(name);
        }

        public static Expression<Func<Patient, object>> NameOrderby()
        {            
            return p => _provider.Decrypt(p.EncryptName);
        }
    }
}