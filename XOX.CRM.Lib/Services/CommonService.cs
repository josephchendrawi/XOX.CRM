using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XOX.CRM.Lib.Common.Constants;
using XOX.CRM.Lib.DBContext;

namespace XOX.CRM.Lib
{
    public class CommonService : ICommonService
    {
        #region GetLookupValues
        /// <summary>
        /// Get top level lookup values
        /// </summary>
        /// <param name="lookupVO"></param>
        public void GetLookupValues(LookupVO lookupVO)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from a in DBContext.XOX_T_VAL_LST
                             where (a.TYPE == lookupVO.LookupKey && a.ACTIVE_FLG == XOXConstants.Active && a.PAR_VAL_ID == null)
                             orderby a.SEQUENCE, a.CREATED
                             select a;

                lookupVO.KeyValues = new List<KeyValuePair<string, string>>();

                foreach (var s in result)
                {
                    var kv = new KeyValuePair<string, string>(s.NAME, s.VAL);

                    lookupVO.KeyValues.Add(kv);
                }
            }
        }
        #endregion

        #region GetLookupValuesChild
        public void GetLookupValuesChild(LookupVO lookupVO)
        {
            var DBContext = new CRMDbContext();
            var parentRowIdList = from a in DBContext.XOX_T_VAL_LST
                                  where (a.TYPE == lookupVO.LookupKey && a.ACTIVE_FLG == XOXConstants.Active && a.VAL == lookupVO.LookupValueParent)
                                  select a.ROW_ID;

            if (parentRowIdList.Count() == 0)
            {
                lookupVO.KeyValues = new List<KeyValuePair<string, string>>();
                return;
            }

            var parentRowId = parentRowIdList.First();

            // Query the child

            var result = from a in DBContext.XOX_T_VAL_LST
                         where (a.ACTIVE_FLG == XOXConstants.Active && a.PAR_VAL_ID == parentRowId)
                         orderby a.SEQUENCE
                         select a;

            lookupVO.KeyValues = new List<KeyValuePair<string, string>>();

            foreach (var s in result)
            {
                var kv = new KeyValuePair<string, string>(s.NAME, s.VAL);

                lookupVO.KeyValues.Add(kv);
            }
        }
        #endregion

        #region AddLookupValues
        public void AddLookupValues(LookupVO lookupVO)
        {
            using (var dbContext = new CRMDbContext())
            {
                int seq = 1;
                foreach (KeyValuePair<string, string> kvp in lookupVO.KeyValues)
                {
                    XOX_T_VAL_LST newValuePair = new XOX_T_VAL_LST();
                    newValuePair.CREATED_BY = lookupVO.CreatedBy;
                    newValuePair.CREATED = DateTime.Now;
                    newValuePair.TYPE = lookupVO.LookupKey;
                    newValuePair.NAME = kvp.Key;
                    newValuePair.VAL = kvp.Value;
                    newValuePair.SEQUENCE = seq;
                    newValuePair.ACTIVE_FLG = XOXConstants.Active;
                    seq++;

                    dbContext.XOX_T_VAL_LST.Add(newValuePair);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion

        public List<string> GetLookupTypes()
        {
            var List = new List<string>();

            using (var DBContext = new CRMDbContext())
            {
                var result = from a in DBContext.XOX_T_VAL_LST
                             where (a.ACTIVE_FLG == XOXConstants.Active && a.PAR_VAL_ID == null)
                             select a.TYPE;
                result = result.Distinct();

                foreach (var s in result)
                {
                    List.Add(s);
                }
            }

            return List;
        }
        
        public void EditLookupValues(LookupVO LookupVO)
        {
            using (var dbContext = new CRMDbContext())
            {
                for (int i = 0; i < LookupVO.KeyValues.Count(); i++)
                {
                    var keyvalue = LookupVO.KeyValues[i];
                    var newkeyvalue = LookupVO.NewKeyValues[i];
                    var result = from a in dbContext.XOX_T_VAL_LST
                                 where (a.NAME == keyvalue.Key && a.VAL == keyvalue.Value && a.TYPE == LookupVO.LookupKey)
                                 select a;

                    if (result.Count() > 0)
                    {
                        result.First().NAME = newkeyvalue.Key;
                        result.First().VAL = newkeyvalue.Value;
                        result.First().LAST_UPD = DateTime.Now;
                        result.First().LAST_UPD_BY = LookupVO.CreatedBy;

                        dbContext.SaveChanges();
                    }
                }
            }
        }

        public void UpdateSequence(string Type, string Value, int Sequence)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from a in DBContext.XOX_T_VAL_LST
                             where (a.TYPE == Type && a.VAL == Value && a.ACTIVE_FLG == XOXConstants.Active)
                             orderby a.SEQUENCE, a.CREATED
                             select a;

                if (result.Count() > 0)
                {
                    result.First().SEQUENCE = Sequence;
                    result.First().LAST_UPD = DateTime.Now;
                    DBContext.SaveChanges();
                }
            }
        }

        public string GetLookupNameByValue(string Val, string Type)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from a in DBContext.XOX_T_VAL_LST
                             where (a.TYPE == Type && a.VAL == Val && a.ACTIVE_FLG == XOXConstants.Active && a.PAR_VAL_ID == null)
                             orderby a.SEQUENCE, a.CREATED
                             select a;

                if (result.Count() > 0)
                {
                    return result.First().NAME;
                }
                else
                {
                    return null;
                }
            }
        }

        public string GetLookupValueByName(string Name, string Type)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from a in DBContext.XOX_T_VAL_LST
                             where (a.TYPE == Type && a.NAME == Name && a.ACTIVE_FLG == XOXConstants.Active && a.PAR_VAL_ID == null)
                             orderby a.SEQUENCE, a.CREATED
                             select a;

                if (result.Count() > 0)
                {
                    return result.First().VAL;
                }
                else
                {
                    return "";
                }
            }
        }

        public string MapDonorName(string DonorName)
        {
            using (var DBContext = new CRMDbContext())
            {
                var result = from a in DBContext.XOX_T_VAL_LST
                             where (a.TYPE == "Donor Mapping" && a.NAME.ToLower() == DonorName.ToLower() && a.ACTIVE_FLG == XOXConstants.Active)
                             orderby a.SEQUENCE, a.CREATED
                             select a;

                if (result.Count() > 0)
                {
                    return result.First().VAL;
                }
                else
                {
                    return null;
                }
            }
        }
                
        public string MaskCreditCardDigit(string Digit)
        {
            var result = "";
            result = Digit.Substring(0, 4) + "****" + Digit.Substring(Digit.Length - 4);

            return result;
        }

        public static string Zip(string value)
        {
            //Transform string into byte[]  
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for compress
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream sw = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Compress);

            //Compress
            sw.Write(byteArray, 0, byteArray.Length);
            //Close, DO NOT FLUSH cause bytes will go missing...
            sw.Close();

            //Transform byte[] zip data to string
            byteArray = ms.ToArray();
            System.Text.StringBuilder sB = new System.Text.StringBuilder(byteArray.Length);
            foreach (byte item in byteArray)
            {
                sB.Append((char)item);
            }
            ms.Close();
            sw.Dispose();
            ms.Dispose();
            return sB.ToString();
        }

        public static string UnZip(string value)
        {
            //Transform string into byte[]
            byte[] byteArray = new byte[value.Length];
            int indexBA = 0;
            foreach (char item in value.ToCharArray())
            {
                byteArray[indexBA++] = (byte)item;
            }

            //Prepare for decompress
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            System.IO.Compression.GZipStream sr = new System.IO.Compression.GZipStream(ms,
                System.IO.Compression.CompressionMode.Decompress);

            //Reset variable to collect uncompressed result
            byteArray = new byte[byteArray.Length];

            //Decompress
            int rByte = sr.Read(byteArray, 0, byteArray.Length);

            //Transform byte[] unzip data to string
            System.Text.StringBuilder sB = new System.Text.StringBuilder(rByte);
            //Read the number of bytes GZipStream red and do not a for each bytes in
            //resultByteArray;
            for (int i = 0; i < rByte; i++)
            {
                sB.Append((char)byteArray[i]);
            }
            sr.Close();
            ms.Close();
            sr.Dispose();
            ms.Dispose();
            return sB.ToString();
        }

    }
}

//insert into xox_t_val_lst
//(created_by,created, type, name, val, SEQUENCE, ACTIVE_FLG)
//values
//(1, getdate(), 'Country', 'MALAYSIA','MALAYSIA', 1, 1)
