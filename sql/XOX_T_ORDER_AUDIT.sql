
--XOX_T_ORDER_AUDIT
DROP TABLE XOX_T_ORDER_AUDIT

CREATE TABLE XOX_T_ORDER_AUDIT
(
 ROW_ID BIGINT IDENTITY(1,1) NOT NULL
,CREATED_BY	BIGINT
,CREATED DATETIME
,LAST_UPD_BY BIGINT
,LAST_UPD DATETIME
,ACCNT_ID BIGINT
,ORDER_ID BIGINT
,MSISDN NVARCHAR(MAX)
,[PLAN] NVARCHAR(MAX)
,CATEGORY NVARCHAR(MAX)
,SIM_SERIAL_NUMBER NVARCHAR(MAX)
,SUBMISSION_DT DATETIME
,SALUTATION NVARCHAR(MAX)
,NAME NVARCHAR(MAX)
,DOB DATETIME
,GENDER INT
,ID_TYPE NVARCHAR(MAX)
,ID_NUM NVARCHAR(MAX)
,MOTHER_MAIDEN_NAM NVARCHAR(MAX)
,EMAIL NVARCHAR(MAX)
,MOBILE_NO NVARCHAR(MAX)
,NATIONALITY NVARCHAR(MAX)
,RACE NVARCHAR(MAX)
,PREF_LANG NVARCHAR(MAX)
,BANK_NAME NVARCHAR(MAX)
,BANK_ACCNT_NUMBER NVARCHAR(MAX)
,ITEMISED_BILLING BIT
,ADDR_1 NVARCHAR(MAX)
,ADDR_2 NVARCHAR(MAX)
,CITY NVARCHAR(MAX)
,[STATE] NVARCHAR(MAX)
,POSTCODE NVARCHAR(MAX)
,COUNTRY NVARCHAR(MAX)
,BILL_ADDR_1 NVARCHAR(MAX)
,BILL_ADDR_2 NVARCHAR(MAX)
,BILL_CITY NVARCHAR(MAX)
,BILL_STATE NVARCHAR(MAX)
,BILL_POSTCODE NVARCHAR(MAX)
,BILL_COUNTRY NVARCHAR(MAX)
,BILL_CARD_TYPE NVARCHAR(MAX)
,BILL_CARD_NUMBER NVARCHAR(MAX)
,BILL_ACCNT_NAME NVARCHAR(MAX)
,BILL_BANK_ISSUER NVARCHAR(MAX)
,BILL_EXPIRY_YEAR INT
,BILL_EXPIRY_MONTH INT
,BILL_THIRD_PARTY BIT
,MSISDN_SUPPLINE NVARCHAR(MAX)
 CONSTRAINT XOX_T_ORDER_AUDIT_P01 PRIMARY KEY CLUSTERED 
(
	ROW_ID
)
)