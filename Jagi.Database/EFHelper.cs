using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Database
{
    public static class EFHelper
    {
        public static string Save(this DbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var message = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += String.Format("Class: {0}, Property: {1}, Error: {2}\n", validationErrors.Entry.Entity.GetType().FullName,
                                      validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return message;
            }
            catch (DbUpdateException updExc)
            {
                return updExc.InnerException.InnerException.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }
    }
}
