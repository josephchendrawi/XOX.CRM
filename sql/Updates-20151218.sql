alter table [XOX_T_ACCNT_PAYMENT]
add PAYMENT_METHOD nvarchar(255),
	BANK_ISSUER nvarchar(255),
	CARD_TYPE nvarchar(255),
	CARD_NUMBER nvarchar(255),
	CARD_EXPIRY_DATE nvarchar(255)