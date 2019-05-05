
--XOX_T_ACCNT
DROP TABLE XOX_T_ACCNT_BANK

CREATE TABLE XOX_T_ACCNT_BANK
(
 ROW_ID BIGINT IDENTITY(1,1) NOT NULL
,CREATED_BY	BIGINT
,CREATED DATETIME
,LAST_UPD_BY BIGINT
,LAST_UPD DATETIME
,CUST_ID BIGINT
,BANK_NAME NVARCHAR(100)
,BANK_ACCNT_NO NVARCHAR(100)
,PAYMENT_METHOD NVARCHAR(100)
,CREDIT_CARD_NO NVARCHAR(255)
,CREDIT_CARD_SALT NVARCHAR(255)
,CREDIT_CARD_ISSUER NVARCHAR(100)
,CREDIT_CARD_HOLDER NVARCHAR(255)
,CREDIT_CARD_EXPIRY_MONTH INT
,CREDIT_CARD_EXPIRY_YEAR INT
,THIRD_PARTY_FLG BIT

 CONSTRAINT XOX_T_ACCNT_BANK_P01 PRIMARY KEY CLUSTERED 
(
	ROW_ID
)
)

CREATE INDEX XOX_T_ACCNT_BANK_F01 ON XOX_T_ACCNT_BANK (CUST_ID)

CREATE INDEX XOX_T_ACCNT_BANK_M01 ON XOX_T_ACCNT_BANK (BANK_NAME)
CREATE INDEX XOX_T_ACCNT_BANK_M02 ON XOX_T_ACCNT_BANK (PAYMENT_METHOD)
