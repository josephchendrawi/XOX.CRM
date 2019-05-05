alter table [XOX_T_ACCNT]
add GENDER int,
	PREFERRED_LANG nvarchar(100),
	RACE nvarchar(50),
	SPONSOR_PERSONNEL nvarchar(200),
	BANK_ACC_NUM nvarchar(50),
	BANK_ISSUER nvarchar(50),
	BANK_THIRD_PARTY bit,
	BANK_EXPIRY_YEAR int,
	BANK_EXPIRY_MONTH int,
	BILL_CARD_TYPE nvarchar(50)